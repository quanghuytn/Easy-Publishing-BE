using EP.Application.Commands.Transactions;
using EP.Application.Common.DTOs.Payment;
using EP.Application.Queries.Transactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace app.Controllers
{
    [Route("api/v1/transaction")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("wallet")]
        public async Task<ActionResult> GetUserWallet()
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var query = new GetUserWalletQuery(userId);
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("purchase_story")]
        public async Task<ActionResult> AddTransactionBuyStory(int storyId)
        {
            
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var command = new PurchaseStoryCommand(storyId, userId);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("purchase_chapter")]
        public async Task<ActionResult> AddTransactionBuyChapter(int chapterId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var command = new PurchaseChaperCommand(chapterId, userId);
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [HttpPost("purchase_many_chapters")]
        public async Task<ActionResult> AddTransactionBuyManyChapters(int chapterStart, int chapterEnd, int storyId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var command = new PurchaseManyChaptersCommand
            {
                ChapterStart = chapterStart,
                ChapterEnd = chapterEnd,
                StoryId = storyId,
                UserId = userId
            };
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [HttpGet("get_information_to_buy_chapters")]
        public async Task<ActionResult> GetInformationToBuyChapter(int storyId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var query = new GetInformationToBuyChapterQuery(storyId, userId);
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("get_transaction_buy_many_chapters")]
        public async Task<ActionResult> GetTransactionBuyManyChapters(int chapterStart, int chapterEnd, int storyId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var query = new GetInfoToPurchaseManyChapterQuery
            {
                ChapterStart = chapterStart,
                ChapterEnd = chapterEnd,
                StoryId = storyId,
                UserId = userId
            };
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("top_up")]
        public async Task<ActionResult> AddTransactionTopUp(int amount)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var command = new AddTransactionTopUpCommand
            {
                Amount = amount,
                UserId = userId
            };
            var data = await _mediator.Send(command);

            return new JsonResult(new
            {
                EC = -1,
                EM = $"Nạp {data}000 VND thành  {data} TLT. Nạp tiền thành công!",
                DT = new { data }
            });
        }

        [Authorize]
        [HttpGet("history")]
        public async Task<ActionResult> GetUserTransactionHistory(int page, int pageSize)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var query = new GetUserTransactionHistoryQuery
            {
                UserId = userId,
                Page = page,
                PageSize = pageSize
            };  
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin_history")]
        public async Task<ActionResult> GetAdminTransactionHistory()
        {
            var query = new GetAdminTransactionHistoryQuery();
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [HttpGet("getTodayRevenue")]
        public async Task<ActionResult> GetTodayRevenue()
        {
            var query = new GetTodayRevenueQuery();
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [HttpGet("getOverallRevenue")]
        public async Task<ActionResult> GetOverallRevenue()
        {
            var query = new GetOverallRevenueQuery();
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [HttpGet("getRevenue")]
        public async Task<ActionResult> GetWeekRevenue()
        {
            var query = new GetWeekRevenueQuery();
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        //[HttpGet("export")]
        //public async Task<ActionResult> ExportOrdersToExcel(DateTime? fromDate, DateTime? toDate)
        //{
        //    try
        //    {
        //        var transaction = await _context.Transactions.Where(o => (!fromDate.HasValue || o.TransactionTime >= fromDate) && (!toDate.HasValue || o.TransactionTime <= toDate))
        //        .Include(t => t.Story)
        //        .Include(t => t.Chapter)
        //        .Select(t => new
        //        {
        //            TransactionId = t.TransactionId,
        //            Amount = t.Amount,
        //            StoryTitile = t.Story.StoryTitle,
        //            ChapterTitle = t.Chapter.ChapterTitle,
        //            FundBefore = t.FundBefore,
        //            FundAfter = t.FundAfter,
        //            RefundAfter = t.RefundAfter,
        //            RefundBefore = t.RefundBefore,
        //            TransactionTime = t.TransactionTime.ToString("dd/MM/yyyy"),
        //            Status = t.Status,
        //            Description = t.Description

        //        })
        //        .ToListAsync();

        //        var stream = new MemoryStream();

        //        using (var package = new ExcelPackage(stream))
        //        {
        //            var worksheet = package.Workbook.Worksheets.Add("Transactions");

        //            // Headers
        //            worksheet.Cells[1, 1].Value = "TransactionId";
        //            worksheet.Cells[1, 2].Value = "Amount";
        //            worksheet.Cells[1, 3].Value = "StoryTitile";
        //            worksheet.Cells[1, 4].Value = "ChapterTitle";
        //            worksheet.Cells[1, 5].Value = "FundBefore";
        //            worksheet.Cells[1, 6].Value = "FundAfter";
        //            worksheet.Cells[1, 7].Value = "RefundAfter";
        //            worksheet.Cells[1, 8].Value = "RefundBefore";
        //            worksheet.Cells[1, 9].Value = "TransactionTime";
        //            worksheet.Cells[1, 10].Value = "Status";
        //            worksheet.Cells[1, 11].Value = "Description";

        //            // Data
        //            for (int i = 0; i < transaction.Count; i++)
        //            {
        //                worksheet.Cells[i + 2, 1].Value = transaction[i].TransactionId;
        //                worksheet.Cells[i + 2, 2].Value = transaction[i].Amount;
        //                worksheet.Cells[i + 2, 3].Value = transaction[i].StoryTitile;
        //                worksheet.Cells[i + 2, 4].Value = transaction[i].ChapterTitle;
        //                worksheet.Cells[i + 2, 5].Value = transaction[i].FundBefore;
        //                worksheet.Cells[i + 2, 6].Value = transaction[i].FundAfter;
        //                worksheet.Cells[i + 2, 7].Value = transaction[i].RefundAfter;
        //                worksheet.Cells[i + 2, 8].Value = transaction[i].RefundBefore;
        //                worksheet.Cells[i + 2, 9].Value = transaction[i].TransactionTime;
        //                worksheet.Cells[i + 2, 10].Value = transaction[i].Status;
        //                worksheet.Cells[i + 2, 11].Value = transaction[i].Description;
        //            }
        //            package.Save();
        //        }

        //        stream.Position = 0;
        //        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Transaction.xlsx");

        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(new
        //        {
        //            EC = -1,
        //            EM = "Hệ thống xảy ra lỗi!"
        //        });
        //    }
        //}


        [HttpPost("vnpay_request")]
        public async Task<IActionResult> SendVNPayRequest([FromBody] SendVNPayRequestCommand command)
        {
            var paymentUrl = await _mediator.Send(command);

            return new JsonResult(new
            {
                EC = 0,
                EM = "Gửi request VNPay thành công",
                DT = new
                {
                    paymentUrl = paymentUrl
                }
            });
        }

        [Authorize]
        [HttpPost("momo_request")]
        public async Task<IActionResult> SendMomoRequest([FromBody] CreateMomoPaymentCommand command)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            command.UserId = userId;

            var (success, paymentUrl, message) = await _mediator.Send(command);

            if (success)
            {
                return new JsonResult(new
                {
                    EC = 0,
                    EM = message,
                    DT = new
                    {
                        paymentUrl = paymentUrl
                    }
                });
            }
            else
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = message
                });
            }
        }

        [HttpPost("notify")]
        public async Task<IActionResult> Notify([FromBody] MomoIPNRequest data)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (data.ResultCode != 0)
            {
                return BadRequest(new { EC = 1, EM = "Thanh toán thất bại" });
            }
            var command = new ProcessMomoTransactionCommand(data, userId);
            var response = await _mediator.Send(command);

            return Ok(response);
        }
    }
}
