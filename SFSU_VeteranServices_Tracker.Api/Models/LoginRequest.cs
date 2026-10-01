/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: LoginRequest.cs
 * 
 * Description:
 * Represents the login information sent to the authentication API.
 * Stores the username and password entered by a Staff or Manager user
 * so the AuthController can verify the credentials against the database.
 * 
 * ***************************************************************************/

namespace SFSU_VeteranServices_Tracker.Api.Models
{
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
