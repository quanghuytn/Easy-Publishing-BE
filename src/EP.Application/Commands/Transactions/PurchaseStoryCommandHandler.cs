using EP.Application.Common;
using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using MediatR;

namespace EP.Application.Commands.Transactions
{
    public record PurchaseStoryCommand(int StoryId, int UserId) : IRequest<ApiResponse<string>>;
    public class PurchaseStoryCommandHandler : IRequestHandler<PurchaseStoryCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public PurchaseStoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<string>> Handle(PurchaseStoryCommand request, CancellationToken cancellationToken)
        {
            var userInfo = await _unitOfWork.UserRepository.GetPurchaseInfoInStory(request.UserId, request.StoryId);
            if (userInfo == null) {
                throw new Exception("User is not available!");
            }

            var storyInfo = await _unitOfWork.StoryRepository.GetStoryPurchaseInfoAsync(request.StoryId);
            if (storyInfo == null) {
                throw new Exception("Story didn't exist!");
            }

            if (userInfo.Wallet.Fund < storyInfo.StoryPrice)
            {
                return ApiResponse<string>.Failure("Bạn không đủ TLT! Hãy nạp tiền");
            }

            if (request.UserId == storyInfo.AuthorId || userInfo.OwnedStoryIds.Contains(storyInfo.StoryId))
            {
                return ApiResponse<string>.Failure("Bạn đã sở hữu truyện này!");
            }

            var chapterIdList = await _unitOfWork.ChapterRepository.SelectWithConditionAsync(ch => ch.StoryId == storyInfo.StoryId, ch => ch.ChapterId);
            var chapterNotOwn = chapterIdList.Except(userInfo.OwnedChapterIds).ToList();
            if (chapterNotOwn.Count == 0)
            {
                return ApiResponse<string>.Failure("Bạn đã sở hữu hết các chương của truyện này!");
            }

            decimal salePercentage = storyInfo.StorySale ?? 0;
            decimal amount = storyInfo.StoryPrice - (storyInfo.StoryPrice * salePercentage / 100);

            var user_transaction = new Transaction
            {
                WalletId = userInfo.Wallet.WalletId,
                Amount = amount,
                StoryId = storyInfo.StoryId,
                FundBefore = userInfo.Wallet.Fund,
                FundAfter = userInfo.Wallet.Fund - amount,
                RefundAfter = 0,
                RefundBefore = 0,
                TransactionTime = DateTime.Now,
                Status = true,
                Description = $"Mua truyện {storyInfo.StoryTitle}"
            };

            var author_transaction = new Transaction
            {
                WalletId = storyInfo.AuthorWallet.WalletId,
                Amount = amount,
                StoryId = storyInfo.StoryId,
                FundBefore = 0,
                FundAfter = 0,
                RefundBefore = storyInfo.AuthorWallet.Refund,
                RefundAfter = storyInfo.AuthorWallet.Refund + amount,
                TransactionTime = DateTime.Now,
                Status = true,
                Description = $"Nhận TLT từ truyện {storyInfo.StoryTitle}"
            };

            var chaptersToAdd = await _unitOfWork.ChapterRepository
                .FindManyAsTrackingAsync(ch => chapterNotOwn.Contains(ch.ChapterId));
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
            var story = await _unitOfWork.StoryRepository.GetByIdAsync(request.StoryId);

            foreach (var chapter in chaptersToAdd)
            {
                user.Chapters.Add(chapter);
            }

            user.StoriesNavigation.Add(story);

            var userWallet = await _unitOfWork.WalletRepository.GetByIdAsync(userInfo.Wallet.WalletId);
            var authorWallet = await _unitOfWork.WalletRepository.GetByIdAsync(storyInfo.AuthorWallet.WalletId);
            userWallet.Fund -= amount;
            authorWallet.Refund += amount;

            await _unitOfWork.TransactionRepository.AddAsync(author_transaction);
            await _unitOfWork.TransactionRepository.AddAsync(user_transaction);

            var affectedRows = await _unitOfWork.CompleteAsync();
            if (affectedRows > 0)
            {
                return ApiResponse<string>.Success($"Mua truyện {storyInfo.StoryTitle} thành công!");
            }
            else
            {
                return ApiResponse<string>.Failure($"Mua truyện {storyInfo.StoryTitle} thất bại!.");
            }
        }
    }
}
