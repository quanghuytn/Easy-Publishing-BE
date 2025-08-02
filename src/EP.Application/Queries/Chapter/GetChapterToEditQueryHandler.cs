using EP.Application.Common;
using EP.Application.Common.DTOs.Chapter;
using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Repositories;
using EP.Domain.Models;
using MediatR;

namespace EP.Application.Queries.Chapter
{
    public record GetChapterToEditQuery : IRequest<ApiResponse<ChapterDto>>
    {
        public int ChapterId { get; set; }
        public int UserId { get; set; }
    }
    public class GetChapterToEditQueryHandler : IRequestHandler<GetChapterToEditQuery, ApiResponse<ChapterDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetChapterToEditQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<ChapterDto>> Handle(GetChapterToEditQuery request, CancellationToken cancellationToken)
        {
            var chapter = await _unitOfWork.ChapterRepository.GetChapterInfor(request.ChapterId);
            if (chapter == null)
            {
                throw new ArgumentNullException("Chương không tồn tại", nameof(chapter));
            }

            var story = await _unitOfWork.StoryRepository.GetByIdAsync(chapter.StoryId);
            if(story.AuthorId != request.UserId)
            {
                throw new Exception("Bạn không được quyền vào trang này");
            }

            return ApiResponse<ChapterDto>.Success(chapter, "Thông tin chương");
        }
    }
}
