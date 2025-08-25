using EP.Application.Common.Interfaces.Repositories;
using EP.Domain.Models;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Repositories
{
    public class StoryReadRepository : Repository<StoryRead>, IStoryReadRepository
    {
        public StoryReadRepository(Context context) : base(context)
        {
        }

        public async Task<long> GetLastestChapterUserRead(int storyId, int userId)
        {
            if (userId == 0) return 1;

            return await _dbSet
                .Where(sr => sr.UserId == userId && sr.StoryId == storyId)
                .OrderByDescending(sr => sr.ReadTime)
                .Select(sr => (long?)sr.Chapter.ChapterNumber)
                .FirstOrDefaultAsync() ?? 1;
        }
    }
}
