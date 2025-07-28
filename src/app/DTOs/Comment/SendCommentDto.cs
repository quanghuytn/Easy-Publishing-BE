namespace app.DTOs.Comment
{
    public class SendCommentDto
    {
        public int? StoryId { get; set; }

        public long? ChapterId { get; set; }

        public string CommentContent { get; set; } = null!;

    }
}
