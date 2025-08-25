using EP.Application.Common.DTOs.Wallet;

namespace EP.Application.Common.DTOs.User
{
    public class UserPurchaseInfoDto
    {
        public int UserId { get; set; }
        public List<int> OwnedStoryIds { get; set; }
        public List<long> OwnedChapterIds { get; set; }
        public UserWalletDto? Wallet { get; set; }
    }
}
