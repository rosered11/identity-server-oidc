using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using client_mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using client_mvc.Client;
using System.Net.Http;
using IdentityModel.Client;
using System.Text.Json;

namespace client_mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApiClient _apiClient;

        public HomeController(ILogger<HomeController> logger, ApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Privacy()
        {
            await LogTokenAndClaims();
            var protectResource = await _apiClient.GetResourceFromProtectApi();
            Console.WriteLine($"Protect resource: {protectResource}");
            return View();
        }

        [Authorize(Roles = "user")]
        public async Task<IActionResult> GetUserInfo()
        {
            var client = new HttpClient();

            var discoveryDoc = await client.GetDiscoveryDocumentAsync("https://localhost:5001");

            if(discoveryDoc.IsError)
            {
                return BadRequest("Cann't get discovery.");
            }

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var userInfo = await client.GetUserInfoAsync(
                new UserInfoRequest{
                    Address = discoveryDoc.UserInfoEndpoint,
                    Token = accessToken
                }
            );

            if (userInfo.IsError){
                return BadRequest("Cann't get user info.");
            }

            var userInfoDic = new Dictionary<string, string>();

            foreach(var claim in userInfo.Claims)
            {
                userInfoDic.Add(claim.Type, claim.Value);
            }

            Console.WriteLine(JsonSerializer.Serialize(userInfoDic));
            return View();
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        public async Task LogTokenAndClaims()
        {
            var identityToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            Console.WriteLine($"Identity token: {identityToken}");

            foreach(var claim in User.Claims)
            {
                Console.WriteLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
