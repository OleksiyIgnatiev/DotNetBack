using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DotNetBack.Helpers;
using DotNetBack.Services;
using System;
using System.Threading.Tasks;
using DotNetBack.Models;

namespace DotNetBack.Controllers
{
    public class GoogleOAuthController : Controller
    {
        private const string RedirectUrl = "https://localhost:5001/GoogleOAuth/Code";
        private const string Scope = "https://www.googleapis.com/auth/cloud-platform";
        private const string PkceSessionKey = "codeVerifier";

        public IActionResult RedirectOnOAuthServer()
        {
            var codeVerifier = Guid.NewGuid().ToString();

            HttpContext.Session.SetString(PkceSessionKey, codeVerifier);

            var codeChallenge = Sha256Helper.ComputeHash(codeVerifier);

            var url = GoogleOAuthService.GenerateOAuthRequestUrl(Scope, RedirectUrl, codeChallenge);
            return Redirect(url);
        }
        public async Task<IActionResult> CodeAsync(string code)
        {
            string codeVerifier = HttpContext.Session.GetString("codeVerifier");

            var tokenResult = await GoogleOAuthService.ExchangeCodeOnTokenAsync(code, codeVerifier, RedirectUrl);

            var refreshedToken = await GoogleOAuthService.RefreshTokenAsync(tokenResult.RefreshToken);

            // Получаем электронную почту пользователя из токена результат
            string userEmail = tokenResult.Email;

            return Ok();
        }
    }
}
