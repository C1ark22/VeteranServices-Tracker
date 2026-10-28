using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using SFSU_VeteranServices_Tracker.Api.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SFSU_VeteranServices_Tracker.Api.Data;
using SFSU_VeteranServices_Tracker.Api.Models;

namespace SFSU_VeteranServices_Tracker.Api.Controllers
{
    /// <summary>
    /// Handles the conversational AI check-in experience
    /// for students using the Veterans Corner application.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AiCheckInController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        private readonly VeteranServicesContext _context;

        public AiCheckInController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration, 
            VeteranServicesContext context)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("message")]
        public async Task<ActionResult<AiCheckInResponse>> SendMessage(
            AiCheckInRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("A message is required.");
            }

            CheckInConversationState state = request.State;

            string instruction;

            switch (state.CurrentStep)
            {
                case "Start":
                    if (IsCheckInIntent(request.Message))
                    {
                        state.CurrentStep = "AwaitingFullName";

                        instruction =
                            """
                            The student wants to check in.

                            Welcome them and ask only for their full name.
                            Do not ask for anything else yet.
                            """;
                    }
                    else
                    {
                        instruction =
                            """
                            Greet the student and ask how you can help them.

                            If appropriate, let them know that you can
                            help them check in to the Veterans Corner.
                            """;
                    }

                    break;

                case "AwaitingFullName":

                    string fullName = request.Message.Trim();

                    if (fullName.Length < 2)
                    {
                        instruction =
                            """
                            The student did not provide a valid full name.

                            Politely ask them to enter their full name again.
                            """;

                        break;
                    }

                    state.FullName = fullName;
                    state.CurrentStep = "AwaitingStudentId";

                    instruction =
                        $"""
                        The student's name is {state.FullName}.

                        Thank them and ask only for their Student ID.
                        """;

                    break;

                case "AwaitingStudentId":

                    string studentId =
                        new string(
                            request.Message
                                .Where(char.IsDigit)
                                .ToArray());

                    if (string.IsNullOrWhiteSpace(studentId))
                    {
                        instruction =
                            """
                            The student did not provide a valid Student ID.

                            Politely ask them to enter their Student ID again.
                            The Student ID should contain numbers.
                            """;

                        break;
                    }

                    state.StudentId = studentId;
                    state.CurrentStep = "AwaitingStatus";

                    instruction =
                        """
                        Thank the student.

                        Ask which status applies to them.

                        The available choices are:

                        Veteran
                        Active Duty
                        Reserve
                        Dependent
                        Civilian

                        Ask only for their status.
                        """;

                    break;

                case "AwaitingStatus":

                    string? status = NormalizeStatus(request.Message);

                    if (status == null)
                    {
                        instruction =
                            """
                            The student's status was not recognized.

                            Politely ask them to choose one of these:

                            Veteran
                            Active Duty
                            Reserve
                            Dependent
                            Civilian
                            """;

                        break;
                    }

                    state.Status = status;
                    state.CurrentStep = "AwaitingCodeOfConductReview";

                    instruction =
                        $"""
                        The student's basic check-in information has been collected.

                        Name: {state.FullName}
                        Student ID: {state.StudentId}
                        Status: {state.Status}

                        Briefly confirm the information.

                        Tell the student that before continuing,
                        they must open and review the official
                        SFSU Standards for Student Conduct.

                        The application will provide a button
                        that opens the official SFSU website.

                        Do not ask them to agree yet.
                        """;

                    break;

                case "AwaitingCodeOfConductReview":

                    instruction =
                        """
                        Before continuing, please use the
                        Review SFSU Code of Conduct button
                        to open the official SFSU Standards
                        for Student Conduct.

                        After reviewing the page, return here
                        and follow the instructions to continue.
                        """;

                    break;

                case "AwaitingCodeOfConduct":

                    if (!state.CodeOfConductReviewed)
                    {
                        state.CurrentStep =
                            "AwaitingCodeOfConductReview";

                        instruction =
                            """
                            The student has not opened the official
                            SFSU Code of Conduct page yet.

                            Tell them to use the Review Code of Conduct
                            button before continuing.
                            """;

                        break;
                    }

                    if (IsCodeOfConductAccepted(request.Message))
                    {
                        state.CodeOfConductAccepted = true;

                        state.CurrentStep =
                            "AwaitingSubmissionConfirmation";

                        instruction =
                            """
                            Thank the student for acknowledging
                            the Code of Conduct.

                            Tell them their check-in information
                            is ready to be submitted.

                            Ask if they would like to submit
                            their check-in now.

                            Do not tell them they are checked in yet.
                            """;
                    }
                    else
                    {
                        instruction =
                            """
                            Ask the student to type "I agree"
                            if they agree to follow the SFSU
                            Standards for Student Conduct.
                            """;
                    }

                    break;

                case "AwaitingSubmissionConfirmation":

                    if (!IsSubmissionConfirmed(request.Message))
                    {
                        instruction =
                            """
                            The student's check-in has not been submitted yet.

                            Ask them if they would like to submit
                            their check-in now.

                            They can say "yes" when they are ready.
                            """;

                        break;
                    }

                    // Validate everything again before saving.
                    if (string.IsNullOrWhiteSpace(state.FullName) ||
                        string.IsNullOrWhiteSpace(state.StudentId) ||
                        string.IsNullOrWhiteSpace(state.Status) ||
                        !state.CodeOfConductAccepted)
                    {
                        return BadRequest(
                            "The check-in information is incomplete.");
                    }

                    StudentCheckIn student = new StudentCheckIn
                    {
                        FullName = state.FullName,
                        StudentId = state.StudentId,
                        Status = state.Status,
                        CheckInTime = DateTime.Now
                    };

                    try
                    {
                        _context.StudentCheckIns.Add(student);

                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        return StatusCode(
                            StatusCodes.Status500InternalServerError,
                            "The check-in could not be saved. Please try again.");
                    }

                    // Only mark it complete AFTER the database save succeeds.
                    state.CurrentStep = "Completed";

                    return Ok(
                        new AiCheckInResponse
                        {
                            Message =
                                "Your check-in has been successfully completed. " +
                                "Thank you for visiting the Veterans Corner.",

                            ResponseId = request.PreviousResponseId,

                            State = state
                        });

                case "Completed":

                    return Ok(
                        new AiCheckInResponse
                        {
                            Message =
                                "Your check-in has already been completed. " +
                                "Thank you for visiting the Veterans Corner.",

                            ResponseId = request.PreviousResponseId,

                            State = state
                        });

                default:

                    state.CurrentStep = "Start";

                    instruction =
                        """
                        Ask the student how you can help them today.
                        """;

                    break;
            }

            (string assistantMessage, string? responseId) =
                await SendToAgentAsync(
                    instruction,
                    request.PreviousResponseId);

            AiCheckInResponse result =
                new AiCheckInResponse
                {
                    Message = assistantMessage,
                    ResponseId = responseId,
                    State = state
                };

            return Ok(result);
        }
        private async Task<(string Message, string? ResponseId)>
            SendToAgentAsync(string instruction,string? previousResponseId)
        {
            string agentEndpoint =
                _configuration["FOUNDRY_AGENT_ENDPOINT"]
                ?? throw new InvalidOperationException(
                    "Foundry agent endpoint is missing.");

            DefaultAzureCredential credential = new();

            AccessToken token =
                await credential.GetTokenAsync(
                    new TokenRequestContext(
                        new[]
                        {
                    "https://ai.azure.com/.default"
                        }));

            string url =
                agentEndpoint.TrimEnd('/') +
                "?api-version=v1";

            HttpClient client =
                _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token.Token);

            Dictionary<string, object> requestBody = new()
            {
                ["input"] = instruction,
                ["stream"] = false
            };

            if (!string.IsNullOrWhiteSpace(previousResponseId))
            {
                requestBody["previous_response_id"] =
                    previousResponseId;
            }

            string json =
                JsonSerializer.Serialize(requestBody);

            HttpContent content =
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

            HttpResponseMessage response =
                await client.PostAsync(url, content);

            string responseBody =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"Foundry returned {(int)response.StatusCode}: " +
                    responseBody);
            }

            using JsonDocument document =
                JsonDocument.Parse(responseBody);

            JsonElement root =
                document.RootElement;

            string? responseId = null;
            string? assistantMessage = null;

            if (root.TryGetProperty(
                    "id",
                    out JsonElement idElement))
            {
                responseId = idElement.GetString();
            }

            if (root.TryGetProperty(
                    "output",
                    out JsonElement outputElement))
            {
                foreach (JsonElement outputItem
                         in outputElement.EnumerateArray())
                {
                    if (responseId == null &&
                        outputItem.TryGetProperty(
                            "response_id",
                            out JsonElement responseIdElement))
                    {
                        responseId =
                            responseIdElement.GetString();
                    }

                    if (!outputItem.TryGetProperty(
                            "content",
                            out JsonElement contentElement))
                    {
                        continue;
                    }

                    foreach (JsonElement contentItem
                             in contentElement.EnumerateArray())
                    {
                        if (contentItem.TryGetProperty(
                                "type",
                                out JsonElement typeElement) &&
                            typeElement.GetString() == "output_text" &&
                            contentItem.TryGetProperty(
                                "text",
                                out JsonElement textElement))
                        {
                            assistantMessage =
                                textElement.GetString();

                            break;
                        }
                    }

                    if (assistantMessage != null)
                    {
                        break;
                    }
                }
            }

            return (
                assistantMessage ?? "I couldn't generate a response.",
                responseId);
        }
        private static bool IsCheckInIntent(string message)
        {
            return message.Contains(
                       "check in",
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   message.Contains(
                       "check-in",
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   message.Contains(
                       "sign in",
                       StringComparison.OrdinalIgnoreCase);
        }
        private static string? NormalizeStatus(string message)
        {
            string value =
                message.Trim().ToLowerInvariant();

            if (value.Contains("active duty"))
            {
                return "Active Duty";
            }

            if (value.Contains("reserve"))
            {
                return "Reserve";
            }

            if (value.Contains("dependent"))
            {
                return "Dependent";
            }

            if (value.Contains("civilian"))
            {
                return "Civilian";
            }

            if (value.Contains("veteran"))
            {
                return "Veteran";
            }

            return null;
        }
        private static bool IsCodeOfConductAccepted(string message)
        {
            string value =
                message.Trim().ToLowerInvariant();

            return value == "i agree"
                || value == "agree"
                || value == "yes, i agree"
                || value == "yes i agree";
        }
        private static bool IsSubmissionConfirmed(string message)
        {
            string value =
                message.Trim().ToLowerInvariant();

            return value == "yes"
                || value == "yes please"
                || value == "submit"
                || value == "submit it"
                || value == "yes submit it"
                || value == "yes, submit it";
        }
    }
}