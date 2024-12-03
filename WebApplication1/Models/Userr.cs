using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

public class Userr
{
    public int UserId { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100, ErrorMessage = "First name cannot be longer than 100 characters.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100, ErrorMessage = "Last name cannot be longer than 100 characters.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    [StringLength(200, ErrorMessage = "Email cannot be longer than 200 characters.")]
    public string Email { get; set; }

    public string? PasswordHash { get; set; }

    // Default empty salt string, can be generated in the service
    public string PasswordSalt { get; set; } = string.Empty;

    // Default RoleId (e.g., 1 for a basic user)
    public int RoleId { get; set; } = 3;

    // Default Role object to avoid null references (if needed)
    public Role? Role { get; set; }


    // Set default CreatedAt to the current date and time
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Allow nullable UpdatedAt
    public DateTime? UpdatedAt { get; set; }

    // Soft delete properties
    public bool? IsDeleted { get; set; } = false;   // Soft delete flag
    public DateTime? DeletedAt { get; set; }       // Timestamp when deleted
}
