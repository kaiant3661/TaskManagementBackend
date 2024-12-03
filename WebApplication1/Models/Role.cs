using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Role
    {
        public int RoleId { get; set; }

        [Required(ErrorMessage = "Role name is required.")]
        [StringLength(100, ErrorMessage = "Role name cannot be longer than 100 characters.")]
        public string RoleName { get; set; }

        // Navigation property for the related Users
        public ICollection<Userr> Users { get; set; } = new List<Userr>(); // Initialize to avoid null reference issues
    }
}
