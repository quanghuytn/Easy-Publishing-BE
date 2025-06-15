using app.DTOs.Author;

namespace app.Interface
{
    public interface IAuthorRepository
    {
        Task<StoryRelateAuthorDto?> GetStoryRelateAuthor(int storyId);
        Task<AuthorDto?> GetAuthorById(int authorId);

    }
}
