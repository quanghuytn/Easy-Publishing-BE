using EP.Application.Common;
using EP.Application.Common.DTOs.Story;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Stories
{
    public record GetStoryForPrintQuery(int StoryId, int UserId) : IRequest<ApiResponse<StoryPrintDto>>;
    public class GetStoryForPrintQueryHandler : IRequestHandler<GetStoryForPrintQuery, ApiResponse<StoryPrintDto>>
    {
        private readonly IStoryRepository _storyRepository;
        public GetStoryForPrintQueryHandler(IStoryRepository storyRepository)
        {
            _storyRepository = storyRepository;
        }
        public async Task<ApiResponse<StoryPrintDto>> Handle(GetStoryForPrintQuery request, CancellationToken cancellationToken)
        {
            var story = await _storyRepository.GetStoryForPrint(request.StoryId, request.UserId);
            if (story == null)
            {
                throw new KeyNotFoundException($"Story not found.");
            }

            return ApiResponse<StoryPrintDto>.Success("Thông tin truyện", story);
        }
    }
}
