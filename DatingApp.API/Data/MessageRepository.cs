using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data.RepositoryInterfaces;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        private readonly DataContext _context;
        public MessageRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int senderId, int receiverId)
        {
            var messages = await _context.Messages
           .Include(m => m.Sender).ThenInclude(u => u.Photos)
           .Include(m => m.Receiver).ThenInclude(u => u.Photos)
           .Where(m => m.SenderId == senderId && m.ReceiverId == receiverId && m.SenderDeleted == false
                  || m.SenderId == receiverId && m.ReceiverId == senderId && m.ReceiverDeleted == false)
           .AsNoTracking()
           .ToListAsync();
            return messages;
        }

        public async Task<PagedList<Message>> GetUserMessages(MessageParams messageParams)
        {
            var messages = _context.Messages
            .Include(m => m.Sender).ThenInclude(u => u.Photos)
            .Include(m => m.Receiver).ThenInclude(u => u.Photos)
            .AsQueryable();

            if (messageParams.MessageContainer == "Inbox")
            {
                messages = messages.Where(m => m.ReceiverId == messageParams.UserId && m.ReceiverDeleted == false);
            }
            else if (messageParams.MessageContainer == "Outbox")
            {
                messages = messages.Where(m => m.SenderId == messageParams.UserId && m.SenderDeleted == false);
            }
            else
            {
                messages = messages.Where(m => m.ReceiverId == messageParams.UserId && m.IsRead == false && m.ReceiverDeleted == false);
            }

            messages = messages.OrderByDescending(m => m.DateSent);
            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);

        }
    }
}