/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: StaffUser.cs
 * 
 * Description:
 * Represents a Staff or Manager account stored in the database.
 * Stores account information such as the username, hashed password,
 * user role, account status, and account creation date.
 * 
 * ***************************************************************************/

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
