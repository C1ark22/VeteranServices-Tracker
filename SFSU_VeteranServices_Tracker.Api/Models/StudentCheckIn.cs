/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: StudentCheckIn.cs
 * 
 * Description:
 * Represents a student check-in record stored in the database.
 * Stores the student's ID, full name, status, check-in date and time,
 * and the database ID used to uniquely identify each record.
 * 
 * ***************************************************************************/

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
