using EP.Application.Common.DTOs.Report;
using EP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.Interfaces.Repositories
{
    public interface IReportRepository : IRepository<ReportContent>
    {
        Task<IEnumerable<ReportDetailDto>> GetAllReports();
        Task<ReportDto?> GetReportById(int id);
    }
}
