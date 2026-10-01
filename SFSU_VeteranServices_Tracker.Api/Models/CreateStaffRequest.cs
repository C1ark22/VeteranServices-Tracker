/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: CreateStaffRequest.cs
 * 
 * Description:
 * Represents the information required to create a new Staff account.
 * Stores the username and password entered by a Manager before the data
 * is sent to the StaffController for validation, password hashing,
 * and database storage.
 * 
 * ***************************************************************************/

namespace SFSU_VeteranServices_Tracker.Api.Models
{
    public class CreateStaffRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
