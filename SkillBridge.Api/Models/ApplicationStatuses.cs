public static class ApplicationStatuses
{
    public const string Pending = "Pending";
    public const string Reviewed = "Reviewed";
    public const string Shortlisted = "Shortlisted";
    public const string Rejected = "Rejected";
    public const string Accepted = "Accepted";

    public static bool TryNormalize(string? value, out string normalizedStatus)
    {
        var statuses = new[]
        {
            Pending,
            Reviewed,
            Shortlisted,
            Rejected,
            Accepted
        };

        normalizedStatus = statuses.FirstOrDefault(status =>
            string.Equals(status, value?.Trim(), StringComparison.OrdinalIgnoreCase))
            ?? string.Empty;

        return normalizedStatus.Length > 0;
    }
}
