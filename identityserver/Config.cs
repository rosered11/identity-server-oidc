using System;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace identity_server_oidc
{
    public class Config
    {
        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // Machine to Machine
                new Client{
                    ClientId = "defaultClient",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "defaultApi" }
                },
                // OpenId for authen user, and Oauth2 authen protect resource
                new Client{
                    ClientId = "default2Client",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RequirePkce = false,
                    AllowRememberConsent = false,
                    RedirectUris = new List<string>(){
                        "https://localhost:5003/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>(){
                        "https://localhost:5003/signout-callback-oidc"
                    },
                    ClientSecrets = new List<Secret>{
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = new List<string>{
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Email,
                        "defaultApi",
                        "roles"
                    }
                }
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                // protech client api
                new ApiScope("defaultApi", "Default API")
            };
        
        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {

            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResources.Email(),
                new IdentityResource(
                    "roles",
                    "Your role(s)",
                    new List<string>() { "role" }
                )
            };
        
        public static List<TestUser> TestUsers =>
            new List<TestUser>
            {
                new TestUser{
                    SubjectId = Guid.NewGuid().ToString(),
                    Username = "ice",
                    Password = "ice",
                    Claims = new List<Claim>{
                        new Claim(JwtClaimTypes.Name, "ice it"),
                        new Claim(JwtClaimTypes.GivenName, "ice"),
                        new Claim(JwtClaimTypes.FamilyName, "it"),
                        new Claim(JwtClaimTypes.Email, "ice@ice.ice"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://ice.ice"),
                        new Claim(JwtClaimTypes.Address, "my address"),
                        new Claim(JwtClaimTypes.Role, "user")
                    }
                },
                new TestUser{
                    SubjectId = Guid.NewGuid().ToString(),
                    Username = "admin",
                    Password = "admin",
                    Claims = new List<Claim>{
                        new Claim(JwtClaimTypes.Name, "admin it"),
                        new Claim(JwtClaimTypes.GivenName, "admin"),
                        new Claim(JwtClaimTypes.FamilyName, "it"),
                        new Claim(JwtClaimTypes.Email, "admin@admin.admin"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://admin.admin"),
                        new Claim(JwtClaimTypes.Address, "my address"),
                        new Claim(JwtClaimTypes.Role, "admin")
                    }
                }
            };
    }
}