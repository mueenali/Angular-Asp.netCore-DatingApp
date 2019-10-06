using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data.RepositoryInterfaces;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class PhotoRepository : Repository<Photo>, IPhotoRepository
    {
        private readonly DataContext _context;
        public PhotoRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public void AddPhoto(User user, Photo photo)
        {
            if (!user.Photos.Any(p => p.IsMain))
                photo.IsMain = true;

            user.Photos.Add(photo);
        }

        public async Task<string> ValidatePhoto(User user, int id)
        {
            if (!user.Photos.Any(p => p.Id == id))
                return "Unauthorized";

            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            if (photo.IsMain)
                return "bad request";

            return "Ok";
        }

        public async Task<Photo> FindMainPhoto(int userId)
        {
            return await _context.Photos.Where(p => p.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

    }
}