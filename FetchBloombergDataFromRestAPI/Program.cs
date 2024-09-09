using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;

class Program
{
    private static readonly HttpClient client = new();

    public class TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string RefreshToken { get; set; }
    }

    static async Task Main(string[] args)
    {
        string tokenUrl = "https://bsso.blpprofessional.com/ext/api/as/token.oauth2";
        string clientId = "ab602060ef550cce364a98b1af9c6c20";
        string clientSecret = "15ad67285907e3b64e24ab4c78c6052848aeca85b04a7ce4ccef2fe8e3261fe3";
        try
        {
            TokenResponse tokenResponse = await GetAccessToken(tokenUrl, clientId, clientSecret);
            Console.WriteLine("Result:");
            Console.WriteLine(tokenResponse.access_token);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }

    private static async Task<TokenResponse> GetAccessToken(string tokenUrl, string clientId, string clientSecret)
    {
        HttpRequestMessage request = new(HttpMethod.Post, tokenUrl);
        Dictionary<string, string> requestBody = new()
        {
            { "grant_type", "client_credentials" },
            { "client_id", clientId },
            { "client_secret", clientSecret }
        };

        request.Content = new FormUrlEncodedContent(requestBody);
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();
        TokenResponse tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseBody);

        if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.access_token))
            throw new Exception("Failed to retrieve a valid access token.");
        return tokenResponse;
    }
}
