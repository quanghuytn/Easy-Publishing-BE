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
    public record FollowStoryCommand : IRequest<ApiResponse<string>>
    {
        public int StoryId { get; set; }
        public int UserId { get; set; }
    }
    public class FollowStoryCommandHandler : IRequestHandler<FollowStoryCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public FollowStoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<ApiResponse<string>> Handle(FollowStoryCommand request, CancellationToken cancellationToken)
        {
            var interaction = await _unitOfWork.StoryFollowLikeRepository
                .FindAsync(c => c.StoryId == request.StoryId && c.UserId == request.UserId);
            var story_interaction = await _unitOfWork.StoryInteractionRepository
                .FindAsync(c => c.StoryId == request.StoryId);
            var message = interaction == null || interaction.Follow == false ? "Bạn đã theo dõi truyện" : "Bạn đã bỏ theo dõi truyện";
            try
            {
                if (interaction != null)
                {
                    story_interaction.Follow = interaction.Follow == true ? story_interaction.Follow - 1 : story_interaction.Follow + 1;
                    interaction.Follow = !interaction.Follow;
                }
                else
                {
                    story_interaction.Follow += 1;
                    StoryFollowLike storyFollowLike = new StoryFollowLike { UserId = request.UserId, StoryId = request.StoryId, Follow = true, Like = false };
                    await _unitOfWork.StoryFollowLikeRepository.AddAsync(storyFollowLike);
                }
                await _unitOfWork.StoryInteractionRepository.UpdateAsync(story_interaction);
                var affectedRows = await _unitOfWork.CompleteAsync();
                if (affectedRows > 0)
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
