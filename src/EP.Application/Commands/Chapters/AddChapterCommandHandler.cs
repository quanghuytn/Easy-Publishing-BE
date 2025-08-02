using EP.Application.Common;
using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using FluentValidation;
using MediatR;

namespace EP.Application.Commands.Chapters
{
    public record AddChapterCommand : IRequest<ApiResponse<string>>
    {
        public int StoryId { get; set; }
        public int VolumeId { get; set; }
        public string ChapterTitle { get; set; } = null!;
        public string? ChapterContentMarkdown { get; set; }
        public string? ChapterContentHtml { get; set; }
        public decimal? ChapterPrice { get; set; }
    }

    public class AddChapterCommandValidator : AbstractValidator<AddChapterCommand>
    {
        public AddChapterCommandValidator()
        {
            RuleFor(x => x.StoryId)
                .GreaterThan(0).WithMessage("StoryId không hợp lệ.");

            RuleFor(x => x.VolumeId)
                .GreaterThan(0).WithMessage("VolumeId không hợp lệ.");

            RuleFor(x => x.ChapterTitle)
                .NotEmpty().WithMessage("Tên chương không được để trống.")
                .MaximumLength(100).WithMessage("Tiêu đề không được vượt quá 100 ký tự.");

            RuleFor(x => x.ChapterContentMarkdown)
                .NotEmpty().WithMessage("Nội dung không được để trống!")
                .MinimumLength(1000).WithMessage("Nội dung phải chứa ít nhất 1000 ký tự!");

            RuleFor(x => x.ChapterContentHtml)
                .NotEmpty().WithMessage("Nội dung không được để trống!")
                .MinimumLength(1000).WithMessage("Nội dung phải chứa ít nhất 1000 ký tự!");

            RuleFor(x => x.ChapterPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Giá chương phải lớn hơn hoặc bằng 0.");
        }
    }
    public class AddChapterCommandHandler : IRequestHandler<AddChapterCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddChapterCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<string>> Handle(AddChapterCommand request, CancellationToken cancellationToken)
        {
            var latestChapterNumber = await _unitOfWork.ChapterRepository
                                .GetLastestChapterNumberInVoumeAsync(request.StoryId, request.VolumeId);
            await _unitOfWork.ChapterRepository
                .RenumberChaptersAfterAddAsync(request.StoryId, latestChapterNumber);

            var chapter = new Chapter
            {
                ChapterContentHtml = request.ChapterContentHtml,
                ChapterContentMarkdown = request.ChapterContentMarkdown,
                StoryId = request.StoryId,
                VolumeId = request.VolumeId,
                ChapterTitle = request.ChapterTitle,
                ChapterPrice = request.ChapterPrice,
                ChapterNumber = latestChapterNumber + 1,
                CreateTime = DateTime.Now,
                Status = 0
            };

            await _unitOfWork.ChapterRepository.AddAsync(chapter);
            var affectedRows = await _unitOfWork.CompleteAsync();

            if (affectedRows > 0)
            {
                return ApiResponse<string>.Success("Thêm chương mới thành công");
            }
            else
            {
                return ApiResponse<string>.Failure("Thêm chương thất bại! Vui lòng thử lại sau.");
            }
        }
    }
}
