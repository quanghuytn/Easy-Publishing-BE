using EP.Application.Common;
using EP.Application.Common.DTOs.Chapter;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Pagination;
using MediatR;

namespace EP.Application.Queries.Chapter
{
    public record GetStoryChapterQuery : IRequest<ApiResponse<PaginatedResult<MinimalChapterDto>>>
    {
        public int StoryId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = 10;
    }
    public class GetStoryChapterQueryHandler : IRequestHandler<GetStoryChapterQuery, ApiResponse<PaginatedResult<MinimalChapterDto>>>
    {
        private readonly IChapterRepository _chapterRepository;
        public GetStoryChapterQueryHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository ?? throw new ArgumentNullException(nameof(chapterRepository));
        }
        public async Task<ApiResponse<PaginatedResult<MinimalChapterDto>>> Handle(GetStoryChapterQuery request, CancellationToken cancellationToken)
        {
            var chapters = await _chapterRepository.GetStoryChapters(request.StoryId, request.PageIndex, request.PageSize);

            return ApiResponse<PaginatedResult<MinimalChapterDto>>.Success("Danh sách chương", chapters);
        }
    }
}
