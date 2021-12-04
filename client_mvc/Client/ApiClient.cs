using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace client_mvc.Client
{
    public class ApiClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApiClient(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<string> GetResourceFromProtectApi()
        {
            // var apiCredential = new ClientCredentialsTokenRequest{
            //     Address = "https://localhost:5001/connect/token",
            //     ClientId = "defaultClient",
            //     ClientSecret = "secret",
            //     Scope = "defaultApi"
            // };

            // var client = new HttpClient();

            // var discoveryDoc = await client.GetDiscoveryDocumentAsync("https://localhost:5001");

            // if(discoveryDoc.IsError)
            // {
            //     return "Cann't get discovery.";
            // }

            // var token = await client.RequestClientCredentialsTokenAsync(apiCredential);
            // if(token.IsError)
            // {
            //     return "Unauthorize.";
            // }

            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            var apiClient = new HttpClient();

            //apiClient.SetBearerToken(token.AccessToken);

            if(string.IsNullOrEmpty(accessToken)){
                return "Access token not found.";
            }
            apiClient.SetBearerToken(accessToken);

            //Console.WriteLine($"apiToken: {token.AccessToken}");

            var response = await apiClient.GetAsync("http://localhost:5002/WeatherForecast");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return content;
        }
    }
}