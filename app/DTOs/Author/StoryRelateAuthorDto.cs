namespace app.DTOs.Author
{
    public class StoryRelateAuthorDto
    {
        public int AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public string? AuthorImage { get; set; }
        public int AuthorStories { get; set; }
        public int Like { get; set; }
        public int Read { get; set; }
        public StoryAuthor? AuthorNewestStory { get; set; }

    }
}
