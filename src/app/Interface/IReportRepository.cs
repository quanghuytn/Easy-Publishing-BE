using app.DTOs.Report;
using app.Models;

namespace app.Interface
{
    public interface IReportRepository
    {
        Task<List<ReportType>> GetReportTypes();
        Task<List<ReportDetailDto>> GetAllReports();
        Task<ReportDto?> GetReportById(int id);
        Task<bool> SendReport(int userId, SendReportDTO newReport);
        Task<string> ChangeReportStatus(int id);
    }
}
