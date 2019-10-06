using System.Threading.Tasks;
using DatingApp.API.Data.RepositoryInterfaces;

namespace DatingApp.API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;

        public IAuthRepository authRepository { get; private set; }

        public IUserRepository userRepository { get; private set; }

        public IPhotoRepository photoRepository { get; private set; }

        public UnitOfWork(DataContext context)
        {
            this._context = context;
            authRepository = new AuthRepository(_context);
            userRepository = new UserRepository(_context);
            photoRepository = new PhotoRepository(_context);
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