using app.DTOs.Interaction;
using app.Models;

namespace app.Interface
{
    public interface IInteractionRepository
    {
        Task<string> FollowStory(int userId, int storyId);
        Task<string> LikeStory(int userId, int storyId);
        Task<string> LikeChapter(int userId, int storyId, int chapterNumber);
        Task<StoryInteractionDto?> GetStoryInteraction(int storyId);
        Task<List<ChapterInteractionDto>> GetStoryChaptersInteraction(int storyId);
    }
}
