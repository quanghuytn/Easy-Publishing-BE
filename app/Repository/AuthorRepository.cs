using app.DTOs.Author;
using app.Interface;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Repository
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly EasyPublishingContext _context;

        public AuthorRepository(EasyPublishingContext context)
        {
            _context = context;
        }

        public async Task<AuthorDto?> GetAuthorById(int authorId)
        {
            var author = await _context.Users.Where(c => c.UserId == authorId)
                .Include(c => c.Stories)
                .Select(c => new AuthorDto
                {
                    AuthorId = c.UserId,
                    AuthorName = c.UserFullname,
                    AuthorImage = c.UserImage,
                    AuthorEmail = c.Email,
                    AuthorDescriptionHtml = c.DescriptionHtml,
                    AuthorDescriptionMarkdown = c.DescriptionMarkdown,
                    AuthorStories = c.Stories.Count,
                }).FirstOrDefaultAsync();
            return author;
        }

        public async Task<StoryRelateAuthorDto?> GetStoryRelateAuthor(int storyId)
        {
            var story = await _context.Stories.FirstOrDefaultAsync(c => c.StoryId == storyId);
            var author = await _context.Users.Where(c => c.UserId == story.AuthorId)
                .Include(c => c.Stories).ThenInclude(c => c.StoryInteraction)
                .Select(c => new StoryRelateAuthorDto
                {
                    AuthorId = c.UserId,
                    AuthorName = c.UserFullname,
                    AuthorImage = c.UserImage,
                    AuthorStories = c.Stories.Count,
                    Like = c.Stories.Select(c => c.StoryInteraction.Like).Sum(),
                    Read = c.Stories.Select(c => c.StoryInteraction.Read).Sum(),
                    AuthorNewestStory = c.Stories.Where(c => c.AuthorId == story.AuthorId).OrderByDescending(c => c.StoryId)
                    .Select(s => new StoryAuthor {StoryId= s.StoryId, StoryTitle= s.StoryTitle,StoryImage = s.StoryImage, StoryDescription = s.StoryDescription,CreateTime = s.CreateTime })
                    .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            return author;
        }
    }
}
