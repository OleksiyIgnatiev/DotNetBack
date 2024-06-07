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
            if (response.StatusCode == 200)
            {
                return Ok(response);
            }
            return Unauthorized(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            Response response = await _userRepository.RegisterAsync(request.Username, request.Email, request.Password);
            if (response.StatusCode == 200)
            {
                return Ok(response);
            }
            return Unauthorized(response);
        }

        [HttpPost("reset")]
        public async Task<IActionResult> Reset([FromBody] ResetRequest request)
        {
            Response response = await _userRepository.ResetAsync(request.email);
            if (response.StatusCode == 200)
            {
                return Ok(response);
            }
            return Unauthorized(response);
        }

        [HttpPut("notification")]
        public async Task<IActionResult> UpdateNotification([FromBody] UpdateNotificationRequest request)
        {
            Response response = await _userRepository.UpdateNotificationAsync(request.UserId, request.NotificationType, request.NotificationTime);
            if (response.StatusCode == 200) 
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response.Message);
        }

        [HttpPut("change")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            Response response = await _userRepository.UpdateUserAsync(request.UserId, request.Username, request.Email, request.Password);
            if (response.StatusCode == 200)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response.Message);
        }

        [HttpPut("subscription")]
        public async Task<IActionResult> UpdateSubscription([FromBody] UpdateSubscriptionRequest request)
        {
            Response response = await _userRepository.UpdateSubscriptionAsync(request.UserId, request.Subscription, request.SubscriptionPeriod);
            if (response.StatusCode == 200)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response.Message);
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
            if (users.StatusCode == 200)
            {
                return Ok(users);
            }
            return StatusCode(users.StatusCode, users.Message);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            Response response = await _userRepository.DeleteUserAsync(userId);
            if (response.StatusCode == 200)
            {
                return Ok(response);
            }
            return StatusCode(response.StatusCode, response.Message);
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
            if (calendar.StatusCode == 200)
            {
                return Ok(calendar);
            }
            return StatusCode(calendar.StatusCode, calendar.Message);
        }

        [HttpGet("record/{userId}")]
        public async Task<IActionResult> GetRecord(int userId)
        {
            Response record = await _userRepository.GetRecordAsync(userId);
            if (record.StatusCode == 200)
            {
                return Ok(record);
            }
            return StatusCode(record.StatusCode, record.Message);
        }

        [HttpGet("info/{userId}")]
        public async Task<IActionResult> GetUserInfo(int userId)
        {
            Response userInfo = await _userRepository.GetUserInfoAsync(userId);
            if (userInfo.StatusCode == 200)
            {
                return Ok(userInfo);
            }
            return StatusCode(userInfo.StatusCode, userInfo.Message);
        }
    }
}
