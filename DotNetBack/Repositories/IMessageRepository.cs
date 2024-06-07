using DotNetBack.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetBack.Repositories
{
    public interface IMessageRepository
    {
        Task<Response> CreateMessageAsync(Message message);
        Task<Response> GetUnreadMessagesAsync(int userId);
    }
}
