using DotNetBack.Models;

namespace DotNetBack.Repositories
{
    public interface IMessageRepository
    {
        Task AddMessageAsync(Message message);

        Task<List<Message>> GetUnreadMessagesAsync(int userId);
    }
}
