using EP.Application.Common.DTOs.Review;

namespace EP.Application.Common.Interfaces.Repositories
{
    public interface IReviewRepository : IRepository<Domain.Models.Review>
    {
        Task<ReviewDto?> GetReviewDetail(int chapterId);
    }
}
