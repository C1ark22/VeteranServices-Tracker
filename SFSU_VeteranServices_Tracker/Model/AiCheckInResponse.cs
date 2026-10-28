using System;
using System.Collections.Generic;
using System.Text;

namespace SFSU_VeteranServices_Tracker.Models
{
    public class AiCheckInResponse
    {
        public string Message { get; set; } = string.Empty;

        public string? ResponseId { get; set; }

        public CheckInConversationState State { get; set; } = new();
    }
}
