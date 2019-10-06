using DatingApp.API.Data.RepositoryInterfaces;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context)
        {

        }
    }
}