using EP.Application.Common.DTOs.Author;
using EP.Application.Common.Interfaces.Repositories;
using EP.Domain.Models;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Repositories
{
    public class AuthorRepository : Repository<User>, IAuthorRepository
    {
        public AuthorRepository(Context context) : base(context)
        {
        }

        public async Task<AuthorDto?> GetAuthorById(int authorId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.UserId == authorId)
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
        }

        public Task<StoryRelateAuthorDto?> GetStoryRelateAuthor(int authorId)
        {
            throw new NotImplementedException();
        }
    }
}
