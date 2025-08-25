using EP.Application.Common.DTOs.Story;
using EP.Application.Common.DTOs.Transaction;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Pagination;
using EP.Domain.Constants;
using EP.Domain.Models;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Repositories
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(Context context) : base(context)
        {
        }

        public async Task<PaginatedResult<TransactionDto>> GetUserTransactionHistory(int userId, int page, int pageSize)
        {
            var baseQuery = _dbSet
                .AsNoTracking()
                .Where(c => c.Wallet.UserId == userId);

            int totalCount = await baseQuery.CountAsync();

            var transactions = await baseQuery
                .OrderByDescending(c => c.TransactionId).ThenByDescending(c => c.TransactionTime)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(t => new TransactionDto
                {
                    TransactionId = t.TransactionId,
                    Amount = (int)t.Amount,
                    StoryTitle = t.Story.StoryTitle,
                    ChapterTitle = t.Chapter.ChapterTitle,
                    FundBefore = (int)t.FundBefore,
                    FundAfter = (int)t.FundAfter,
                    RefundAfter = (int)t.RefundAfter,
                    RefundBefore = (int)t.RefundBefore,
                    TransactionTime = t.TransactionTime,
                    Status = t.Status,
                    Description = t.Description
                })
                .ToListAsync();

            return new PaginatedResult<TransactionDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: transactions);
        }

        public async Task<IEnumerable<AdminTransactionDto>> GetAdminTransactionHistory()
        {
            return await _dbSet
                    .Include(t => t.Wallet)
                    .ThenInclude(w => w.User)
                    .Where(t => t.Wallet.UserId == SystemConstants.AdminUserId)
                    .OrderByDescending(t => t.TransactionId)
                    .Select(t => new AdminTransactionDto
                    {
                        Username = t.Wallet.User.Username,
                        TransactionId = t.TransactionId,
                        Amount = t.Amount * 1000,
                        RefundBefore = t.RefundBefore,
                        RefundAfter = t.RefundAfter,
                        TransactionTime = t.TransactionTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        Status = (bool)t.Status ? "Success" : "Failed",
                        Description = t.Description
                    })
                    .ToListAsync();
        }

        public async Task<List<(DateTime Date, decimal Total)>> GetDailyRevenueAsync(
                    DateTime startDate,
                    DateTime endDate,
                    CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(t => t.TransactionTime >= startDate
                            && t.TransactionTime <= endDate
                            && t.FundAfter > t.FundBefore)
                .GroupBy(t => t.TransactionTime.Date)
                .Select(g => new ValueTuple<DateTime, decimal>(
                    g.Key,
                    g.Sum(t => t.Amount) * 1000
                ))
                .ToListAsync(cancellationToken);
        }

        public async Task<decimal> GetTotalAmountForDayAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var startOfDay = date.Date;
            var endOfDay = date.Date.AddDays(1).AddTicks(-1);

            var total = await _dbSet
                .Where(t => t.TransactionTime >= startOfDay
                            && t.TransactionTime <= endOfDay
                            && t.FundAfter > t.FundBefore)
                .SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0;

            return total * 1000;
        }

        public async Task<decimal> GetOverallRevenueAsync(CancellationToken cancellationToken = default)
        {
            var total = await _dbSet
                .Where(t => t.FundAfter > t.FundBefore)
                .SumAsync(t => (decimal?)t.Amount, cancellationToken) ?? 0;

            return total * 1000;
        }
    }
}
