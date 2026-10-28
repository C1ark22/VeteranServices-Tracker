using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SFSU_VeteranServices_Tracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiTestController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AiTestController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> Test()
    {
        string projectEndpoint =
            _configuration["FOUNDRY_AGENT_ENDPOINT"]
            ?? throw new InvalidOperationException(
                "Foundry project endpoint is missing.");

        string model =
            _configuration["FOUNDRY_MODEL"]
            ?? "gpt-5-mini";

        DefaultAzureCredential credential = new();

        AccessToken token =
            await credential.GetTokenAsync(
                new TokenRequestContext(
                    new[]
                    {
                        "https://ai.azure.com/.default"
                    }));

        string url =
            projectEndpoint.TrimEnd('/') +
            "?api-version=v1";

        HttpClient client =
            _httpClientFactory.CreateClient();

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                token.Token);

        var requestBody = new
        {
            model = model,
            input =
                """
                You are a conversational assistant
                for a university Veterans Corner.

                Say hello and ask the student
                how you can help them today.
                """
        };

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
            return StatusCode(
                (int)response.StatusCode,
                responseBody);
        }

        return Content(
            responseBody,
            "application/json");
    }
}