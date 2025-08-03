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
    public record LikeStoryCommand : IRequest<ApiResponse<string>>
    {
        public int StoryId { get; set; }
        public int UserId { get; set; }
    }
    public class LikeStoryCommandHandler : IRequestHandler<LikeStoryCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public LikeStoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<ApiResponse<string>> Handle(LikeStoryCommand request, CancellationToken cancellationToken)
        {
            var interaction = await _unitOfWork.StoryFollowLikeRepository
                .FindAsync(c => c.StoryId == request.StoryId && c.UserId == request.UserId);
            var story_interaction = await _unitOfWork.StoryInteractionRepository
                .FindAsync(c => c.StoryId == request.StoryId);
            var message = interaction == null || interaction.Follow == false ? "Bạn đã thích truyện" : "Bạn đã bỏ thích truyện";

            try
            {
                if (interaction != null)
                {
                    story_interaction.Like = interaction.Like == true ? story_interaction.Like - 1 : story_interaction.Like + 1;
                    interaction.Like = !interaction.Like;
                }
                else
                {
                    story_interaction.Like += 1;
                    StoryFollowLike storyFollowLike = new StoryFollowLike { UserId = request.UserId, StoryId = request.StoryId, Follow = false, Like = true };
                    await _unitOfWork.StoryFollowLikeRepository.AddAsync(storyFollowLike);
                }
                await _unitOfWork.StoryInteractionRepository.UpdateAsync(story_interaction);
                var affectedRow = await _unitOfWork.CompleteAsync();

                if (affectedRow > 0)
                {
                    return ApiResponse<string>.Success(message);
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
