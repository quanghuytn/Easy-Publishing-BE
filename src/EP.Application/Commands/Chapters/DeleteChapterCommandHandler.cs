using EP.Application.Common;
using EP.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Commands.Chapters
{
    public record DeleteChapterCommand : IRequest<ApiResponse<string>>
    {
        public long ChapterId { get; set; }
        public int UserId { get; set; }
    }
    public class DeleteChapterCommandHandler : IRequestHandler<DeleteChapterCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteChapterCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<ApiResponse<string>> Handle(DeleteChapterCommand request, CancellationToken cancellationToken)
        {
            var currentChapter = await _unitOfWork.ChapterRepository.GetByIdAsync(request.ChapterId);

            if (currentChapter == null || currentChapter.Status == -1)
            {
                return ApiResponse<string>.Failure("Chapter không tồn tại.");
            }

            int storyId = currentChapter.StoryId;
            var story = await _unitOfWork.StoryRepository.GetByIdAsync(storyId);
            // Check if the user is the author of the chapter
            if (story.AuthorId != request.UserId)
            {
                return ApiResponse<string>.Failure("Bạn không có quyền xóa chương này!.");
            }

            currentChapter.Status = -1;
            await _unitOfWork.ChapterRepository
                .RenumberChaptersAfterAddAsync(currentChapter.StoryId, currentChapter.ChapterNumber, true);
            await _unitOfWork.ChapterRepository.UpdateAsync(currentChapter);
            var affectedRows = await _unitOfWork.CompleteAsync();

            if (affectedRows <= 0)
            {
                return ApiResponse<string>.Failure("Xóa chương thất bại! Vui lòng thử lại sau.");
            }
            // Successfully deleted the chapter
            return ApiResponse<string>.Failure("Xóa chương thành công!.");
        }
    }
}
