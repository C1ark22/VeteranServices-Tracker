/******************************************************************************
 * Project: SFSU Veteran Services Tracker
 * Author: Clark Batungbakal 
 * Class Name: ApiService.cs
 * 
 * Description: ApiService handles all communication between the MAUI app
 * and the backend API. It sends HTTP requests, receives JSON
 * responses, and manages actions such as student check-ins,
 * staff login, viewing check-in records, and creating staff accounts.
 * 
 * ***************************************************************************/


using SFSU_VeteranServices_Tracker.Model;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Maui.Storage;

namespace SFSU_VeteranServices_Tracker.Services
{
    public class ApiService
    {
        private readonly HttpClient httpClient;

        public ApiService()
        {
            httpClient = new HttpClient();

            // Set the base address for the API
            httpClient.BaseAddress =
                new Uri("https://veteranservices-api-clark-czducbc0gyfwffe9.westus3-01.azurewebsites.net/");
        }

        public async Task<bool> CreateCheckInAsync(StudentCheckIn student)
        {
            // Send a POST request to the API to create a new check-in
            HttpResponseMessage response =
                await httpClient.PostAsJsonAsync("api/CheckIns",student);

            if (!response.IsSuccessStatusCode)
            {
                string errorMessage =
                    await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine(
                    $"Check-In Error: {(int)response.StatusCode} " +
                    $"{response.StatusCode}");

                System.Diagnostics.Debug.WriteLine(
                    errorMessage);
            }

            // If the API returns null, return an empty list instead
            return response.IsSuccessStatusCode;
        }

        // Get all check-ins from the API
        public async Task<List<StudentCheckIn>> GetCheckInsAsync()
        {
            string? token =
                await SecureStorage.Default.GetAsync("auth_token");

            if (string.IsNullOrWhiteSpace(token))
            {
                return new List<StudentCheckIn>();
            }

            HttpRequestMessage request =
                new HttpRequestMessage(HttpMethod.Get,"api/CheckIns");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",token);

            HttpResponseMessage response =
                await httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Unable to retrieve check-ins.");
            }

            List<StudentCheckIn>? checkIns =
                await response.Content.ReadFromJsonAsync<List<StudentCheckIn>>();

            return checkIns ?? new List<StudentCheckIn>();
        }
        public async Task<LoginResponse?> LoginAsync(string username, string password)
        {
            LoginRequest loginRequest = new LoginRequest
            {
                Username = username,
                Password = password
            };

            HttpResponseMessage response =
                await httpClient.PostAsJsonAsync("api/Auth/login",loginRequest);

            // Incorrect username or password.
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return null;
            }

            // This catches other errors such as 500.
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Unable to connect to the login service.");
            }

            LoginResponse? loginResponse =
                await response.Content.ReadFromJsonAsync<LoginResponse>();

            return loginResponse;
        }
        public async Task<bool> CreateStaffAsync(string username, string password)
        {
            // Get the Manager's JWT token from SecureStorage.
            string? token = await SecureStorage.Default.GetAsync("auth_token");

            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            CreateStaffRequest staffRequest = new CreateStaffRequest
                {
                    Username = username,
                    Password = password
                };

            // Create the POST request.
            HttpRequestMessage request = 
                new HttpRequestMessage( HttpMethod.Post,"api/Staff");

            request.Content = JsonContent.Create(staffRequest);

            // Send the Manager's JWT token with the request.
            request.Headers.Authorization = 
                new AuthenticationHeaderValue("Bearer",token);

            HttpResponseMessage response = await httpClient.SendAsync(request);

            return response.IsSuccessStatusCode;
        }
    }
}