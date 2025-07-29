using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.Interfaces
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
        Task<IEnumerable<TopStoryDto>> GetTopStoriesReadShelves(int cateId);
        Task<IEnumerable<TopStoryDto>> GetStoriesTopCate(int cateId);
        //Task<List<Top6PurchaseDto>> GetTop6StoriesPurchase();
        //Task<List<TopPriceStoryDto>> GetTopPriceStories();
        //Task<List<TopSaleDto>> GetTop6StoriesSale();
        //Task<List<TopAuthorRevenueDto>> GetTop6AuthorRevenue();
    }
}
