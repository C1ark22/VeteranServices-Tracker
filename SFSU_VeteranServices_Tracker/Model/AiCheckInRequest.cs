using System;
using System.Collections.Generic;
using System.Text;

namespace SFSU_VeteranServices_Tracker.Models
{
    public class AiCheckInRequest
    {
        public string Message { get; set; } = string.Empty;

        public string? PreviousResponseId { get; set; }

        public CheckInConversationState State { get; set; } = new();
    }
}
