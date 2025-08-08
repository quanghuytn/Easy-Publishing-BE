using EP.Application.Common.DTOs.Category;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Pagination;

namespace EP.Application.Common.Interfaces.Repositories
{
    public interface IShelvesRepository
    {
        Task<PaginatedResult<TopStoryDto>> GetTopFamousStories(int page, int pageSize);
        Task<PaginatedResult<TopStoryDto>> GetMinimalTopFamousStories(int page, int pageSize);
        Task<PaginatedResult<TopStoryDto>> GetMinimalTopLatestStoriesByChapter(int page, int pageSize);
        Task<PaginatedResult<TopStoryDto>> GetTopLatestStoriesByChapter(int page, int pageSize);
        Task<PaginatedResult<TopStoryDto>> GetTopLatestStories(int page, int pageSize);
        Task<PaginatedResult<TopStoryDto>> GetMinimalTopLatestStories(int page, int pageSize);
        Task<PaginatedResult<TopStoryDto>> GetTopStoriesRead(int page, int pageSize);
        Task<PaginatedResult<TopStoryDto>> GetMinimalTopStoriesRead(int page, int pageSize);
        Task<PaginatedResult<TopStoryDto>> GetStoriesEachCate(int categoryId, int page, int pageSize);
        Task<PaginatedResult<TopStoryDto>> GetStoriesDoneEachCate(int categoryId, int page, int pageSize);
        Task<PaginatedResult<TopStoryDto>> GetOwnedStory(int userId, int page, int pageSize);
        Task<PaginatedResult<TopStoryDto>> GetFollowedStory(int userId, int page, int pageSize);
        Task<PaginatedResult<TopStoryDto>> GetReadHistory(int userId, int page, int pageSize);
        Task<PaginatedResult<TopStoryDto>> FilterStory(string? title,int? to, int? from, string? sort, List<int> cates,
                                                        int? status, int page, int pageSize);
        Task<PaginatedResult<StoryOfAuthorDto>> GetStoryOfAuthor(int authorId, string? title, string? sort, int page, int pageSize);
        Task<IEnumerable<TopStoryDto>> GetTopStoriesReadShelves(int cateId);
        Task<IEnumerable<TopStoryDto>> GetTopFamousStoryOfAuthor(int authorId);
        Task<IEnumerable<TopStoryDto>> GetTopPurchaseStoryOfAuthor(int authorId);
        Task<IEnumerable<TopStoryDto>> GetNewestStoryOfAuthor(int authorId);
        Task<IEnumerable<TopStoryDto>> GetWrittenStoryOfAuthor(int authorId);
        Task<IEnumerable<TopStoryDto>> GetStoriesTopCate(int cateId);
        Task<IEnumerable<TopStoryDto>> GetTop6StoriesPurchase();
        Task<IEnumerable<TopSaleDto>> GetTop6StoriesSale();
        Task<IEnumerable<TopAuthorRevenueDto>> GetTop6AuthorRevenue();
        Task<IEnumerable<CategoryWithStoryDto>> GetStoriesInCategoryShelf();
    }
}
