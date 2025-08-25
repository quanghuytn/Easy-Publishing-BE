using EP.Application.Common;
using EP.Application.Common.DTOs.Transaction;
using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using FluentValidation;
using MediatR;

namespace EP.Application.Commands.Transactions
{
    public record PurchaseManyChaptersCommand : IRequest<ApiResponse<PurchaseManyChapterResponseDto>>
    {
        public int ChapterStart { get; set; }
        public int ChapterEnd { get; set; }
        public int StoryId { get; set; }
        public int UserId { get; set; }
    }

    public class PurchaseManyChaptersCommandValidator : AbstractValidator<PurchaseManyChaptersCommand>
    {
        public PurchaseManyChaptersCommandValidator()
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
    public class PurchaseManyChaptersCommandHandler : IRequestHandler<PurchaseManyChaptersCommand, ApiResponse<PurchaseManyChapterResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public PurchaseManyChaptersCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<PurchaseManyChapterResponseDto>> Handle(PurchaseManyChaptersCommand request, CancellationToken cancellationToken)
        {
            if (request.ChapterStart > request.ChapterEnd)
            {
                return ApiResponse<PurchaseManyChapterResponseDto>.Failure("Chương cuối phải lớn hơn hoặc bằng chương bắt đầu.");
            }

            var chapterToPurchaseList = await _unitOfWork.ChapterRepository
                .FindManyAsTrackingAsync(ch => ch.ChapterNumber >= request.ChapterStart && ch.ChapterNumber <= request.ChapterEnd && ch.StoryId == request.StoryId && ch.Status > 0);

            var userInfo = await _unitOfWork.UserRepository.GetPurchaseInfoInStory(request.UserId, request.StoryId);
            if (userInfo == null)
            {
                throw new Exception("Hệ thống xảy ra lỗi!. Vui lòng thử lại sau!");
            }

            var chapterNotOwnedToBuy = chapterToPurchaseList.Where(ch => !userInfo.OwnedChapterIds.Contains(ch.ChapterId)).ToList();
            if (chapterNotOwnedToBuy.Count == 0) {
                return ApiResponse<PurchaseManyChapterResponseDto>.Failure("Bạn đã sở hữu các chương này!");
            }

            decimal amount = chapterNotOwnedToBuy.Sum(c => c.ChapterPrice) ?? 0;
            if (userInfo.Wallet.Fund < amount)
            {
                return ApiResponse<PurchaseManyChapterResponseDto>.Failure("Bạn không đủ TLT! Hãy nạp tiền");
            }

            var story = await _unitOfWork.StoryRepository.GetByIdAsync(request.StoryId);
            if (request.UserId == story.AuthorId || userInfo.OwnedStoryIds.Contains(story.StoryId))
            {
                return ApiResponse<PurchaseManyChapterResponseDto>.Failure("Bạn đã sở hữu truyện(chương) này!");
            }

            var userWallet = await _unitOfWork.WalletRepository.GetByIdAsync(userInfo.Wallet.WalletId);
            var authorWallet = await _unitOfWork.WalletRepository.FindAsync(w => w.UserId == story.AuthorId);

            var user_transaction = new Transaction
            {
                WalletId = userWallet.WalletId,
                Amount = amount,
                StoryId = story.StoryId,
                ChapterId = null,
                FundBefore = userWallet.Fund,
                FundAfter = userWallet.Fund - amount,
                RefundAfter = 0,
                RefundBefore = 0,
                TransactionTime = DateTime.Now,
                Status = true,
                Description = $"Mua {chapterToPurchaseList.Count()} chương của truyện {story.StoryTitle}"
            };
            var author_transaction = new Transaction
            {
                WalletId = authorWallet.WalletId,
                Amount = amount,
                StoryId = story.StoryId,
                ChapterId = null,
                FundBefore = 0,
                FundAfter = 0,
                RefundBefore = authorWallet.Refund,
                RefundAfter = authorWallet.Refund + amount,
                TransactionTime = DateTime.Now,
                Status = true,
                Description = $"Nhận TLT từ truyện {story.StoryTitle}"
            };

            userWallet.Fund -= amount;
            authorWallet.Refund += amount;

            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
            foreach (var chapter in chapterNotOwnedToBuy)
            {
                user!.Chapters.Add(chapter);
            }

            await _unitOfWork.TransactionRepository.AddAsync(author_transaction);
            await _unitOfWork.TransactionRepository.AddAsync(user_transaction);

            var affectedRows = await _unitOfWork.CompleteAsync();
            if (affectedRows > 0)
            {
                return ApiResponse<PurchaseManyChapterResponseDto>.Success("Bạn đã mua thành công", new PurchaseManyChapterResponseDto
                {
                    Amount = amount,
                    Chapter_buy = chapterToPurchaseList.Count()
                });
            }
            else
            {
                return ApiResponse<PurchaseManyChapterResponseDto>.Failure("Mua chương thất bại! Vui lòng thử lại sau!");
            }
        } 
    }
}
