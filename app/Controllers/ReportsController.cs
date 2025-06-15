using app.DTOs.Report;
using app.Interface;
using app.Models;
using app.Service;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace app.Controllers
{
    [Route("api/v1/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IReportRepository _reportRepo;
        private MsgService _msgService = new MsgService();
        public ReportsController(IReportRepository reportRepository)
        {
            _reportRepo = reportRepository;
        }

        [HttpGet("options")]
        public async Task<ActionResult> GetReportType()
        {
            var types = await _reportRepo.GetReportTypes();
            return _msgService.MsgReturn(0, "Các loại báo cáo", types);
        }

        [HttpGet("all_report")]
        public async Task<ActionResult> GetAllReports()
        {
            var reports = await _reportRepo.GetAllReports();
            return _msgService.MsgReturn(0, "Thể loại tố cáo", reports);
        }

        [HttpGet("report/{id}")]
        public async Task<ActionResult> GetReport(int id)
        {
            var report = await _reportRepo.GetReportById(id);
            return _msgService.MsgReturn(0, "Get Report", report);
        }

        [Authorize]
        [HttpPost("send")]
        public async Task<ActionResult> SendReport(SendReportDTO newReport)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!ModelState.IsValid) return _msgService.MsgActionReturn(-1, "Thiếu điều kiện");
            
            try
            {
                var result = await _reportRepo.SendReport(userId, newReport);
                if (!result)
                {
                    return _msgService.MsgActionReturn(-1, "Đối tượng không tồn tại");
                }
            }
            catch (Exception)
            {
                return _msgService.MsgActionReturn(-4, "Hệ thống xảy ra lỗi!");
            }
            return _msgService.MsgActionReturn(0, "Báo cáo thành công");
        }

        [HttpPut("resolveReport")]
        public async Task<ActionResult> SwitchStatus(int id)
        {
            try
            {
                string msg = await _reportRepo.ChangeReportStatus(id);
                return new JsonResult(new
                {
                    EC = 0,
                    EM = msg
                });
            }
            catch (Exception)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Hệ thống xảy ra lỗi!"
                });
            }
        }
    }
}
