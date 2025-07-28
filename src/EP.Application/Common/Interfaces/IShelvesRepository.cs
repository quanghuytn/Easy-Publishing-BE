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
        //Task<List<StoryShelfDto>> GetTopFamousStories();
        //Task<List<TopLatestStoryDto>> GetTopLatestStoriesByChapter();
        //Task<List<TopLatestStoryDto>> GetTopLatestStories();
        Task<PaginatedResult<TopReadStoryDto>> GetTopStoriesRead(int page, int pageSize);
        Task<PaginatedResult<TopReadStoryDto>> GetMinimalTopStoriesRead(int page, int pageSize);
        //Task<List<Top6PurchaseDto>> GetTop6StoriesPurchase();
        //Task<List<TopPriceStoryDto>> GetTopPriceStories();
        //Task<List<TopSaleDto>> GetTop6StoriesSale();
        //Task<List<TopAuthorRevenueDto>> GetTop6AuthorRevenue();
    }
}
