/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: LoginResponse.cs
 * 
 * Description: Represents a response to a login request with authentication
 * information.
 * 
 * ***************************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

namespace SFSU_VeteranServices_Tracker.Model
{
    public class LoginResponse
    {
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
