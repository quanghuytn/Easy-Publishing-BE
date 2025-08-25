using EP.Application.Common;
using EP.Application.Common.DTOs.Story;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Stories
{
    public record GetStoryInformationQuery(int StoryId, int authorId) : IRequest<ApiResponse<StoryInformationDto>>;
    public class GetStoryInformationQueryHandler : IRequestHandler<GetStoryInformationQuery, ApiResponse<StoryInformationDto>>
    {
        private readonly IStoryRepository _storyRepository;
        public GetStoryInformationQueryHandler(IStoryRepository storyRepository)
        {
            _storyRepository = storyRepository;
        }
        public async Task<ApiResponse<StoryInformationDto>> Handle(GetStoryInformationQuery request, CancellationToken cancellationToken)
        {
            var story = await _storyRepository.GetStoryInformation(request.StoryId, request.authorId);
            if (story == null)
            {
                return ApiResponse<StoryInformationDto>.Failure("Truyện không tồn tại hoặc bạn không có quyền truy cập.");
            }

            return ApiResponse<StoryInformationDto>.Success("Thông tin truyện", story);
        }
    }
}
