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
        private IAuthorRepository? _authorRepository;
        private IStoryRepository? _storyRepository;
        private IVolumeRepository? _volumeRepository;

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

        public IStoryRepository StoryRepository
        {
            get
            {
                if (_storyRepository == null)
                {
                    _storyRepository = new StoryRepository(_context);
                }
                return _storyRepository;
            }
        }

        public IVolumeRepository VolumeRepository
        {
            get
            {
                if (_volumeRepository == null)
                {
                    _volumeRepository = new VolumeRepository(_context);
                }
                return _volumeRepository;
            }
        }

        public IAuthorRepository AuthorRepository
        {
            get
            {
                if (_authorRepository == null)
                {
                    _authorRepository = new AuthorRepository(_context);
                }
                return _authorRepository;
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
