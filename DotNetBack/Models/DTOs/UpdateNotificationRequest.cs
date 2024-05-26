namespace DotNetBack.Models.DTOs
{
    public class UpdateNotificationRequest
    {
        public int UserId { get; set; }
        public string NotificationType { get; set; }
        public DateTime NotificationTime { get; set; }
    }
}
