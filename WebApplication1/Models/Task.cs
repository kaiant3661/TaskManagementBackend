namespace WebApplication1.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Task
    {
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Task name is required.")]
        [StringLength(200, ErrorMessage = "Task name cannot be longer than 200 characters.")]
        public string TaskName { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters.")]
        public string Description { get; set; } = "";
        [Required]
        [RegularExpression("^(Pending|In Progress|Completed)$", ErrorMessage = "Invalid status. Allowed values: Pending, In Progress, Completed.")]
        public string Status { get; set; } = "Pending"; // Default value

        [Required]
        [RegularExpression("^(Low|Medium|High)$", ErrorMessage = "Invalid priority. Allowed values: Low, Medium, High.")]
        public string Priority { get; set; } = "Medium"; // Default value

        public int AssignedToUserId { get; set; }

        [Required]
        public int CreatedByUserId { get; set; } // Must be provided by the user

        public DateTime? DueDate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public Userr AssignedToUser { get; set; }

        public Userr CreatedByUser { get; set; }
    }
}
