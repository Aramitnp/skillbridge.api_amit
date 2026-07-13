public static class SupportedUserTypes
{
    public const string Applicant = "Applicant";
    public const string Company = "Company";

    public static bool TryNormalize(string? value, out string normalizedType)
    {
        if (string.Equals(value?.Trim(), Applicant, StringComparison.OrdinalIgnoreCase))
        {
            normalizedType = Applicant;
            return true;
        }

        if (string.Equals(value?.Trim(), Company, StringComparison.OrdinalIgnoreCase))
        {
            normalizedType = Company;
            return true;
        }

        normalizedType = string.Empty;
        return false;
    }
}
