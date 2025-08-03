using EP.Application.Common;
using EP.Application.Common.DTOs.Report;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Reports
{
    public record GetReportByIdQuery(int ReportId) : IRequest<ApiResponse<ReportDto>>;
    public class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQuery, ApiResponse<ReportDto>>
    {
        private readonly IReportRepository _reportRepository;
        public GetReportByIdQueryHandler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
        }
        public async Task<ApiResponse<ReportDto>> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
        {
            var report = await _reportRepository.GetReportById(request.ReportId);

            return ApiResponse<ReportDto>.Success("Chi tiết báo cáo", report);
        }
    }
}
