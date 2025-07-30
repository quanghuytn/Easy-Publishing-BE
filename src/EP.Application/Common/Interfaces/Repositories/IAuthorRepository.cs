using EP.Application.Common.DTOs.Author;

namespace EP.Application.Common.Interfaces.Repositories
{
    public interface IAuthorRepository
    {
        Task<StoryRelateAuthorDto?> GetStoryRelateAuthor(int authorId);
        Task<AuthorDto?> GetAuthorById(int authorId);
    }
}
