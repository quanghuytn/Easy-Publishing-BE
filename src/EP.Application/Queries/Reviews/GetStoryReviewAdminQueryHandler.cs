using EP.Application.Common;
using EP.Application.Common.DTOs.Story;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Queries.Reviews
{
    public record GetStoryReviewAdminQuery : IRequest<ApiResponse<IEnumerable<StoryReviewAdminDto>>>;
    public class GetStoryReviewAdminQueryHandler : IRequestHandler<GetStoryReviewAdminQuery, ApiResponse<IEnumerable<StoryReviewAdminDto>>>
    {
        private readonly IStoryRepository _storyRepository;
        public GetStoryReviewAdminQueryHandler(IStoryRepository storyRepository)
        {
            _storyRepository = storyRepository ?? throw new ArgumentNullException(nameof(storyRepository));
        }
        public async Task<ApiResponse<IEnumerable<StoryReviewAdminDto>>> Handle(GetStoryReviewAdminQuery request, CancellationToken cancellationToken)
        {
            var storyReviews = await _storyRepository.GetStoryReviewAdmin();

            return ApiResponse<IEnumerable<StoryReviewAdminDto>>.Success("Danh sách truyện có chương cần review", storyReviews);
        }
    }
}
