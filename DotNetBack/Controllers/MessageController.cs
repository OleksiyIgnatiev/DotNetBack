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

        // POST: api/message
        /// <summary>
        /// Creates a new message for a user.
        /// </summary>
        /// <param name="message">The message to create.</param>
        /// <returns>A status indicating whether the creation was successful.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] Message message)
        {
            if (message == null)
            {
                return BadRequest("Message is null.");
            }

            await _messageRepository.AddMessageAsync(message);
            return Ok(new { Status = "Message created successfully" });
        }

        // GET: api/message/{userId}
        /// <summary>
        /// Gets unread messages for a user.
        /// </summary>
        /// <param name="userId">The ID of the user to get messages for.</param>
        /// <returns>A list of unread messages.</returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUnreadMessages(int userId)
        {
            var messages = await _messageRepository.GetUnreadMessagesAsync(userId);
            if (messages == null || messages.Count == 0)
            {
                return NotFound("No unread messages found.");
            }

            return Ok(messages);
        }
    }
}
