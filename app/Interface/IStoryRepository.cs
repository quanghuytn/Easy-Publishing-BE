using app.Models;

namespace app.Interface
{
    public interface IStoryRepository
    {
        Task<Story?> GetStory(int storyId);
    }
}
