using System;
using System.Threading.Tasks;

namespace DatingApp.API.Data.RepositoryInterfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task<bool> Commit();
        IAuthRepository authRepository { get; }
        IUserRepository userRepository { get; }
        IPhotoRepository photoRepository { get; }
        IStorageService storageService { get; }

    }
}