/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: LoginRequest.cs
 * 
 * Description: Represents a request to log in a user with authentication 
 * information.
 * 
 * ***************************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

namespace SFSU_VeteranServices_Tracker.Model
{
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
