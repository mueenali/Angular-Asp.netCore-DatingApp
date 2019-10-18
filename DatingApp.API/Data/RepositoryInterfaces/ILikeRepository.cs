using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data.RepositoryInterfaces
{
    public interface ILikeRepository : IRepository<Like>
    {
        Task<Like> GetLike(int userId, int recipientId);
    }
}