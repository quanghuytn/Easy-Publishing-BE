using EP.Application.Common;
using EP.Application.Common.DTOs.Review;
using EP.Application.Common.Interfaces;
using MediatR;

namespace EP.Application.Queries.Reviews
{
    public record GetReviewDetailQuery(int UserId, int ChapterId) : IRequest<ApiResponse<ReviewDto?>>;
    public class GetReviewDetailQueryHandler : IRequestHandler<GetReviewDetailQuery, ApiResponse<ReviewDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetReviewDetailQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<ApiResponse<ReviewDto?>> Handle(GetReviewDetailQuery request, CancellationToken cancellationToken)
        {
            var chapter = await _unitOfWork.ChapterRepository.GetByIdAsync(request.ChapterId);
            if (chapter == null)
            {
                return ApiResponse<ReviewDto?>.Failure("Chương không tồn tại.");
            }

            var story = await _unitOfWork.StoryRepository.GetByIdAsync(chapter.StoryId);
            if (story == null)
            {
                return ApiResponse<ReviewDto?>.Failure("Truyện không tồn tại.");
            }

            if (story.AuthorId != request.UserId)
            {
                return ApiResponse<ReviewDto?>.Failure("Bạn không có quyền truy cập!.");
            }

            var review = await _unitOfWork.ReviewRepository.GetReviewDetail(request.ChapterId);
            if (review == null)
            {
                return ApiResponse<ReviewDto?>.Failure("Chương chưa được review.");
            }

            if(review.Reviewer.UserId != request.UserId)
            {
                return ApiResponse<ReviewDto?>.Failure("Bạn không có quyền truy cập!.");
            }

            return ApiResponse<ReviewDto?>.Success("Thông tin review của chương", review);
        }
    }
}
