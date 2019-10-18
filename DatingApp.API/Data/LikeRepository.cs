using System.Threading.Tasks;
using DatingApp.API.Data.RepositoryInterfaces;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class LikeRepository : Repository<Like>, ILikeRepository
    {
        private readonly DataContext _context;

        public LikeRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientId);
        }
    }
}