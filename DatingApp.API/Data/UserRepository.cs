using System.Collections.Generic;
using System;
using System.Linq;

using System.Threading.Tasks;
using DatingApp.API.Data.RepositoryInterfaces;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedList<User>> PaginateWithFilter(UserParams userParams)
        {
            var users = GetAllWithInclude(u => u.Photos, userParams.PageNumber, userParams.PageSize).OrderByDescending(u => u.LastActive).AsQueryable();
            users = users.Where(u => u.ID != userParams.UserId);

            if (userParams.Gender != null)
            {
                users = users.Where(u => u.Gender == userParams.Gender);
            }

            if (userParams.Likers)
            {
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikers.Contains(u.ID));
            }

            if (userParams.Likees)
            {
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLikees.Contains(u.ID));
            }

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


        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users.Include(u => u.Likers).Include(u => u.Likees).FirstOrDefaultAsync(u => u.ID == id);
            if (likers)
            {
                return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            else
            {
                return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }
        }
    }
}