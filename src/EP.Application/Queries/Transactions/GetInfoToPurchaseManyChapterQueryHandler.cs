using EP.Application.Commands.Transactions;
using EP.Application.Common;
using EP.Application.Common.DTOs.Transaction;
using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using FluentValidation;
using MediatR;
using System;

namespace EP.Application.Queries.Transactions
{
    public record GetInfoToPurchaseManyChapterQuery : IRequest<ApiResponse<GetInforPurchaseManyChaptersResponseDto>>
    {
        public int ChapterStart { get; set; }
        public int ChapterEnd { get; set; }
        public int StoryId { get; set; }
        public int UserId { get; set; }
    }

    public class GetInfoToPurchaseManyChapterQueryValidator : AbstractValidator<GetInfoToPurchaseManyChapterQuery>
    {
        public GetInfoToPurchaseManyChapterQueryValidator()
        {
            RuleFor(x => x.ChapterStart).GreaterThan(0).WithMessage("ChapterStart must be greater than 0.");

            RuleFor(x => x.ChapterEnd)
                .GreaterThan(0).WithMessage("ChapterEnd must be greater than 0.")
                .GreaterThanOrEqualTo(x => x.ChapterStart).WithMessage("Chương cuối phải lớn hơn hoặc bằng chương bắt đầu.");

            RuleFor(x => x.StoryId)
                .GreaterThan(0).WithMessage("StoryId is invalid.");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId is invalid.");
        }
    }
    public class GetInfoToPurchaseManyChapterQueryHandler : IRequestHandler<GetInfoToPurchaseManyChapterQuery, ApiResponse<GetInforPurchaseManyChaptersResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetInfoToPurchaseManyChapterQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<GetInforPurchaseManyChaptersResponseDto>> Handle(GetInfoToPurchaseManyChapterQuery request, CancellationToken cancellationToken)
        {
            var chapterToPurchaseList = await _unitOfWork.ChapterRepository
                .FindManyAsTrackingAsync(ch => ch.ChapterNumber >= request.ChapterStart && ch.ChapterNumber <= request.ChapterEnd && ch.StoryId == request.StoryId && ch.Status > 0);

            var userInfo = await _unitOfWork.UserRepository.GetPurchaseInfoInStory(request.UserId, request.StoryId);
            if (userInfo == null)
            {
                throw new Exception("Hệ thống xảy ra lỗi!. Vui lòng thử lại sau!");
            }

            var chapterNotOwnedToBuy = chapterToPurchaseList.Where(ch => !userInfo.OwnedChapterIds.Contains(ch.ChapterId)).ToList();
            if (chapterNotOwnedToBuy.Count == 0)
            {
                return ApiResponse<GetInforPurchaseManyChaptersResponseDto>.Failure("Bạn đã sở hữu các chương này!");
            }

            var story = await _unitOfWork.StoryRepository.GetByIdAsync(request.StoryId);
            if (request.UserId == story.AuthorId || userInfo.OwnedStoryIds.Contains(request.StoryId))
            {
                return ApiResponse<GetInforPurchaseManyChaptersResponseDto>.Failure("Bạn đã sở hữu truyện(chương) này!");
            }

            decimal amount = chapterNotOwnedToBuy.Sum(c => c.ChapterPrice) ?? 0;

            return ApiResponse<GetInforPurchaseManyChaptersResponseDto>
                .Success("Thông tin giao dịch mua nhiều chương", new GetInforPurchaseManyChaptersResponseDto
                                                                {
                                                                    Number_chapter_buy = chapterToPurchaseList.Count(),
                                                                    Amount = amount,
                                                                    Balance = userInfo.Wallet.Fund
                                                                });
        }
    }
}
