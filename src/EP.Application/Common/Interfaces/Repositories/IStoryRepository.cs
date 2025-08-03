using EP.Application.Common.DTOs.Story;
using EP.Application.Common.Pagination;
using EP.Domain.Models;

namespace EP.Application.Common.Interfaces.Repositories
{
    public interface IStoryRepository : IRepository<Story>
    {
        Task<PaginatedResult<StoryReviewDto>> GetStoryReview(int userId, int page, int pageSize);
        Task<IEnumerable<StoryReviewAdminDto>> GetStoryReviewAdmin();
    }
}
