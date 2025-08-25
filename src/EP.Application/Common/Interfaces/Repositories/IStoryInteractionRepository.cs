using EP.Domain.Models;

namespace EP.Application.Common.Interfaces.Repositories
{
    public interface IStoryInteractionRepository : IRepository<StoryInteraction>
    {
        Task IncrementViewCountAsync(int storyId);
    }
}
