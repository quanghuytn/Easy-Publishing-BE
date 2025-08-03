using EP.Application.Common;
using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Commands.Comments
{
    public record AddCommentCommand : IRequest<ApiResponse<string>>
    {
        public int? StoryId { get; set; }
        public int UserId { get; set; }
        public long? ChapterId { get; set; }
        public string? CommentContent { get; set; }
    }

    public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
    {
        public AddCommentCommandValidator()
        {
            RuleFor(command => command.StoryId)
                .NotNull().WithMessage("StoryId is required.")
                .GreaterThan(0).WithMessage("StoryId không hợp lệ.");

            RuleFor(command => command.UserId)
                .GreaterThan(0).WithMessage("User Id không hợp lệ.");

            //RuleFor(command => command.ChapterId)
            //    .NotNull().WithMessage("ChapterId is required.")
            //    .GreaterThan(0).WithMessage("ChapterId không hợp lệ.");

            RuleFor(command => command.CommentContent)
                .NotEmpty().WithMessage("CommentContent is required.")
                .MinimumLength(10).WithMessage("Nội dung bình luận phải có ít nhất 10 ký tự")
                .MaximumLength(1000).WithMessage("Nội dung bình luận không được vượt quá 1000 ký tự");
        }
    }

    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddCommentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<ApiResponse<string>> Handle(AddCommentCommand request, CancellationToken cancellationToken)
        {
            var comment = new Comment()
            {
                UserId = request.UserId,
                StoryId = request.StoryId,
                ChapterId = request.ChapterId,
                CommentContent = request.CommentContent,
                CommentDate = DateTime.Now,
            };
            await _unitOfWork.CommentRepository.AddAsync(comment);
            var affectedRows = await _unitOfWork.CompleteAsync();
            if (affectedRows <= 0)
            {
                return ApiResponse<string>.Failure("Bình luận thất bại. Vui lòng thử lại sau!");
            }
            
            return ApiResponse<string>.Success(null, "Bình luận thành công!");
        }
    }
}
