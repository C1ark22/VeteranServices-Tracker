/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: StudentCheckIn.cs
 * 
 * Description: Represents a student check-in record with information about the
 * student and their check-in status.
 * 
 * ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace SFSU_VeteranServices_Tracker.Model
{
    public class StudentCheckIn
    {
        public string FullName { get; set; }
        public string StudentId { get; set; }
        public string Status { get; set; }
        public DateTime CheckInTime { get; set; }
    }
}
