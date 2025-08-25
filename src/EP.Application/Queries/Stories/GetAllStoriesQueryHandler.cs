using EP.Application.Common;
using EP.Application.Common.DTOs.Story;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Stories
{
    public record GetAllStoriesQuery : IRequest<ApiResponse<IEnumerable<StoryListDto>>>;
    public class GetAllStoriesQueryHandler : IRequestHandler<GetAllStoriesQuery, ApiResponse<IEnumerable<StoryListDto>>>
    {
        private readonly IStoryRepository _storyRepository;
        public GetAllStoriesQueryHandler(IStoryRepository storyRepository)
        {
            _storyRepository = storyRepository;
        }
        public async Task<ApiResponse<IEnumerable<StoryListDto>>> Handle(GetAllStoriesQuery request, CancellationToken cancellationToken)
        {
            var stories = await _storyRepository.GetAllStories();

            return ApiResponse<IEnumerable<StoryListDto>>.Success("Danh sách truyện", stories);
        }
    }
}
