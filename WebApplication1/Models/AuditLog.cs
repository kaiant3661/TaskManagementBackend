using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }

        [Required(ErrorMessage = "Action is required.")]
        [StringLength(500, ErrorMessage = "Action cannot be longer than 500 characters.")]
        public string Action { get; set; }

        public int PerformedByUserId { get; set; }

        [Required]
        public Userr PerformedByUser { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }
    }
}
