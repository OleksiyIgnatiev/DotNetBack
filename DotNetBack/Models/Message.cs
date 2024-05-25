using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DotNetBack.Models
{
    public class Message
    {
        private Message(int messageId, string message, int? userId, int? adminId, bool isShown)
        {
            MessageId = messageId;
            Text = message;
            UserId = userId;
            AdminId = adminId;
            IsShown = isShown;
        }

        public Message() { }

        [JsonIgnore]
        public int MessageId { get; set; }

        [JsonPropertyName("message")]
        [Required]
        public string Text { get; set; }

        [JsonPropertyName("user_id")]
        [Required]
        public int? UserId { get; set; }

        [JsonPropertyName("admin_id")]
        [Required]
        public int? AdminId { get; set; }

        [JsonIgnore]
        public bool IsShown { get; set; }

        public static Message Create(int messageId, string message, int? userId, int? adminId, bool isShown)
        {
            return new Message(messageId, message, userId, adminId, isShown);
        }
    }
}
