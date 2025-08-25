using EP.Application.Common.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IAuthorRepository AuthorRepository { get; }
        IStoryRepository StoryRepository { get; }
        IVolumeRepository VolumeRepository { get; }
        IChapterRepository ChapterRepository { get; }
        IStoryInteractionRepository StoryInteractionRepository { get; }
        IStoryReadRepository StoryReadRepository { get; }
        ICommentRepository CommentRepository { get; }
        IStoryFollowLikeRepository StoryFollowLikeRepository { get; }
        IChapterLikedRepository ChapterLikedRepository { get; }
        IReportRepository ReportRepository { get; }
        IReviewRepository ReviewRepository { get; }
        ITicketRepository TicketRepository { get; }
        IRefundRequestsRepository RefundRequestsRepository { get; }
        IWalletRepository WalletRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        Task<int> CompleteAsync();
    }
}
