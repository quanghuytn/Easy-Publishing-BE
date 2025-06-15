using app.DTOs.Comment;

namespace app.Interface
{
    public interface ICommentRepository
    {
        Task<List<CommentDto>> GetChapterComments(int userId, int chapterId);
        Task<List<CommentDto>> GetStoryComments(int userId, int storyId);
        Task AddComment(int userId, SendCommentDto newComment);
        Task<bool> UpdateComment(int userId, int commentId, string? commentContent);
        Task<bool> DeleteComment(int commentId);

    }
}
