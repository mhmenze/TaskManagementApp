namespace TaskManagementApp.Domain.DTOs
{
    public class UpdateUserRequest
    {
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? DisplayName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }  // optional
        public string? UserRole { get; set; }  // optional
        public string? Email { get; set; }
    }

}
