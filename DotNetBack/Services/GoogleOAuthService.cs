using Microsoft.AspNetCore.WebUtilities;
using DotNetBack.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetBack.Services
{
    public class GoogleOAuthService
    {
        private const string ClientId = "845923484613-57s707glu5bu505ugqtgmkfb363aphft.apps.googleusercontent.com";
        private const string ClientSecret = "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";

        private const string OAuthServerEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string TokenServerEndpoint = "https://oauth2.googleapis.com/token";

        public static string GenerateOAuthRequestUrl(string scope, string redirectUrl, string codeChallenge)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "client_id", ClientId },
                { "redirect_url", redirectUrl },
                { "response_type", "code" },
                { "scope", scope },
                { "code_challenge", codeChallenge },
                { "code_challenge_method", "S256" },
                { "acces_type", "offline" }
            };
            
            var url = QueryHelpers.AddQueryString(OAuthServerEndpoint, queryParams);
            return url;
        }

        public static async Task<TokenResult> ExchangeCodeOnTokenAsync(string code, string codeVerifier, string redirectUrl)
        {
            var authParams = new Dictionary<string, string>
            {
                { "client_id", ClientId },
                { "client_secret", ClientSecret },
                { "code", code },
                { "code_verifier", codeVerifier },
                { "grant_type", "authorization_code" },
                { "redirect_url", redirectUrl },
                { "scope", "openid email" }
            };

            var tokenResult = await HttpClientHelper.SendPostRequest<TokenResult>(OAuthServerEndpoint, authParams);
            return tokenResult;
        }

        public static async Task<TokenResult> RefreshTokenAsync(string refreshToken)
        {
            var refreshParams = new Dictionary<string, string>
            {
                { "client_id", ClientId },
                { "client_secret", ClientSecret },
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };

            var tokenResult = await HttpClientHelper.SendPostRequest<TokenResult>(TokenServerEndpoint, refreshParams);

            return tokenResult;
        }
    }
}
