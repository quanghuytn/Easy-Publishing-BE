using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Repositories;
using EP.Infrastructure.Data;
using EP.Infrastructure.Repositories;

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
        private IChapterRepository? _chapterRepository;
        private IStoryReadRepository? _storyReadRepository;
        private IStoryInteractionRepository? _storyInteractionRepository;
        private ICommentRepository? _commentRepository;
        private IStoryFollowLikeRepository? _storyFollowLikeRepository;
        private IChapterLikedRepository? _chapterLikedRepository;
        private IReportRepository? _reportRepository;
        private IReviewRepository? _reviewRepository;
        private ITicketRepository? _ticketRepository;
        private IRefundRequestsRepository? _refundRequestsRepository;
        private IWalletRepository? _walletRepository;
        private ITransactionRepository? _transactionRepository;

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

        public IChapterRepository ChapterRepository
        {
            get
            {
                if (_chapterRepository == null)
                {
                    _chapterRepository = new ChapterRepository(_context);
                }
                return _chapterRepository;
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

        public IStoryReadRepository StoryReadRepository
        {
            get
            {
                if (_storyReadRepository == null)
                {
                    _storyReadRepository = new StoryReadRepository(_context);
                }
                return _storyReadRepository;
            }
        }

        public IStoryInteractionRepository StoryInteractionRepository
        {
            get
            {
                if (_storyInteractionRepository == null)
                {
                    _storyInteractionRepository = new StoryInteractionRepository(_context);
                }
                return _storyInteractionRepository;
            }
        }
        public ICommentRepository CommentRepository
        {
            get
            {
                if (_commentRepository == null)
                {
                    _commentRepository = new CommentRepository(_context);
                }
                return _commentRepository;
            }
        }
        public IStoryFollowLikeRepository StoryFollowLikeRepository
        {
            get
            {
                if (_storyFollowLikeRepository == null)
                {
                    _storyFollowLikeRepository = new StoryFollowLikeRepository(_context);
                }
                return _storyFollowLikeRepository;
            }
        }
        public IChapterLikedRepository ChapterLikedRepository
        {
            get
            {
                if (_chapterLikedRepository == null)
                {
                    _chapterLikedRepository = new ChapterLikedRepository(_context);
                }
                return _chapterLikedRepository;
            }
        }
        public IReportRepository ReportRepository
        {
            get
            {
                if (_reportRepository == null)
                {
                    _reportRepository = new ReportRepository(_context);
                }
                return _reportRepository;
            }
        }
        public IReviewRepository ReviewRepository
        {
            get
            {
                if (_reviewRepository == null)
                {
                    _reviewRepository = new ReviewRepository(_context);
                }
                return _reviewRepository;
            }
        }
        public ITicketRepository TicketRepository
        {
            get
            {
                if (_ticketRepository == null)
                {
                    _ticketRepository = new TicketRepository(_context);
                }
                return _ticketRepository;
            }
        }
        public IRefundRequestsRepository RefundRequestsRepository
        {
            get
            {
                if (_refundRequestsRepository == null)
                {
                    _refundRequestsRepository = new RefundRequestsRepository(_context);
                }
                return _refundRequestsRepository;
            }
        }
        public IWalletRepository WalletRepository
        {
            get
            {
                if (_walletRepository == null)
                {
                    _walletRepository = new WalletRepository(_context);
                }
                return _walletRepository;
            }
        }
        public ITransactionRepository TransactionRepository
        {
            get
            {
                if (_transactionRepository == null)
                {
                    _transactionRepository = new TransactionRepository(_context);
                }
                return _transactionRepository;
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
