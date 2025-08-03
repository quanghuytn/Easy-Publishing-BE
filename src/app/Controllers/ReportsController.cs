using EP.Application.Commands.Reports;
using EP.Application.Common.DTOs.Report;
using EP.Application.Queries.Reports;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace app.Controllers
{
    [Route("api/v1/reports")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ReportsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("options")]
        public async Task<ActionResult> GetReportType()
        {
            var query = new GetReportTypesQuery();
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("all_report")]
        public async Task<ActionResult> GetAllReports()
        {
            var query = new GetAllReportsQuery();
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [HttpGet("report/{id}")]
        public async Task<ActionResult> GetReport(int id)
        {
            var query = new GetReportByIdQuery(id);
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("send")]
        public async Task<ActionResult> SendReport(SendReportDto newReport)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var command = new SendReportCommand
            {
                UserId = userId,
                ReportTypeId = newReport.ReportTypeId,
                StoryId = newReport.StoryId,
                ChapterId = newReport.ChapterId,
                CommentId = newReport.CommentId,
                ReportContent = newReport.ReportContent
            };
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("resolveReport")]
        public async Task<ActionResult> SwitchStatus(int id)
        {
            var command = new ResolveReportCommand
            {
                ReportId = id
            };
            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}
