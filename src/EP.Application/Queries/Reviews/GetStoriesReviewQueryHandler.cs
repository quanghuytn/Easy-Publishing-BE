using EP.Application.Common;
using EP.Application.Common.DTOs.Chapter;
using EP.Application.Common.DTOs.Story;
using EP.Application.Common.Interfaces;
using EP.Application.Common.Pagination;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Queries.Reviews
{
    public record GetStoriesReviewQuery : IRequest<ApiResponse<PaginatedResult<StoryReviewDto>>>
    {
        public int UserId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = 10;
    }
    public class GetStoriesReviewQueryHandler : IRequestHandler<GetStoriesReviewQuery, ApiResponse<PaginatedResult<StoryReviewDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetStoriesReviewQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<ApiResponse<PaginatedResult<StoryReviewDto>>> Handle(GetStoriesReviewQuery request, CancellationToken cancellationToken)
        {
            var data = await _unitOfWork.StoryRepository.GetStoryReview(request.UserId, request.PageIndex, request.PageSize);

            return ApiResponse<PaginatedResult<StoryReviewDto>>.Success("Danh sách truyện có chương cần review", data);
        }
    }
}
