using EP.Application.Common.DTOs.RefundRequest;
using EP.Domain.Models;

namespace EP.Application.Common.Interfaces.Repositories
{
    public interface IRefundRequestsRepository : IRepository<RefundRequest>
    {
        Task<IEnumerable<RefundRequestListDto>> GetAllRefundRequestsAsync();
        Task<IEnumerable<RefundRequest>> GetAllUnprocessedRequests();
        Task<IEnumerable<RefundRequest>> GetPendingRefundRequestExportAsync();
        Task<IEnumerable<RefundExportDto>> GetInProgressRefundRequestExportAsync();
    }
}
