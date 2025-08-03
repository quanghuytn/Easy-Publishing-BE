using EP.Application.Common;
using EP.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Commands.Reports
{
    public record ResolveReportCommand : IRequest<ApiResponse<string>>
    {
        public long ReportId { get; set; }
    }
    public class ResolveReportCommandHandler : IRequestHandler<ResolveReportCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ResolveReportCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<ApiResponse<string>> Handle(ResolveReportCommand request, CancellationToken cancellationToken)
        {
            var report = await _unitOfWork.ReportRepository.GetByIdAsync(request.ReportId);
            string message = "Resolved report successfully!";

            if (report.Status == null || report.Status == false)
            {
                report.Status = true;
            }
            else
            {
                message = "Unsolved report successfully!";
                report.Status = false;
            }

            await _unitOfWork.ReportRepository.UpdateAsync(report);
            var affectedRows = await _unitOfWork.CompleteAsync();
            if (affectedRows > 0)
            {
                return ApiResponse<string>.Success(message, "Report status updated successfully.");
            }
            else
            {
                return ApiResponse<string>.Failure("Failed to update report status.");
            }
        }
    }
}
