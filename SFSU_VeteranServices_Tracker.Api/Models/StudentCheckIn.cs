namespace SFSU_VeteranServices_Tracker.Api.Models
{
    public class StudentCheckIn
    {
        // Primary key
        public int Id { get; set; }

        public string StudentId { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CheckInTime { get; set; }
    }
}
