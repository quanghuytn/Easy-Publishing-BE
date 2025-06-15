using app.DTOs.Review;
using app.Models;

namespace app.Interface
{
    public interface IReviewRepository
    {
        Task<bool> SendReview(int userId, SendReviewDto data);
        Task<Review?> GetReviewByChapter(int chapterId);
        Task<ReviewDto?> GetReviewDetail(int chapterId);
        Task<List<ChapterReviewDto>> GetChapterNotReviewOfAuthor(int authorId);
        Task<List<ChapterReviewDto>> GetChapterNotReview(int authorId);


    }
}
