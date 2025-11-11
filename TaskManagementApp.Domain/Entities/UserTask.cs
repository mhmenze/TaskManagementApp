using System.ComponentModel.DataAnnotations;

namespace TaskManagementApp.Domain
{
    public class UserTask
    {
        [Required]
        public long TaskID { get; set; }

        [Required]
        [StringLength(200)]
        public string TaskName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? TaskDescription { get; set; }

        public List<long>? AssignedUserIDs { get; set; }

        [Required]
        public TaskCurrentStatus Status { get; set; } = TaskCurrentStatus.ToDo;

        public bool IsDelayed { get; set; }

        public DateTime? Deadline { get; set; }

        public DateTime CreatedOn { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }
    }
}
