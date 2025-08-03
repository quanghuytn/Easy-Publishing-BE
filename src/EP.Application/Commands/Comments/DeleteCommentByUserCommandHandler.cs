using EP.Application.Common;
using EP.Application.Common.Interfaces;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Commands.Comments
{
    public class DeleteCommentByUserCommand : IRequest<ApiResponse<string>>
    {
        public long CommentId { get; set; }
        public int UserId { get; set; }
    }

    public class DeleteCommentByUserCommandValidator : AbstractValidator<EditCommentCommand>
    {
        public DeleteCommentByUserCommandValidator()
        {
            RuleFor(command => command.CommentId)
                .NotNull().WithMessage("StoryId is required.")
                .GreaterThan(0).WithMessage("StoryId không hợp lệ.");

            RuleFor(command => command.UserId)
                .GreaterThan(0).WithMessage("User Id không hợp lệ.");
        }
    }
    public class DeleteCommentByUserCommandHandler : IRequestHandler<DeleteCommentByUserCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeleteCommentByUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<ApiResponse<string>> Handle(DeleteCommentByUserCommand request, CancellationToken cancellationToken)
        {
            var comment = await _unitOfWork.CommentRepository.FindAsync(c => c.UserId == request.UserId && c.CommentId == request.CommentId);
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
