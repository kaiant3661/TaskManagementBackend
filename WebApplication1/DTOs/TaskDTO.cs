using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs
{
    public class TaskDTO
    {
        [Required(ErrorMessage = "Task name is required.")]
        [StringLength(200, ErrorMessage = "Task name cannot be longer than 200 characters.")]
        public string TaskName { get; set; }

        [Required]
        [RegularExpression("^(Pending|InProgress|Completed)$", ErrorMessage = "Invalid status. Allowed values: Pending, In Progress, Completed.")]
        public string Status { get; set; } = "Pending"; // Default value

        [Required]
        [RegularExpression("^(Low|Medium|High)$", ErrorMessage = "Invalid priority. Allowed values: Low, Medium, High.")]
        public string Priority { get; set; } = "Medium"; // Default value

        [Required]
        public int AssignedToUserId { get; set; }

        [Required]
        public int CreatedByUserId { get; set; }

        public DateTime? DueDate { get; set; } = null; // Optional field
        public string Description { get; set; } = "";
    }
}
