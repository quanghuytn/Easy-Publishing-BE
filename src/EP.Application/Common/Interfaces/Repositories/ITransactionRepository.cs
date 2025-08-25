using EP.Application.Common.DTOs.Transaction;
using EP.Application.Common.Pagination;
using EP.Domain.Models;

namespace EP.Application.Common.Interfaces.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<PaginatedResult<TransactionDto>> GetUserTransactionHistory(int userId, int page, int pageSize);
        Task<IEnumerable<AdminTransactionDto>> GetAdminTransactionHistory();
        Task<List<(DateTime Date, decimal Total)>> GetDailyRevenueAsync(
                                                            DateTime startDate,
                                                            DateTime endDate,
                                                            CancellationToken cancellationToken = default);
        Task<decimal> GetTotalAmountForDayAsync(DateTime date, CancellationToken cancellationToken = default);
        Task<decimal> GetOverallRevenueAsync(CancellationToken cancellationToken = default);
    }
}
