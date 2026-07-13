using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace SkillBridge.Web.Services;

public sealed class ApiService
{
    private readonly HttpClient _client;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public ApiService(HttpClient client, IHttpContextAccessor httpContextAccessor)
    {
        _client = client;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<ApiResult<T>> GetAsync<T>(string path, CancellationToken cancellationToken = default) =>
        SendAsync<T>(HttpMethod.Get, path, null, cancellationToken);

    public Task<ApiResult<T>> PostAsync<T>(string path, object body, CancellationToken cancellationToken = default) =>
        SendAsync<T>(HttpMethod.Post, path, body, cancellationToken);

    public Task<ApiResult<T>> PutAsync<T>(string path, object body, CancellationToken cancellationToken = default) =>
        SendAsync<T>(HttpMethod.Put, path, body, cancellationToken);

    public Task<ApiResult<object>> DeleteAsync(string path, CancellationToken cancellationToken = default) =>
        SendAsync<object>(HttpMethod.Delete, path, null, cancellationToken);

    private async Task<ApiResult<T>> SendAsync<T>(
        HttpMethod method,
        string path,
        object? body,
        CancellationToken cancellationToken)
    {
        try
        {
            using var request = new HttpRequestMessage(method, path);
            var token = _httpContextAccessor.HttpContext?.Session.GetString(SessionKeys.AccessToken);
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            if (body is not null)
            {
                request.Content = JsonContent.Create(body, options: JsonOptions);
            }

            using var response = await _client.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return ApiResult<T>.Success(default, response.StatusCode);
                }

                var data = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
                return ApiResult<T>.Success(data, response.StatusCode);
            }

            return ApiResult<T>.Failure(
                response.StatusCode,
                await ReadErrorAsync(response, cancellationToken));
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return ApiResult<T>.Failure(HttpStatusCode.RequestTimeout, "The API request timed out. Please try again.");
        }
        catch (HttpRequestException)
        {
            return ApiResult<T>.Failure(HttpStatusCode.ServiceUnavailable, "The API is currently unavailable. Please try again later.");
        }
        catch (JsonException)
        {
            return ApiResult<T>.Failure(HttpStatusCode.BadGateway, "The API returned an unexpected response.");
        }
    }

    private static async Task<string> ReadErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(content))
            {
                return DefaultMessage(response.StatusCode);
            }

            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;
            if (root.TryGetProperty("message", out var message) &&
                message.ValueKind == JsonValueKind.String)
            {
                return message.GetString() ?? DefaultMessage(response.StatusCode);
            }

            if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
            {
                var messages = errors.EnumerateObject()
                    .SelectMany(error => error.Value.EnumerateArray())
                    .Where(value => value.ValueKind == JsonValueKind.String)
                    .Select(value => value.GetString())
                    .Where(value => !string.IsNullOrWhiteSpace(value));

                var combined = string.Join(" ", messages!);
                if (!string.IsNullOrWhiteSpace(combined))
                {
                    return combined;
                }
            }
        }
        catch (JsonException)
        {
            // Fall through to a safe status-based message.
        }

        return DefaultMessage(response.StatusCode);
    }

    private static string DefaultMessage(HttpStatusCode statusCode) => statusCode switch
    {
        HttpStatusCode.Unauthorized => "Please sign in to continue.",
        HttpStatusCode.Forbidden => "You do not have permission to perform this action.",
        HttpStatusCode.NotFound => "The requested item was not found.",
        HttpStatusCode.Conflict => "The request conflicts with existing data.",
        _ => "The request could not be completed. Please try again."
    };
}

public sealed record ApiResult<T>(bool IsSuccess, T? Data, HttpStatusCode StatusCode, string? Error)
{
    public static ApiResult<T> Success(T? data, HttpStatusCode statusCode) =>
        new(true, data, statusCode, null);

    public static ApiResult<T> Failure(HttpStatusCode statusCode, string error) =>
        new(false, default, statusCode, error);
}
