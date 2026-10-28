using System;
using System.Collections.Generic;
using System.Text;

namespace SFSU_VeteranServices_Tracker.Models
{
    public class CheckInConversationState
    {
        public string CurrentStep { get; set; } = "Start";

        public string? FullName { get; set; }

        public string? StudentId { get; set; }

        public string? Status { get; set; }

        public bool CodeOfConductReviewed { get; set; }

        public bool CodeOfConductAccepted { get; set; }
    }
}
