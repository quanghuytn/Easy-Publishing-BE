using EP.Domain.Models;

namespace EP.Application.Common.Interfaces.Repositories
{
    public interface IStoryReadRepository : IRepository<StoryRead>
    {
        Task<long> GetLastestChapterUserRead(int storyId, int userId);
    }
}
