namespace SFSU_VeteranServices_Tracker.Api.Models
{
    public class StaffUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;

        // We never store the real password, only the hashed version into the database.
        public string PasswordHash { get; set; } = string.Empty;
        // Either "Manager" or "Staff"
        public string Role { get; set; } = "Staff";
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
