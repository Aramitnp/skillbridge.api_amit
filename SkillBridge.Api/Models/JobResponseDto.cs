public sealed class JobResponseDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal MinimumSalary { get; init; }
    public decimal MaximumSalary { get; init; }
    public string Location { get; init; } = string.Empty;
    public string JobType { get; init; } = string.Empty;
    public DateTime PostedDate { get; init; }
    public DateTime Deadline { get; init; }
    public bool IsActive { get; init; }
    public int CompanyId { get; init; }
    public string CompanyName { get; init; } = string.Empty;
}
