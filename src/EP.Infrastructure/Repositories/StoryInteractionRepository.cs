using EP.Application.Common.Interfaces.Repositories;
using EP.Domain.Models;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Repositories
{
    public class StoryInteractionRepository : Repository<StoryInteraction>, IStoryInteractionRepository
    {
        public StoryInteractionRepository(Context context) : base(context)
        {
        }

        public async Task IncrementViewCountAsync(int storyId)
        {
            await _dbSet
                .Where(si => si.StoryId == storyId)
                .ExecuteUpdateAsync(s => s.SetProperty(si => si.View, si => si.View + 1));
        }
    }
}
