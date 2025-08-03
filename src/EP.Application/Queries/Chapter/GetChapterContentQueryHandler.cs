using EP.Application.Common;
using EP.Application.Common.DTOs.Chapter;
using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace EP.Application.Queries.Chapter
{
    public record GetChapterContentQuery : IRequest<ApiResponse<ChapterContentDto>>
    {
        public long ChapterNumber { get; set; }
        public int StoryId { get; set; }
        public int UserId { get; set; }
    }
    public class GetChapterContentQueryHandler : IRequestHandler<GetChapterContentQuery, ApiResponse<ChapterContentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetChapterContentQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<ApiResponse<ChapterContentDto>> Handle(GetChapterContentQuery request, CancellationToken cancellationToken)
        {
            var hasPurchased = false;

            if(request.UserId > 0)
            {
                hasPurchased = await _unitOfWork.UserRepository
                    .CheckPurchase(request.UserId, request.ChapterNumber, request.StoryId);
            }

            var chapter = await _unitOfWork.ChapterRepository
                .GetChapterContent(request.UserId, request.ChapterNumber, request.StoryId, hasPurchased);

            if (chapter != null && chapter.Owned && request.UserId != 0)
            {
                var story_interaction = await _unitOfWork.StoryInteractionRepository.GetByIdAsync(request.StoryId);
                story_interaction.Read += 1;

                var story_read = await _unitOfWork.StoryReadRepository.FindAsync(c => c.UserId == request.UserId && c.StoryId == request.StoryId);
                if (story_read != null)
                {
                    story_read.ChapterId = chapter.ChapterId;
                    story_read.ReadTime = DateTime.Now;
                }
                else
                {
                    await _unitOfWork.StoryReadRepository.AddAsync(new StoryRead
                    {
                        StoryId = chapter.Story.StoryId,
                        UserId = request.UserId,
                        ChapterId = chapter.ChapterId,
                        ReadTime = DateTime.Now
                    });
                }

                await _unitOfWork.CompleteAsync();
            }

            if (chapter == null)
            {
                return ApiResponse<ChapterContentDto>.Failure("Chương không khả dụng. Vui lòng thử lại sau!.");
            }

            return ApiResponse<ChapterContentDto>.Success("Nội dung chương", chapter);
        }
    }
}
