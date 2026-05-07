using System;
using System.ComponentModel.DataAnnotations;

public class User
{
    [Required]
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    public string Type { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }
}