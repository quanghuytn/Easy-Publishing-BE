using EP.Application.Common.DTOs.Chapter;
using EP.Application.Common.Pagination;
using EP.Domain.Models;

namespace EP.Application.Common.Interfaces.Repositories
{
    public interface IChapterRepository : IRepository<Chapter>
    {
        Task<long> GetLastestChapterNumberInVoumeAsync(int storyId, long volumeId);
        Task RenumberChaptersAfterAddAsync(int storyId, long startChapterNumber, bool isDelete = false);
        Task<PaginatedResult<MinimalChapterDto>> GetStoryChapters(int storyId, int page, int pageSize);
        Task<PaginatedResult<ChapterReviewDto>> GetChapterNotReviewOfAuthor(int authorId, int page, int pageSize);
        Task<PaginatedResult<ChapterReviewDto>> GetChapterNotReview(int authorId, int page, int pageSize);
        Task<ChapterDto?> GetChapterInfor(int chapterId);
        Task<ChapterInformationReviewDto?> GetChapterInformationToReview(int chapterId);
        Task<ChapterContentDto?> GetChapterContent(int userId, long chapterNumber, int storyId, bool hasPurchased);

    }
}
