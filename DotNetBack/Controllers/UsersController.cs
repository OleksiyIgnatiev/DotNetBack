using System;
using System.Threading.Tasks;
using DotNetBack.Repositories;
using Microsoft.AspNetCore.Mvc;
using DotNetBack.Models.DTOs;
using DotNetBack.Models;

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
            Response response = await _userRepository.LoginAsync(request.Username, request.Password);
            if (Response.StatusCode == 200)
            {
                return Ok(response);
            }
            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            Response response = await _userRepository.RegisterAsync(request.Username, request.Email, request.Password);
            return Ok(response);
        }

        [HttpPut("notification")]
        public async Task<IActionResult> UpdateNotification([FromBody] UpdateNotificationRequest request)
        {
            Response response = await _userRepository.UpdateNotificationAsync(request.UserId, request.NotificationType, request.NotificationTime);
            return Ok(response);
        }

        [HttpPut("change")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            Response response = await _userRepository.UpdateUserAsync(request.UserId, request.Username, request.Email, request.Password);
            return Ok(response);
        }

        [HttpPut("subscription")]
        public async Task<IActionResult> UpdateSubscription([FromBody] UpdateSubscriptionRequest request)
        {
            Response response = await _userRepository.UpdateSubscriptionAsync(request.UserId, request.Subscription, request.SubscriptionPeriod);
            return Ok(response);
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
            Response users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            Response response = await _userRepository.DeleteUserAsync(userId);
            return Ok(response);
        }

        //[HttpPut("share-successes")]
        //public async Task<IActionResult> ShareSuccesses(int userId)
        //{
        //    var successes = await _userRepository.ShareSuccessesAsync(userId);
        //    return Ok(successes);
        //}

        [HttpGet("calendar/{userId}")]
        public async Task<IActionResult> GetCalendar(int userId)
        {
            Response calendar = await _userRepository.GetCalendarAsync(userId);
            return Ok(calendar);
        }

        [HttpGet("record/{userId}")]
        public async Task<IActionResult> GetRecord(int userId)
        {
            Response record = await _userRepository.GetRecordAsync(userId);
            return Ok(record);
        }

        [HttpGet("info/{userId}")]
        public async Task<IActionResult> GetUserInfo(int userId)
        {
            Response userInfo = await _userRepository.GetUserInfoAsync(userId);
            return Ok(userInfo);
        }
    }
}
