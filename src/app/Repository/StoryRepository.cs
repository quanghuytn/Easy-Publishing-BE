using app.Interface;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Repository
{
    public class StoryRepository : IStoryRepository
    {
        private readonly EasyPublishingContext _context;

        public StoryRepository(EasyPublishingContext context)
        {
            _context = context;
        }
        public async Task<Story?> GetStory(int storyId)
        {
            return await _context.Stories.FirstOrDefaultAsync(s => s.StoryId == storyId);
        }
    }
}
