using EP.Application.Common;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Stories
{
    public record GetRelatedStoriesQuery(int StoryId) : IRequest<ApiResponse<IEnumerable<TopStoryDto>>>;
    public class GetRelatedStoriesQueryHandler : IRequestHandler<GetRelatedStoriesQuery, ApiResponse<IEnumerable<TopStoryDto>>>
    {
        private readonly IStoryRepository _storyRepository;
        public GetRelatedStoriesQueryHandler(IStoryRepository storyRepository)
        {
            _storyRepository = storyRepository;
        }
        public async Task<ApiResponse<IEnumerable<TopStoryDto>>> Handle(GetRelatedStoriesQuery request, CancellationToken cancellationToken)
        {
            var relatedStories = await _storyRepository.GetRelatedStories(request.StoryId);

            return ApiResponse<IEnumerable<TopStoryDto>>.Success("Related stories retrieved successfully.", relatedStories);
        }
    }
}
