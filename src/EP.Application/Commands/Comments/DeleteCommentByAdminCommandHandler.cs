using EP.Application.Common;
using EP.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Commands.Comments
{
    public record DeleteCommentByAdminCommand : IRequest<ApiResponse<string>>
    {
        public int CommentId { get; set; }
    }
    public class DeleteCommentByAdminCommandHandler : IRequestHandler<DeleteCommentByAdminCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteCommentByAdminCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<ApiResponse<string>> Handle(DeleteCommentByAdminCommand request, CancellationToken cancellationToken)
        {
            var comment = await _unitOfWork.CommentRepository.GetByIdAsync(request.CommentId);
            if (comment == null)
            {
                throw new Exception("Bình luận không tồn tại.");
            }
            await _unitOfWork.CommentRepository.Remove(comment);

            var affectedRows = await _unitOfWork.CompleteAsync();
            if (affectedRows <= 0)
            {
                return ApiResponse<string>.Failure("Xóa bình luận thất bại. Vui lòng thử lại sau!");
            }

            return ApiResponse<string>.Success("Xóa bình luận thành công!");

        }
    }
}
