using DotNetBack.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetBack.Repositories
{
    public interface IMessageRepository
    {
        Task<int> CreateMessageAsync(Message message);
        Task<List<Message>> GetUnreadMessagesAsync(int userId);
    }
}
