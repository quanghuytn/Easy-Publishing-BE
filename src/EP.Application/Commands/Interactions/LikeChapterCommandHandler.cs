using EP.Application.Common;
using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Commands.Interactions
{
    public record LikeChapterCommand : IRequest<ApiResponse<string>>
    {
        public int ChapterNumber { get; set; }
        public int StoryId { get; set; }
        public int UserId { get; set; }
    }
    public class LikeChapterCommandHandler : IRequestHandler<LikeChapterCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public LikeChapterCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<ApiResponse<string>> Handle(LikeChapterCommand request, CancellationToken cancellationToken)
        {
            var chapter = await _unitOfWork.ChapterRepository.FindAsync(c => c.StoryId == request.StoryId && c.ChapterNumber == request.ChapterNumber);
            var interaction = await _unitOfWork.ChapterLikedRepository.FindAsync(c => c.ChapterId == chapter.ChapterId && c.UserId == request.UserId);
            var story_interaction = await _unitOfWork.StoryInteractionRepository.FindAsync(c => c.StoryId == request.StoryId);

            var msg = interaction == null ? "Bạn đã thích chương" : "Bạn đã bỏ thích chương";
            try
            {
                if (interaction != null)
                {
                    story_interaction.Like -= 1;
                    await _unitOfWork.ChapterLikedRepository.Remove(interaction);
                }
                else
                {
                    story_interaction.Like += 1;
                    ChapterLiked chapterLiked = new ChapterLiked { UserId = request.UserId, ChapterId = chapter.ChapterId, Status = null };
                    await _unitOfWork.ChapterLikedRepository.AddAsync(chapterLiked);
                }
                await _unitOfWork.StoryInteractionRepository.UpdateAsync(story_interaction);
                var affectedRows = await _unitOfWork.CompleteAsync();
                if (affectedRows > 0)
                {
                    return ApiResponse<string>.Success(msg);
                }
                else
                {
                    return ApiResponse<string>.Failure("Không thể thực hiện thao tác này. Vui lòng thử lại sau.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Hệ thống xảy ra lỗi. Vui lòng thử lại sau!", ex);
            }
        }
    }
}
