using EP.Application.Common.Interfaces.Repositories;
using EP.Domain.Models;
using EP.Infrastructure.Data;

namespace EP.Infrastructure.Repositories
{
    public class ChapterLikedRepository : Repository<ChapterLiked>, IChapterLikedRepository
    {
        public ChapterLikedRepository(Context context) : base(context)
        {
        }

    }
}
