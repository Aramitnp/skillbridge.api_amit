public sealed class PaginatedJobResponseDto
{
    public int CurrentPage { get; init; }
    public int PageSize { get; init; }
    public int TotalRecords { get; init; }
    public int TotalPages { get; init; }
    public IReadOnlyList<JobResponseDto> Items { get; init; } =
        Array.Empty<JobResponseDto>();
}
