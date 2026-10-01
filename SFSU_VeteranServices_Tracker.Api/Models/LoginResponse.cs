/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: LoginResponse.cs
 * 
 * Description:
 * Represents the information returned after a successful Staff or Manager
 * login. Stores the authenticated username, user role, and JWT token that
 * the MAUI application uses for protected API requests.
 * 
 * ***************************************************************************/

namespace SFSU_VeteranServices_Tracker.Api.Models
{
    public class LoginResponse
    {
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}

