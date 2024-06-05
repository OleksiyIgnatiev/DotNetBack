using DotNetBack.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetBack.Repositories
{
    public interface IUserRepository
    {
        Task<Response> LoginAsync(string username, string passwordHash);
        Task<Response> RegisterAsync(string username, string email, string password);
        Task<Response> ResetAsync(string email);
        Task<Response> UpdateNotificationAsync(int userId, string notificationType, DateTime notificationTime);
        Task<Response> UpdateUserAsync(int userId, string username, string email, string password);
        Task<Response> UpdateSubscriptionAsync(int userId, string subscription, DateTime subscriptionPeriod);
        //Task LogoutAsync(int userId);
        Task<Response> GetAllUsersAsync();
        //Task<object> AdminActionAsync(int userId);
        Task<Response> DeleteUserAsync(int userId);
        //Task<IEnumerable<object>> ShareSuccessesAsync(int userId);
        Task<Response> GetCalendarAsync(int userId);
        Task<Response> GetRecordAsync(int userId);
        Task<Response> GetUserInfoAsync(int userId);
        Task<Response> GetUserByEmailAsync(string email);
    }
}
