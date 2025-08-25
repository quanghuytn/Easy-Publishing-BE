using EP.Application.Common;
using EP.Application.Common.DTOs.Transaction;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Transactions
{
    public record GetAdminTransactionHistoryQuery() : IRequest<ApiResponse<IEnumerable<AdminTransactionDto>>>;
    public class GetAdminTransactionHistoryQueryHandler : IRequestHandler<GetAdminTransactionHistoryQuery, ApiResponse<IEnumerable<AdminTransactionDto>>>
    {
        private readonly ITransactionRepository _transactionRepository;
        public GetAdminTransactionHistoryQueryHandler(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
        public async Task<ApiResponse<IEnumerable<AdminTransactionDto>>> Handle(GetAdminTransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            var data = await _transactionRepository.GetAdminTransactionHistory();

            return ApiResponse<IEnumerable<AdminTransactionDto>>.Success("Tiền ra vào hệ thống", data);
        }
    }
}
