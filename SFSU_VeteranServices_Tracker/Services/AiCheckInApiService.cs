using SFSU_VeteranServices_Tracker.Models;
using System.Net.Http.Json;

namespace SFSU_VeteranServices_Tracker.Services
{
    /// <summary>
    /// Sends conversational check-in messages
    /// between the MAUI application and the API.
    /// </summary>
    public static class AiCheckInApiService
    {
        private static readonly HttpClient httpClient = new();

        // Local API address while developing on Windows.
        private const string AiCheckInUrl =
            "https://localhost:7014/api/AiCheckIn/message";

        public static async Task<AiCheckInResponse?> SendMessageAsync(
            AiCheckInRequest request)
        {
            HttpResponseMessage response =
                await httpClient.PostAsJsonAsync(
                    AiCheckInUrl,
                    request);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<AiCheckInResponse>();
        }
    }
}