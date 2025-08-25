using EP.Application.Common;
using EP.Application.Common.DTOs.Transaction;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Pagination;
using MediatR;

namespace EP.Application.Queries.Transactions
{
    public record GetUserTransactionHistoryQuery : IRequest<ApiResponse<PaginatedResult<TransactionDto>>>
    {
        public int UserId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
    public class GetUserTransactionHistoryQueryHandler : IRequestHandler<GetUserTransactionHistoryQuery, ApiResponse<PaginatedResult<TransactionDto>>>
    {
        private readonly ITransactionRepository _transactionRepository;
        public GetUserTransactionHistoryQueryHandler(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }
        public async Task<ApiResponse<PaginatedResult<TransactionDto>>> Handle(GetUserTransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            var data = await _transactionRepository.GetUserTransactionHistory(request.UserId, request.Page, request.PageSize);

            return ApiResponse<PaginatedResult<TransactionDto>>.Success("Get user transaction history successfully", data);
        }
    }
}
