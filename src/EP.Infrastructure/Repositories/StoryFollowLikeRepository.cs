using EP.Application.Common.Interfaces.Repositories;
using EP.Domain.Models;
using EP.Infrastructure.Data;

namespace EP.Infrastructure.Repositories
{
    public class StoryFollowLikeRepository : Repository<StoryFollowLike>, IStoryFollowLikeRepository
    {
        public StoryFollowLikeRepository(Context context) : base(context)
        {
        }
    }
}
