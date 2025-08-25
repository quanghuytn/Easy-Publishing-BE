using EP.Application.Common;
using EP.Application.Common.DTOs.Story;
using EP.Application.Common.Interfaces;
using MediatR;

namespace EP.Application.Queries.Stories
{
    public record GetStoryDetailQuery(int StoryId, int UserId) : IRequest<ApiResponse<StoryDetailDto>>;
    public class GetStoryDetailQueryHandler : IRequestHandler<GetStoryDetailQuery, ApiResponse<StoryDetailDto>>
    {
        public readonly IUnitOfWork _unitOfWork;
        public GetStoryDetailQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<StoryDetailDto>> Handle(GetStoryDetailQuery request, CancellationToken cancellationToken)
        {
            var story = await _unitOfWork.StoryRepository.GetStoryDetail(request.StoryId, request.UserId);

            if (story == null)
            {
                throw new KeyNotFoundException($"Story not found.");
            }

            story.LastReadChapter = await _unitOfWork.StoryReadRepository.GetLastestChapterUserRead(request.StoryId, request.UserId);

            await _unitOfWork.StoryInteractionRepository.IncrementViewCountAsync(request.StoryId);
            await _unitOfWork.CompleteAsync();

            return ApiResponse<StoryDetailDto>.Success("Thông tin truyện", story);
        }
    }
}
