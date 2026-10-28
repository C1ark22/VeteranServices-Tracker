namespace SFSU_VeteranServices_Tracker.Api.Models
{
    /// <summary>
    /// Represents a message sent by the student
    /// during an AI check-in conversation.
    /// </summary>
    public class AiCheckInRequest
    {
        public string Message { get; set; } = string.Empty;

        // Used to continue an existing AI conversation.
        // This will be null for the first message.
        public string? PreviousResponseId { get; set; }

        // Keeps track of what information
        // has already been collected.
        public CheckInConversationState State { get; set; } = new();
    }
}