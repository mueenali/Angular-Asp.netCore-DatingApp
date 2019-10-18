using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Data.RepositoryInterfaces
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<PagedList<Message>> GetUserMessages(MessageParams messageParams);
        Task<IEnumerable<Message>> GetMessageThread(int senderId, int receiverId);

    }
}