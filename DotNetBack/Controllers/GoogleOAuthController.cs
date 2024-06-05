using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DotNetBack.Helpers;
using DotNetBack.Services;
using System;
using System.Threading.Tasks;
using DotNetBack.Models;
using DotNetBack.Repositories;

namespace DotNetBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoogleOAuthController : ControllerBase
    {
        private const string RedirectUrl = "https://localhost:5001/GoogleOAuth/Code";
        private const string Scope = "https://www.googleapis.com/auth/cloud-platform";
        private const string PkceSessionKey = "codeVerifier";
        private readonly IUserRepository _userRepository;

        public GoogleOAuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet("RedirectOnOAuthServer")]
        public IActionResult RedirectOnOAuthServer()
        {
            var codeVerifier = Guid.NewGuid().ToString();

            HttpContext.Session.SetString(PkceSessionKey, codeVerifier);

            var codeChallenge = Sha256Helper.ComputeHash(codeVerifier);

            var url = GoogleOAuthService.GenerateOAuthRequestUrl(Scope, RedirectUrl, codeChallenge);
            return Redirect(url);
        }

        [HttpGet("Code")]
        public async Task<IActionResult> CodeAsync(string code)
        {
            string codeVerifier = HttpContext.Session.GetString(PkceSessionKey);

            var tokenResult = await GoogleOAuthService.ExchangeCodeOnTokenAsync(code, codeVerifier, RedirectUrl);

            var refreshedToken = await GoogleOAuthService.RefreshTokenAsync(tokenResult.RefreshToken);

            // Получаем электронную почту пользователя из токена результат
            string userEmail = tokenResult.Email;

            // Извлекаем пользователя из базы данных по email
            var response = await _userRepository.GetUserByEmailAsync(userEmail);

            if (response.StatusCode == 404)
            {
                return NotFound(response.Message);
            }
            else if (response.StatusCode == 500)
            {
                return StatusCode(500, response.Message);
            }

            return Ok(response.Data);
        }
    }
}
