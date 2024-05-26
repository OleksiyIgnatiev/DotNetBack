using System;
using System.Threading.Tasks;
using DotNetBack.Repositories;
using Microsoft.AspNetCore.Mvc;
using DotNetBack.Models.DTOs;

namespace DotNetBack.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class usersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public usersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (await _userRepository.LoginAsync(request.Username, request.Password))
            {
                // Generate tokens
                return Ok(new
                {
                    access_token = "your_access_token",
                    refresh_token = "your_refresh_token",
                    take_the_test = true // or whatever logic you need
                });
            }
            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            await _userRepository.RegisterAsync(request.Username, request.Email, request.Password);
            return Ok();
        }

        [HttpPut("notification")]
        public async Task<IActionResult> UpdateNotification([FromBody] UpdateNotificationRequest request)
        {
            await _userRepository.UpdateNotificationAsync(request.UserId, request.NotificationType, request.NotificationTime);
            return Ok();
        }

        [HttpPut("change")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            await _userRepository.UpdateUserAsync(request.UserId, request.Username, request.Email, request.Password);
            return Ok();
        }

        [HttpPut("subscription")]
        public async Task<IActionResult> UpdateSubscription([FromBody] UpdateSubscriptionRequest request)
        {
            await _userRepository.UpdateSubscriptionAsync(request.UserId, request.Subscription, request.SubscriptionPeriod);
            return Ok();
        }

        //[HttpDelete("logout")]
        //public async Task<IActionResult> Logout(int userId)
        //{
        //    await _userRepository.LogoutAsync(userId);
        //    return Ok();
        //}

        [HttpGet("to-admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }

        //[HttpPut("to-admin")]
        //public async Task<IActionResult> AdminAction(int userId)
        //{
        //    var result = await _userRepository.AdminActionAsync(userId);
        //    return Ok(result);
        //}

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            await _userRepository.DeleteUserAsync(userId);
            return Ok();
        }

        //[HttpPut("share-successes")]
        //public async Task<IActionResult> ShareSuccesses(int userId)
        //{
        //    var successes = await _userRepository.ShareSuccessesAsync(userId);
        //    return Ok(successes);
        //}

        //[HttpGet("calendar/{userId}")]
        //public async Task<IActionResult> GetCalendar(int userId)
        //{
        //    var calendar = await _userRepository.GetCalendarAsync(userId);
        //    return Ok(calendar);
        //}

        //[HttpGet("record/{userId}")]
        //public async Task<IActionResult> GetRecord(int userId)
        //{
        //    var record = await _userRepository.GetRecordAsync(userId);
        //    return Ok(record);
        //}

        [HttpGet("info/{userId}")]
        public async Task<IActionResult> GetUserInfo(int userId)
        {
            var userInfo = await _userRepository.GetUserInfoAsync(userId);
            return Ok(userInfo);
        }
    }
}
