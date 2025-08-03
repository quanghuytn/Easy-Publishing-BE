using EP.Application.Common;
using EP.Application.Common.DTOs.Chapter;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Reviews
{
    public record GetChapterInformationToReviewQuery(int ChapterId) : IRequest<ApiResponse<ChapterInformationReviewDto?>>;
    public class GetChapterInformationToReviewQueryHandler : IRequestHandler<GetChapterInformationToReviewQuery, ApiResponse<ChapterInformationReviewDto?>>
    {
        private readonly IChapterRepository _chapterRepository;
        public GetChapterInformationToReviewQueryHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository ?? throw new ArgumentNullException(nameof(chapterRepository));
        }
        public async Task<ApiResponse<ChapterInformationReviewDto?>> Handle(GetChapterInformationToReviewQuery request, CancellationToken cancellationToken)
        {
            var chapter = await _chapterRepository.GetChapterInformationToReview(request.ChapterId);
            if (chapter == null)
            {
                return ApiResponse<ChapterInformationReviewDto?>.Failure("Chương không tồn tại hoặc đã bị xóa.");
            }

            if (chapter.ChapterStatus != 0)
            {
                return ApiResponse<ChapterInformationReviewDto?>.Failure("Chương không thể được review vì đã được duyệt hoặc không còn khả dụng.");
            }

            return ApiResponse<ChapterInformationReviewDto?>.Success("Thông tin chương để review", chapter);
        }
    }
}
