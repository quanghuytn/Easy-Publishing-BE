namespace EP.Application.Common.DTOs.Story
{
    public class StoryPurchaseDto
    {
        public int StoryId { get; set; }
        public string StoryTitle { get; set; }
        public decimal StoryPrice { get; set; }
        public decimal? StorySale { get; set; }
        public int AuthorId { get; set; }
        public AuthorWalletDto? AuthorWallet { get; set; }
    }

    public class AuthorWalletDto
    {
        public int WalletId { get; set; }
        public decimal Refund { get; set; }
    }
}
