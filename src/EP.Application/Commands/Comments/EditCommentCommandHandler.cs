using EP.Application.Common;
using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Commands.Comments
{
    public record EditCommentCommand : IRequest<ApiResponse<string>>
    {
        public long CommentId { get; set; }
        public int UserId { get; set; }
        public string CommentContent { get; set; }
    }

    public class EditCommentCommandValidator : AbstractValidator<EditCommentCommand>
    {
        public EditCommentCommandValidator()
        {
            RuleFor(command => command.CommentId)
                .NotNull().WithMessage("StoryId is required.")
                .GreaterThan(0).WithMessage("StoryId không hợp lệ.");

            RuleFor(command => command.UserId)
                .GreaterThan(0).WithMessage("User Id không hợp lệ.");

            RuleFor(command => command.CommentContent)
                .NotEmpty().WithMessage("Nội dung bình luận không được để trống.")
                .MinimumLength(10).WithMessage("Nội dung bình luận phải có ít nhất 10 ký tự")
                .MaximumLength(1000).WithMessage("Nội dung bình luận không được vượt quá 1000 ký tự");
        }
    }
    public class EditCommentCommandHandler : IRequestHandler<EditCommentCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EditCommentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<ApiResponse<string>> Handle(EditCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = await _unitOfWork.CommentRepository.FindAsync(c => c.UserId == request.UserId && c.CommentId == request.CommentId);
            if (comment == null)
            {
                throw new Exception("Bình luận không tồn tại.");
            }
            if(string.IsNullOrEmpty(request.CommentContent))
            {
                throw new Exception("Nội dung bình luận không được để trống.");
            }
            comment.CommentContent = request.CommentContent;

            await _unitOfWork.CommentRepository.UpdateAsync(comment);
            var affectedRows = await _unitOfWork.CompleteAsync();
            if (affectedRows <= 0)
            {
                return ApiResponse<string>.Failure("Sửa bình luận thất bại. Vui lòng thử lại sau!");
            }

            return ApiResponse<string>.Success("Sửa bình luận thành công!");
        }
    }
}
