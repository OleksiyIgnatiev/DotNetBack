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

        public int MessageId { get; set; }

        public string Text { get; private set; }

        public int? UserId { get; private set; }

        public int? AdminId { get; private set; }

        public bool IsShown { get; private set; }

        public static Message Create(int messageId, string message, int? userId, int? adminId, bool isShown)
        {
            return new Message(messageId, message, userId, adminId, isShown);
        }
    }
}
