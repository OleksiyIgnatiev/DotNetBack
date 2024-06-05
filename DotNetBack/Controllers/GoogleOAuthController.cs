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
        private const string RedirectUrl = "http://localhost:5264/api/GoogleOAuth/Code";
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

            var url = GoogleOAuthService.GenerateOAuthRequestUrl("openid email", RedirectUrl, codeChallenge);

            return Redirect(url);
        }

        [HttpGet("Code")]
        public async Task<IActionResult> CodeAsync(string code)
        {
            string codeVerifier = HttpContext.Session.GetString(PkceSessionKey);

            var tokenResult = await GoogleOAuthService.ExchangeCodeOnTokenAsync(code, codeVerifier, RedirectUrl);

            // Получаем адрес электронной почты пользователя из токена результат
            string userEmail = tokenResult.Email;

            // Если адрес электронной почты не возвращается из токена, попробуйте получить его отдельным запросом к Google API
            if (string.IsNullOrEmpty(userEmail))
            {
                userEmail = await GoogleApiService.GetUserEmailAsync(tokenResult.AccessToken);
            }

            // Извлекаем пользователя из базы данных по email
            var response = await _userRepository.GetUserByEmailAsync(userEmail);

            if (response.StatusCode == 500)
            {
                return StatusCode(500, response.Message);
            }

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