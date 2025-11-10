using System.ComponentModel.DataAnnotations;

namespace TaskManagementApp.Domain.DTOs
{
    public class UpdateTaskRequest
    {
        [Required]
        [StringLength(200)]
        public string TaskName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? TaskDescription { get; set; }

        public List<long>? AssignedUserIDs { get; set; }

        [Required]
        public TaskCurrentStatus Status { get; set; }

        public DateTime? Deadline { get; set; }
    }
}
