using EP.Application.Common;
using EP.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EP.Application.Commands.Stories
{
    public record DeleteStoryCommand : IRequest<ApiResponse<string>>
    {
        public int UserId { get; set; }
        public int StoryId { get; set; }
    }
    public class DeleteStoryCommandHandler : IRequestHandler<DeleteStoryCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteStoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<string>> Handle(DeleteStoryCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
            var currentStory = await _unitOfWork.StoryRepository
                                .FindAsync(s => s.StoryId == request.StoryId
                                               && (s.AuthorId == request.UserId || user.RoleId == 1));
            if (currentStory == null)
            {
                return ApiResponse<string>.Failure("Bạn không có quyền dùng chức năng này");
            }

            if (currentStory.Status == -1)
            {
                return ApiResponse<string>.Failure("Truyện này đã bị khóa");
            }

            currentStory.Status = -1;
            var affectedRows = await _unitOfWork.CompleteAsync();
            if (affectedRows > 0)
            {
                return ApiResponse<string>.Success("Xóa truyện thành công!");
            }
            else
            {
                return ApiResponse<string>.Failure("Xóa truyện thất bại!. Vui lòng thử lại sau.");
            }
        }
    }
}
