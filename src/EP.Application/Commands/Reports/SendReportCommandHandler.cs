using EP.Application.Common;
using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Interfaces.Services.Common;
using EP.Domain.Models;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Commands.Reports
{
    public record SendReportCommand : IRequest<ApiResponse<string>>
    {
        public int UserId { get; set; }
        public int ReportTypeId { get; set; }
        public int? StoryId { get; set; }
        public long? ChapterId { get; set; }
        public int? CommentId { get; set; }
        public string? ReportContent { get; set; } = null!;
    }

    public class SendReportCommandValidator : AbstractValidator<SendReportCommand>
    {
        public SendReportCommandValidator()
        {
            RuleFor(command => command.UserId)
                .GreaterThan(0).WithMessage("User Id Không hợp lệ.");

            RuleFor(command => command.ReportTypeId)
                .GreaterThan(0).WithMessage("ReportTypeId Không hợp lệ.");

            RuleFor(command => command.StoryId)
                .NotNull().WithMessage("StoryId is required.")
                .GreaterThan(0).WithMessage("StoryId Không hợp lệ.")
                .When(command => command.StoryId.HasValue);

            RuleFor(command => command.ReportContent)
                .NotEmpty().WithMessage("Nội dung report không được để trống.")
                .MinimumLength(10).WithMessage("Nội dung báo cáo phải có ít nhất 10 ký tự.")
                .MaximumLength(1000).WithMessage("Nội dung báo cáo không được vượt quá 1000 ký tự.");
        }
    }

    public class SendReportCommandHandler : IRequestHandler<SendReportCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ReportType> _reportTypeRepository;
        private readonly IMailService _mailService;
        public SendReportCommandHandler(IUnitOfWork unitOfWork, IRepository<ReportType> reportTypeRepository, IMailService mailService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _reportTypeRepository = reportTypeRepository ?? throw new ArgumentNullException(nameof(reportTypeRepository));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }
        public async Task<ApiResponse<string>> Handle(SendReportCommand request, CancellationToken cancellationToken)
        {
            var user_report = 0;
            var mail_content = "";
            var link = "";
            if (request.CommentId != null)
            {
                var comment = await _unitOfWork.CommentRepository.FindAsync(c => c.CommentId == request.CommentId);
                if (comment == null) return ApiResponse<string>.Failure("Bình luận không tồn tại");
                var storyId = comment.StoryId;

                link = $"https://easy-publishing.vercel.app/story/detail/{storyId}/di-the-ta-quan";
                mail_content = $"<p>Nội dung của bạn: <b>{comment.CommentContent}</b></p>" +
                               $"<p>Xin hãy chỉnh sửa sớm nhất thông qua đường link dưới</p>" +
                               $"<a href=\"{link}\">Link chỉnh sửa</a>";
                user_report = comment.UserId;
            }
            else if (request.StoryId != null)
            {

                link = $"https://easy-publishing.vercel.app/author/write-story?mode=edit&storyId={request.StoryId}";
                if (request.ChapterId != null) link =
                 $"https://easy-publishing.vercel.app/author/write-chapter?mode=edit&storyId={request.StoryId}&chapterId={request.ChapterId}";

                mail_content = $"<p>Nội dung bạn đăng tải đã vi phạm tiêu chí trên" +
                               $"<p>Xin hãy chỉnh sửa sớm nhất thông qua đường link dưới</p>" +
                               $"<a href=\"{link}\">Link chỉnh sửa</a>";
                var author = await _unitOfWork.StoryRepository.FindAsync(c => c.StoryId == request.StoryId);
                user_report = author.AuthorId;

            }

            var report_type = await _reportTypeRepository.FindAsync(c => c.ReportTypeId == request.ReportTypeId);
            var user = await _unitOfWork.UserRepository.FindAsync(c => c.UserId == user_report);
            var name = user.UserFullname == null ? user.Email : user.UserFullname;
            try
            {
                _mailService.Send(user.Email,
                        "Bạn vi phạm nguyên tắc cộng đồng",
                        "<p>Easy Publishing Xin chào <b> " + name + "</b>,</p>" +
                        "<b>Thông tin vi phạm như sau:</b>" +
                        "<p>Nguyên nhân: <b>" + report_type.ReportTypeContent + "</b></p>" +
                        mail_content +
                        "<p>Cảm ơn bạn đã tin tưởng.</p>");
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi gửi báo cáo. Vui lòng thử lại sau. ", ex);
            }
            try
            {
                ReportContent report = new ReportContent()
                {
                    UserId = request.UserId,
                    ReportTypeId = request.ReportTypeId,
                    StoryId = request.StoryId,
                    ChapterId = request.ChapterId,
                    CommentId = request.CommentId,
                    ReportContent1 = request.ReportContent,
                    ReportDate = DateTime.Now,
                    Status = false,
                };
                await _unitOfWork.ReportRepository.AddAsync(report);
                var affectedRows = await _unitOfWork.CompleteAsync();
                if (affectedRows <= 0)
                {
                    return ApiResponse<string>.Failure("Không thể gửi báo cáo, vui lòng thử lại sau.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi gửi báo cáo. Vui lòng thử lại sau. ", ex);
            }
            return ApiResponse<string>.Success("Báo cáo thành công");
        }
    }
}
