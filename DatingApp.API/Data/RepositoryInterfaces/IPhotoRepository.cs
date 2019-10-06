using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp.API.Data.RepositoryInterfaces
{
    public interface IPhotoRepository : IRepository<Photo>
    {
        void AddPhoto(User user, Photo photo);
        Task<string> ValidatePhoto(User user, int id);
        Task<Photo> FindMainPhoto(int userId);

    }
}