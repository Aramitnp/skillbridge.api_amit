using System;
using System.ComponentModel.DataAnnotations;

public class User
{
    [Required]
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public ICollection<Job> Jobs { get; set; } = new List<Job>();

    public ICollection<JobApplication> JobApplications { get; set; } =
        new List<JobApplication>();
}
