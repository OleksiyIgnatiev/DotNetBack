using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetBack.Repositories
{
    public interface IUserRepository
    {
        Task<bool> LoginAsync(string username, string passwordHash);
        Task RegisterAsync(string username, string email, string password);
        Task UpdateNotificationAsync(int userId, string notificationType, DateTime notificationTime);
        Task UpdateUserAsync(int userId, string username, string email, string password);
        Task UpdateSubscriptionAsync(int userId, string subscription, DateTime subscriptionPeriod);
        //Task LogoutAsync(int userId);
        Task<IEnumerable<object>> GetAllUsersAsync();
        //Task<object> AdminActionAsync(int userId);
        Task DeleteUserAsync(int userId);
        //Task<IEnumerable<object>> ShareSuccessesAsync(int userId);
        //Task<IEnumerable<object>> GetCalendarAsync(int userId);
        //Task<object> GetRecordAsync(int userId);
        Task<object> GetUserInfoAsync(int userId);
    }
}
