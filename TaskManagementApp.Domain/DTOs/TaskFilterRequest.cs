namespace TaskManagementApp.Domain.DTOs
{
    public class TaskFilterRequest
    {
        public TaskCurrentStatus? Status { get; set; }
        public long? AssignedUserID { get; set; }
        public bool? IsDelayed { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } // "name", "deadline", "status", "created"
        public bool SortDescending { get; set; } = false;
    }
}