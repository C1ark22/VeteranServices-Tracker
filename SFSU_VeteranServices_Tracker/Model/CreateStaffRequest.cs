/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: CreateStaffRequest.cs
 * 
 * Description: Represents a request to create a new staff member with 
 * authentication information.
 * 
 * ***************************************************************************/


using System;
using System.Collections.Generic;
using System.Text;

namespace SFSU_VeteranServices_Tracker.Model
{
    public class CreateStaffRequest
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
