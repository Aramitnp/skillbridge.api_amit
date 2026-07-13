public sealed class JobApplicationResponseDto
{
    public int Id { get; init; }
    public int JobId { get; init; }
    public string JobTitle { get; init; } = string.Empty;
    public string CompanyName { get; init; } = string.Empty;
    public int ApplicantId { get; init; }
    public string ApplicantName { get; init; } = string.Empty;
    public string ApplicantEmail { get; init; } = string.Empty;
    public string CoverLetter { get; init; } = string.Empty;
    public DateTime AppliedDate { get; init; }
    public string Status { get; init; } = string.Empty;
}
