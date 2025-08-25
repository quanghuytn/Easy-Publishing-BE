using EP.Application.Common;
using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using MediatR;

namespace EP.Application.Commands.Transactions
{
    public record PurchaseChaperCommand(int ChapterId, int UserId) : IRequest<ApiResponse<string>>;
    public class PurchaseChapterCommandHandler : IRequestHandler<PurchaseChaperCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public PurchaseChapterCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<string>> Handle(PurchaseChaperCommand request, CancellationToken cancellationToken)
        {
            var chapter = await _unitOfWork.ChapterRepository.GetByIdAsync(request.ChapterId);
            if (chapter == null) {
                throw new Exception("Hệ thống xảy ra lỗi. Vui lòng thử lại sau!");
            }

            var story = await _unitOfWork.StoryRepository.GetByIdAsync(chapter.StoryId);
            if (story == null)
            {
                throw new Exception("Hệ thống xảy ra lỗi. Vui lòng thử lại sau!");
            }

            var userInfo = await _unitOfWork.UserRepository.GetPurchaseInfoInStory(request.UserId, chapter.StoryId);
            if (userInfo == null)
            {
                throw new Exception("Hệ thống xảy ra lỗi. Vui lòng thử lại sau!");
            }

            if (userInfo.Wallet.Fund < chapter.ChapterPrice)
            {
                return ApiResponse<string>.Failure("Bạn không đủ TLT! Hãy nạp tiền");
            }

            if (request.UserId == story.AuthorId || userInfo.OwnedChapterIds.Contains(request.ChapterId) || userInfo.OwnedStoryIds.Contains(chapter.StoryId))
            {
                return ApiResponse<string>.Failure("Bạn đã sở hữu chương này!");
            }

            var userWallet = await _unitOfWork.WalletRepository.GetByIdAsync(userInfo.Wallet.WalletId);
            var authorWallet = await _unitOfWork.WalletRepository.FindAsync(w => w.UserId == story.AuthorId);

            var user_transaction = new Transaction
            {
                WalletId = userInfo.Wallet.WalletId,
                Amount = (decimal)chapter.ChapterPrice,
                StoryId = story.StoryId,
                ChapterId = chapter.ChapterId,
                FundBefore = userInfo.Wallet.Fund,
                FundAfter = userInfo.Wallet.Fund - (decimal)chapter.ChapterPrice,
                RefundAfter = 0,
                RefundBefore = 0,
                TransactionTime = DateTime.Now,
                Status = true,
                Description = $"Mua chương {chapter.ChapterNumber} của truyện {story.StoryTitle}"
            };
            var author_transaction = new Transaction
            {
                WalletId = authorWallet.WalletId,
                Amount = (decimal)chapter.ChapterPrice,
                StoryId = story.StoryId,
                ChapterId = chapter.ChapterId,
                FundBefore = 0,
                FundAfter = 0,
                RefundAfter = authorWallet.Refund + (decimal)chapter.ChapterPrice,
                RefundBefore = authorWallet.Refund,
                TransactionTime = DateTime.Now,
                Status = true,
                Description = $"Nhận TLT từ truyện {story.StoryTitle}"
            };

            userWallet.Fund -= (decimal)chapter.ChapterPrice;
            authorWallet.Refund += (decimal)chapter.ChapterPrice;

            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
            user.Chapters.Add(chapter);

            await _unitOfWork.TransactionRepository.AddAsync(author_transaction);
            await _unitOfWork.TransactionRepository.AddAsync(user_transaction);

            var affectedRows = await _unitOfWork.CompleteAsync();
            if (affectedRows > 0)
            {
                return ApiResponse<string>.Success($"Buy chapter {chapter.ChapterNumber} {chapter.ChapterTitle} in story {story.StoryTitle} successful");
            }
            else
            {
                return ApiResponse<string>.Failure($"Mua chương thất bại!. Vui lòng thử lại sau!");
            }
        }
    }
}
