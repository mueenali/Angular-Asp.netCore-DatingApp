using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Data.RepositoryInterfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<PagedList<User>> PaginateWithFilter(UserParams userParams);
    }
}