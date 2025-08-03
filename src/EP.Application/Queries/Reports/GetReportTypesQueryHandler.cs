using EP.Application.Common;
using EP.Application.Common.Interfaces.Repositories;
using EP.Domain.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Queries.Reports
{
    public record GetReportTypesQuery() : IRequest<ApiResponse<IEnumerable<ReportType>>>;
    public class GetReportTypesQueryHandler : IRequestHandler<GetReportTypesQuery, ApiResponse<IEnumerable<ReportType>>>
    {
        private readonly IRepository<ReportType> _reportTypeRepository;
        public GetReportTypesQueryHandler(IRepository<ReportType> reportTypeRepository)
        {
            _reportTypeRepository = reportTypeRepository ?? throw new ArgumentNullException(nameof(reportTypeRepository));
        }
        public async Task<ApiResponse<IEnumerable<ReportType>>> Handle(GetReportTypesQuery request, CancellationToken cancellationToken)
        {
            var types = await _reportTypeRepository.GetAllAsync();

            return ApiResponse<IEnumerable<ReportType>>.Success("Các loại báo cáo", types);
        }
    }
}
