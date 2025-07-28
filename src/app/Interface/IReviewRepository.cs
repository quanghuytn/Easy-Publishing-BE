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
        Task<List<StoryReviewAdminDto>> GetStoryReviewAdmin();
        Task<ChapterInformationReviewDto?> GetChapterInformationReview(int chapterId);
        Task<List<VolumeReviewDto>> GetVolumeReview(int storyId, int userId);
        Task<List<StoryReviewDto>> GetStoryReview(int userId);
    }
}
