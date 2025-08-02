using EP.Application.Common;
using EP.Application.Common.Interfaces;
using FluentValidation;
using MediatR;

namespace EP.Application.Commands.Chapters
{
    public record UpdateChapterCommand : IRequest<ApiResponse<string>>
    {
        public long ChapterId { get; set; }
        public string ChapterTitle { get; set; } = null!;
        public string? ChapterContentMarkdown { get; set; }
        public string? ChapterContentHtml { get; set; }
        public decimal? ChapterPrice { get; set; }
    }

    public class UpdateChapterCommandValidator : AbstractValidator<UpdateChapterCommand>
    {
        public UpdateChapterCommandValidator()
        {
            RuleFor(x => x.ChapterId)
                .GreaterThan(0).WithMessage("ChapterId không hợp lệ.");

            RuleFor(x => x.ChapterTitle)
                .NotEmpty().WithMessage("Tên chương không được để trống.")
                .MaximumLength(100).WithMessage("Tiêu đề không được vượt quá 100 ký tự.");

            RuleFor(x => x.ChapterContentMarkdown)
                .NotEmpty().WithMessage("Nội dung không được để trống!")
                .MinimumLength(1000).WithMessage("Nội dung phải chứa ít nhất 1000 ký tự!");

            RuleFor(x => x.ChapterContentHtml)
                .NotEmpty().WithMessage("Nội dung không được để trống!")
                .MinimumLength(1000).WithMessage("Nội dung phải chứa ít nhất 1000 ký tự!");

            RuleFor(x => x.ChapterPrice).GreaterThanOrEqualTo(0).WithMessage("Giá chương phải lớn hơn hoặc bằng 0.");
        }
    }
    public class UpdateChapterCommandHandler : IRequestHandler<UpdateChapterCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateChapterCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<ApiResponse<string>> Handle(UpdateChapterCommand request, CancellationToken cancellationToken)
        {
            var chapter = await _unitOfWork.ChapterRepository.GetByIdAsync(request.ChapterId);

            if (chapter == null)
            {
                return ApiResponse<string>.Failure("Chapter not found.");
            }

            chapter.ChapterTitle = chapter.ChapterTitle;
            chapter.ChapterContentHtml = chapter.ChapterContentHtml;
            chapter.ChapterContentMarkdown = chapter.ChapterContentMarkdown;
            chapter.ChapterPrice = chapter.ChapterPrice;
            chapter.UpdateTime = DateTime.Now;

            await _unitOfWork.ChapterRepository.UpdateAsync(chapter);
            var affectedRows = await _unitOfWork.CompleteAsync();
            // Check if the update was successful
            if (affectedRows > 0)
            {
                return ApiResponse<string>.Success("Cập nhật thành công!");
            }
            else
            {
                return ApiResponse<string>.Failure("Cập nhật thất bại!");
            }
        }
    }
}
