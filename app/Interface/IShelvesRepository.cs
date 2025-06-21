using app.DTOs.Shelves;

namespace app.Interface
{
    public interface IShelvesRepository
    {
        Task<List<StoryShelfDto>> GetTopFamousStories();
        Task<List<TopLatestStoryDto>> GetTopLatestStoriesByChapter();
        Task<List<TopReadStoryDto>> GetTopStoriesRead();
        Task<List<Top6PurchaseDto>> GetTop6StoriesPurchase();
        Task<List<TopPriceStoryDto>> GetTopPriceStories();

    }
}
