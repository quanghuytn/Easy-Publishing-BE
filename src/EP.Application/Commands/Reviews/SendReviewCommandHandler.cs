using EP.Application.Common;
using EP.Application.Common.DTOs.Review;
using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Services;
using EP.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EP.Application.Commands.Reviews
{
    public record SendReviewCommand : IRequest<ApiResponse<string>>
    {
        public int UserId { get; set; }
        public int ChapterId { get; set; }
        public bool SpellingError { get; set; }
        public bool LengthError { get; set; }
        public bool PoliticalContentError { get; set; }
        public bool DistortHistoryError { get; set; }
        public bool SecretContentError { get; set; }
        public bool OffensiveContentError { get; set; }
        public bool UnhealthyContentError { get; set; }
        public string? ReviewContent { get; set; }
    }
    public class SendReviewCommandHandler : IRequestHandler<SendReviewCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;
        public SendReviewCommandHandler(IUnitOfWork unitOfWork, IMailService mailService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }
        public async Task<ApiResponse<string>> Handle(SendReviewCommand request, CancellationToken cancellationToken)
        {
            var chapter = await _unitOfWork.ChapterRepository.GetByIdAsync(request.ChapterId);
            if (chapter == null)
            {
                return ApiResponse<string>.Failure("Chương không tồn tại.");
            }

            var story = await _unitOfWork.StoryRepository.GetByIdAsync(chapter.StoryId);

            if (story == null)
            {
                return ApiResponse<string>.Failure("Truyện không tồn tại.");
            }

            if (story.AuthorId == request.UserId)
            {
                return ApiResponse<string>.Failure("Bạn không được quyền review chương này.");
            }

            var review = await _unitOfWork.ReviewRepository.FindAsync(r => r.ChapterId == request.ChapterId);
            if (review != null)
            {
                return ApiResponse<string>.Failure("Chương đã được review.");
            }

            bool hasError = new[]
                    {
                        request.SpellingError,
                        request.LengthError,
                        request.PoliticalContentError,
                        request.DistortHistoryError,
                        request.SecretContentError,
                        request.OffensiveContentError,
                        request.UnhealthyContentError
                    }.Any(error => error);

            if (hasError && string.IsNullOrWhiteSpace(request.ReviewContent))
            {
                return ApiResponse<string>.Failure("Yêu cầu nhập nội dung review");
            }

            chapter.Status = hasError ? null : 1;
            if (!hasError && story.Status == 0)
            {
                story.Status = 1;
            }

            Review newReview = new Review()
            {
                UserId = request.UserId,
                ChapterId = request.ChapterId,
                ReviewDate = DateTime.Now,
                SpellingError = request.SpellingError,
                LengthError = request.LengthError,
                PoliticalContentError = request.PoliticalContentError,
                DistortHistoryError = request.DistortHistoryError,
                SecretContentError = request.SecretContentError,
                OffensiveContentError = request.OffensiveContentError,
                UnhealthyContentError = request.UnhealthyContentError,
                ReviewContent = request.ReviewContent
            };

            await _unitOfWork.ReviewRepository.AddAsync(newReview);

            // Gửi email thông báo cho tác giả truyện
            try
            {
                var link = "https://easy-publishing.vercel.app/author/review-a-chapter?mode=readOnly&storyId=" + story.StoryId + "&chapterId=" + chapter.ChapterId;
                _mailService.Send(story.Author.Email,
                        "Easy Publishing: Truyện của bạn đã được review",
                        "<p>Xin chào <b>" + story.Author.Username + "</b>,</p>" +
                        "<p>Chương <b>" + chapter.ChapterTitle + "</b> của Truyện <b>" + story.StoryTitle + "</b> của bạn đã được review.</p> " +
                        "<p>Chi tiết vui lòng truy cập:</p> " +
                        "<a href = " + link + ">Xem kết quả review</a>");
            }
            catch (Exception ex)
            {
                throw new Exception("Gửi review không thành công. Vui lòng thử lại sau.", ex);
            }

            var affectedRows = await _unitOfWork.CompleteAsync();
            if (affectedRows > 0)
            {
                return ApiResponse<string>.Success("Đã gửi review thành công.");
            }
            else
            {
                return ApiResponse<string>.Failure("Gửi review không thành công, vui lòng thử lại sau.");
            }
        }
    }
}
