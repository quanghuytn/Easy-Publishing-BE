using EP.Application.Commands.RefundRequests;
using EP.Application.Commands.Tickets;
using EP.Application.Queries.RefundRequests;
using EP.Application.Queries.Tickets;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace app.Controllers
{
    [Authorize]
    [Route("api/v1/tickets")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TicketsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all_ticket")]
        public async Task<ActionResult> GetAllTickets()
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var query = new GetAllTicketQuery();
            var response = await _mediator.Send(query);
            
            return Ok(response);
        }

        [Authorize]
        [HttpPost("send")]
        public async Task<ActionResult> SendRequest()
        {

            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var command = new SendReviewerRequestCommand(userId);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("invite_interview")]
        public async Task<ActionResult> ApproveRequest([FromBody] InviteReviewerInterviewCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("approve")]
        public async Task<ActionResult> ApproveRequest([FromBody] ApproveReviewerRequestCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("deny")]
        public async Task<ActionResult> DenyRequest([FromBody] DenyReviewerRequestCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [HttpPost("refund_send")]
        public async Task<ActionResult> SendRefund([FromBody] SendRefundRequestCommand command)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            command.UserId = userId;
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("refunds")]
        public async Task<ActionResult> GetAllRefund()
        {
            var query = new GetAllRefundRequestQuery();
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("refund_export")]
        public async Task<ActionResult> ExportRefunds()
        {
            var query = new GetPendingRefundRequestQuery();
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        //[Authorize(Roles = "Admin")]
        //[HttpGet("refund_export2")]
        //public async Task<ActionResult> ExportRefunds2()
        //{
        //    var requests = await _context.RefundRequests
        //       .Where(c => c.ResponseTime == null && c.Status == null)
        //       .Include(c => c.Wallet).ThenInclude(c => c.User)
        //       .OrderByDescending(c => c.RequestId)
        //       .ToListAsync();

        //    if (requests.Count() == 0) return _msgService.MsgActionReturn(-2, "Yêu cầu đã được phê duyệt rồi");
        //    requests.ForEach(request => request.ResponseTime = DateTime.Now);
        //    _context.RefundRequests.UpdateRange(requests);
        //    await _context.SaveChangesAsync();

        //    var stream = new MemoryStream();

        //    using (var package = new ExcelPackage(stream))
        //    {
        //        var worksheet = package.Workbook.Worksheets.Add("Refund_Request");

        //        // Headers
        //        worksheet.Cells[1, 1].Value = "UserFullname";
        //        worksheet.Cells[1, 2].Value = "BankId";
        //        worksheet.Cells[1, 3].Value = "BankAccount";
        //        worksheet.Cells[1, 4].Value = "Amount";
        //        worksheet.Cells[1, 5].Value = "RequestTime";


        //        // Data
        //        for (int i = 0; i < requests.Count; i++)
        //        {
        //            worksheet.Cells[i + 2, 1].Value = requests[i].Wallet.User.UserFullname;
        //            worksheet.Cells[i + 2, 2].Value = requests[i].BankId;
        //            worksheet.Cells[i + 2, 3].Value = requests[i].BankAccount;
        //            worksheet.Cells[i + 2, 4].Value = requests[i].Amount * 1000;
        //            worksheet.Cells[i + 2, 5].Value = requests[i].RequestTime.ToString("yyyy-MM-dd HH:mm:ss");
        //        }
        //        package.Save();
        //    }

        //    stream.Position = 0;
        //    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Refund_Request.xlsx");
        //}

        [Authorize(Roles = "Admin")]
        [HttpPut("refund_approve")]
        public async Task<ActionResult> ApproveRefund()
        {
            var command = new ApproveRefundRequestCommand();
            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}
