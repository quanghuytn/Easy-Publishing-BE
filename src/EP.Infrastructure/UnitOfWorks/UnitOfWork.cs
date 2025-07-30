using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Repositories;
using EP.Infrastructure.Data;
using EP.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.UnitOfWorks
{
    public class UnitOfWork(Context context) : IUnitOfWork
    {
        private readonly Context _context = context ?? throw new ArgumentNullException(nameof(context));
        private bool _disposed;
        private IUserRepository? _userRepository;
        private ICategoryRepository? _categoryRepository;

        public IUserRepository UserRepository
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new UserRepository(_context);
                }
                return _userRepository;
            }
        }

        public ICategoryRepository CategoryRepository
        {
            get
            {
                if (_categoryRepository == null)
                {
                    _categoryRepository = new CategoryRepository(_context);
                }
                return _categoryRepository;
            }
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }
    }
}
