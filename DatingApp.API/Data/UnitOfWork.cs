using System.Threading.Tasks;
using DatingApp.API.Data.RepositoryInterfaces;
using Microsoft.Extensions.Configuration;

namespace DatingApp.API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public IAuthRepository authRepository { get; private set; }

        public IUserRepository userRepository { get; private set; }

        public IPhotoRepository photoRepository { get; private set; }

        public IStorageService storageService { get; private set; }

        public UnitOfWork(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            authRepository = new AuthRepository(_context);
            userRepository = new UserRepository(_context);
            photoRepository = new PhotoRepository(_context);
            storageService = new StorageService(_configuration);
        }
        public async Task<bool> Commit()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}