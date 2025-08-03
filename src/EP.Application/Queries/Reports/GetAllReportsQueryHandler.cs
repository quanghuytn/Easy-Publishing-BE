using EP.Application.Common;
using EP.Application.Common.DTOs.Report;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Queries.Reports
{
    public record GetAllReportsQuery : IRequest<ApiResponse<IEnumerable<ReportDetailDto>>>;
    public class GetAllReportsQueryHandler : IRequestHandler<GetAllReportsQuery, ApiResponse<IEnumerable<ReportDetailDto>>>
    {
        private readonly IReportRepository _reportRepository;
        public GetAllReportsQueryHandler(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
        }
        public async Task<ApiResponse<IEnumerable<ReportDetailDto>>> Handle(GetAllReportsQuery request, CancellationToken cancellationToken)
        {
            var reports = await _reportRepository.GetAllReports();

            return ApiResponse<IEnumerable<ReportDetailDto>>.Success("Danh sách báo cáo", reports);
        }
    }
}
