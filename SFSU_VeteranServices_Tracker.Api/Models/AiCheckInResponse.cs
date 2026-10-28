namespace SFSU_VeteranServices_Tracker.Api.Models
{
    /// <summary>
    /// Represents the AI assistant's response
    /// during a student check-in conversation.
    /// </summary>
    public class AiCheckInResponse
    {
        public string Message { get; set; } = string.Empty;

        // The client sends this value back with
        // the student's next message.
        public string? ResponseId { get; set; }

        public CheckInConversationState State { get; set; } = new();
    }
}