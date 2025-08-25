using EP.Application.Common.DTOs.RefundRequest;
using EP.Application.Common.Interfaces.Repositories;
using EP.Domain.Models;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Repositories
{
    public class RefundRequestsRepository : Repository<RefundRequest>, IRefundRequestsRepository
    {
        public RefundRequestsRepository(Context context) : base(context)
        {
        }

        public async Task<IEnumerable<RefundRequestListDto>> GetAllRefundRequestsAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.Status == null)
                .Include(c => c.Wallet).ThenInclude(c => c.User)
                .Select(c => new RefundRequestListDto
                {
                    RequestId = c.RequestId,
                    UserFullname = c.Wallet.User.UserFullname,
                    WalletId = c.WalletId,
                    BankId = c.BankId,
                    BankAccount = c.BankAccount,
                    Amount = c.Amount * 1000,
                    RequestTime = c.RequestTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    ResponseTime = c.ResponseTime
                })
                .OrderByDescending(c => c.RequestId)
                .ToListAsync();
        }

        public async Task<IEnumerable<RefundRequest>> GetAllUnprocessedRequests()
        {
            return await _dbSet
                    .Where(c => c.ResponseTime != null && c.Status == null)
                    .Include(c => c.Wallet)
                        .ThenInclude(w => w.User)
                    .ToListAsync();
        }

        public async Task<IEnumerable<RefundExportDto>> GetInProgressRefundRequestExportAsync()
        {
            return await _dbSet 
                .AsNoTracking()
                .Where(c => c.ResponseTime != null && c.Status == null)
                .Include(c => c.Wallet).ThenInclude(c => c.User)
                .Select(c => new RefundExportDto
                {
                    UserFullname = c.Wallet.User.UserFullname,
                    BankId = c.BankId,
                    BankAccount = c.BankAccount,
                    Amount = ((int)c.Amount * 1000).ToString(),
                    RequestTime = c.RequestTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    ResponseTime = c.ResponseTime.Value.ToString("yyyy-MM-dd HH:mm:ss")
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<RefundRequest>> GetPendingRefundRequestExportAsync()
        {
            return await _dbSet
                    .Where(c => c.ResponseTime == null && c.Status == null)
                    .OrderByDescending(c => c.RequestId)
                    .ToListAsync();
        }
    }
}
