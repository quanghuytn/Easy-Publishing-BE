using EP.Application.Common;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Transactions
{
    public class GetTodayRevenueQuery() : IRequest<ApiResponse<decimal>>;
    public class GetTodayRevenueQueryHandler : IRequestHandler<GetTodayRevenueQuery, ApiResponse<decimal>>
    {
        private readonly ITransactionRepository _repository;

        public GetTodayRevenueQueryHandler(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public async Task<ApiResponse<decimal>> Handle(GetTodayRevenueQuery request, CancellationToken cancellationToken)
        {
            var totalRevenue = await _repository.GetTotalAmountForDayAsync(DateTime.Today, cancellationToken);

            return ApiResponse<decimal>.Success("Doanh thu hôm nay", totalRevenue);
        }
    }
}
