using System;
using System.Linq;

using System.Threading.Tasks;
using DatingApp.API.Data.RepositoryInterfaces;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DataContext context) : base(context)
        {

        }

        public async Task<PagedList<User>> PaginateWithFilter(UserParams userParams)
        {
            var users = GetAllWithInclude(u => u.Photos, userParams.PageNumber, userParams.PageSize).OrderByDescending(u => u.LastActive).AsQueryable();
            users = users.Where(u => u.ID != userParams.UserId);
            users = users.Where(u => u.Gender == userParams.Gender);
            if (userParams.MinAge != 18 || userParams.MaxAge != 80)
            {
                var minDateOfBirth = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDateOfBirth = DateTime.Today.AddYears(-userParams.MinAge);
                users = users.Where(u => u.DateOfBirth >= minDateOfBirth && u.DateOfBirth <= maxDateOfBirth);
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                if (userParams.OrderBy == "created")
                    users = users.OrderByDescending(u => u.Created);

                else
                    users = users.OrderByDescending(u => u.LastActive);

            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }
    }
}