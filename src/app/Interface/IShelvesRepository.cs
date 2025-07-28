using app.DTOs.Shelves;

namespace app.Interface
{
    public interface IShelvesRepository
    {
        Task<List<StoryShelfDto>> GetTopFamousStories();
        Task<List<TopLatestStoryDto>> GetTopLatestStoriesByChapter();
        Task<List<TopLatestStoryDto>> GetTopLatestStories();
        Task<List<TopReadStoryDto>> GetTopStoriesRead();
        Task<List<Top6PurchaseDto>> GetTop6StoriesPurchase();
        Task<List<TopPriceStoryDto>> GetTopPriceStories();
        Task<List<TopSaleDto>> GetTop6StoriesSale();
        Task<List<TopAuthorRevenueDto>> GetTop6AuthorRevenue();
    }
}
