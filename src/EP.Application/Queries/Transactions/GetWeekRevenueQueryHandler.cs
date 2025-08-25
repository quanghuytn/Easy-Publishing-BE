using EP.Application.Common;
using EP.Application.Common.DTOs.Transaction;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Transactions
{
    public class GetWeekRevenueQuery() : IRequest<ApiResponse<GetWeekRevenueDto>>;
    public class GetWeekRevenueQueryHandler
    : IRequestHandler<GetWeekRevenueQuery, ApiResponse<GetWeekRevenueDto>>
    {
        private readonly ITransactionRepository _transactionRepository;

        public GetWeekRevenueQueryHandler(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<ApiResponse<GetWeekRevenueDto>> Handle(
            GetWeekRevenueQuery request,
            CancellationToken cancellationToken)
        {
            var today = DateTime.Today;
            var startDate = today.AddDays(-6);
            var endDate = today.AddDays(1).AddTicks(-1);

            var revenues = await _transactionRepository
                .GetDailyRevenueAsync(startDate, endDate, cancellationToken);

            var data = new GetWeekRevenueDto();

            for (int i = -6; i <= 0; i++)
            {
                var date = today.AddDays(i).Date;
                data.Labels.Add(date.ToString("dd/MM"));

                var dayRevenue = revenues.FirstOrDefault(r => r.Date == date);
                data.Data.Add(dayRevenue.Total);
            }

            return ApiResponse<GetWeekRevenueDto>.Success("Doanh thu trong 7 ngày", data);
        }
    }
}
