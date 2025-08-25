using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.DTOs.Story;
using EP.Application.Common.DTOs.Transaction;
using EP.Application.Common.Pagination;
using EP.Domain.Models;

namespace EP.Application.Common.Interfaces.Repositories
{
    public interface IStoryRepository : IRepository<Story>
    {
        Task<PaginatedResult<StoryReviewDto>> GetStoryReview(int userId, int page, int pageSize);
        Task<IEnumerable<StoryReviewAdminDto>> GetStoryReviewAdmin();
        Task<StoryDetailDto?> GetStoryDetail(int storyId, int userId);
        Task<IEnumerable<StoryListDto>> GetAllStories();
        Task<Story?> GetStoryWithCategory(int storyId, int authorId);
        Task<StoryInformationDto?> GetStoryInformation(int storyId, int authorId);
        Task<IEnumerable<TopStoryDto>> GetRelatedStories(int storyId);
        Task<StoryPrintDto?> GetStoryForPrint(int storyId, int authorId);
        Task<AuthorAndStoryNumberDto?> GetAuthorAndStoryNumber();
        Task<StoryPurchaseDto?> GetStoryPurchaseInfoAsync(int storyId);
        Task<IEnumerable<TopStoryDto>> SearchGlobal(string? search, int? authorId, int? from, int? to, int? status, List<int> cates);
    }
}
