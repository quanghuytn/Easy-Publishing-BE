using EP.Application.Common;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Transactions
{
    public class GetOverallRevenueQuery() : IRequest<ApiResponse<decimal>>;
    public class GetOverallRevenueQueryHandler : IRequestHandler<GetOverallRevenueQuery, ApiResponse<decimal>>
    {
        private readonly ITransactionRepository _repository;

        public GetOverallRevenueQueryHandler(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<decimal>> Handle(GetOverallRevenueQuery request, CancellationToken cancellationToken)
        {
            var totalRevenue = await _repository.GetOverallRevenueAsync(cancellationToken);

            return ApiResponse<decimal>.Success("Danh thu tổng", totalRevenue);
        }
    }
}
