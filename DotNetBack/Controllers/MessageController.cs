using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DotNetBack.Models;
using DotNetBack.Repositories;

namespace DotNetBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;

        public MessageController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] Message message)
        {
            if (message == null)
            {
                return BadRequest("Message is null.");
            }

            await _messageRepository.CreateMessageAsync(message);
            return Ok();
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUnreadMessages(int userId)
        {
            var messages = await _messageRepository.GetUnreadMessagesAsync(userId);
            return Ok(messages);
        }
    }
}
