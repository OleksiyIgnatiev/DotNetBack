

namespace DotNetBack.Models
{
    public class User
    {
        private User(
            int userId,
            string username,
            string password,
            string email,
            int level,
            string subscription,
            DateTime subscriptionPeriod,
            string notificationType,
            string resetPasswordLink,
            string role,
            TimeSpan notificationTime)
        {
            UserId = userId;
            Username = username;
            Password = password;
            Email = email;
            Level = level;
            Subscription = subscription;
            SubscriptionPeriod = subscriptionPeriod;
            NotificationType = notificationType;
            ResetPasswordLink = resetPasswordLink;
            Role = role;
            NotificationTime = notificationTime;
        }

        public int UserId { get; set; }

        public string Username { get; private set; }

        public string Password { get; private set; }

        public string Email { get; private set; }

        public int Level { get; private set; }

        public string Subscription { get; private set; }

        public DateTime SubscriptionPeriod { get; private set; }

        public string NotificationType { get; private set; }

        public string ResetPasswordLink { get; private set; }

        public string Role { get; private set; }

        public TimeSpan NotificationTime { get; private set; }

        public static User Create(
            int userId,
            string username,
            string password,
            string email,
            int level,
            string subscription,
            DateTime subscriptionPeriod,
            string notificationType,
            string resetPasswordLink,
            string role,
            TimeSpan notificationTime)
        {
            return new User(userId, username, password, email, level, subscription, subscriptionPeriod, notificationType, resetPasswordLink, role, notificationTime);
        }
    }
}
