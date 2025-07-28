using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace EP.Infrastructure.Repositories
{
    public class StoryRepository : Repository<Story>, IStoryRepository
    {
        public StoryRepository(Context context) : base(context)
        {
        }
    }
}
