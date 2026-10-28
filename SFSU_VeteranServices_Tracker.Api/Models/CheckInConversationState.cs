namespace SFSU_VeteranServices_Tracker.Api.Models
{
    /// <summary>
    /// Stores the information collected during
    /// an AI student check-in conversation.
    /// </summary>
    public class CheckInConversationState
    {
        // Tells the application what information
        // it is currently waiting for.
        public string CurrentStep { get; set; } = "Start";

        public string? FullName { get; set; }

        public string? StudentId { get; set; }

        public string? Status { get; set; }

        public bool CodeOfConductReviewed { get; set; }

        public bool CodeOfConductAccepted { get; set; }
    }
}