using EP.Application.Common.DTOs.Wallet;
using EP.Application.Common.Interfaces.Repositories;
using EP.Domain.Models;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Repositories
{
    public class WalletRepository : Repository<Wallet>, IWalletRepository
    {
        public WalletRepository(Context context) : base(context)
        {
        }

        public async Task<UserWalletDto?> GetUserWallet(int userId)
        {
            return await _dbSet
            .Where(w => w.UserId == userId)
            .Select(w => new UserWalletDto
            {
                WalletId = w.WalletId,
                UserId = w.UserId,
                Fund = w.Fund,
                Refund = w.Refund,
                AmountReceived = w.Transactions
                    .Where(t => t.RefundAfter > t.RefundBefore)
                    .Sum(t => (decimal?)t.Amount) ?? 0,
                AmountSpent = w.Transactions
                    .Where(t => t.FundAfter < t.FundBefore)
                    .Sum(t => (decimal?)t.Amount) ?? 0,
                AmountTopUp = w.Transactions
                    .Where(t => t.FundAfter > t.FundBefore)
                    .Sum(t => (decimal?)t.Amount) ?? 0,
                AmountWithdrawn = w.Transactions
                    .Where(t => t.RefundAfter < t.RefundBefore)
                    .Sum(t => (decimal?)t.Amount) ?? 0
            })
            .FirstOrDefaultAsync();
        }
    }
}
