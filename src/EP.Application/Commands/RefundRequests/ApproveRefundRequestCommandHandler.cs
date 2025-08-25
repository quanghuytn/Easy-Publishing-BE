using EP.Application.Common;
using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Services;
using EP.Domain.Models;
using MediatR;

namespace EP.Application.Commands.RefundRequests
{
    public record ApproveRefundRequestCommand : IRequest<ApiResponse<string>>;
    public class ApproveRefundRequestCommandHandler : IRequestHandler<ApproveRefundRequestCommand, ApiResponse<string>>
    {
        private readonly IMailService _mailService;
        private readonly IUnitOfWork _unitOfWork;
        public ApproveRefundRequestCommandHandler(IUnitOfWork unitOfWork, IMailService mailService)
        {
            _unitOfWork = unitOfWork;
            _mailService = mailService;
        }
        public async Task<ApiResponse<string>> Handle(ApproveRefundRequestCommand request, CancellationToken cancellationToken)
        {
            var refundRequests = await _unitOfWork.RefundRequestsRepository.GetAllUnprocessedRequests();
            if (!refundRequests.Any())
                return ApiResponse<string>.Failure("Yêu cầu đã được phê duyệt rồi");

            var adminWallet = await _unitOfWork.WalletRepository.FindAsync(u => 1 == 1);
            foreach(var refundRequest in refundRequests)
            {
                var userWallet = refundRequest.Wallet;
                var user = userWallet.User;

                refundRequest.Status = true;
                refundRequest.ResponseTime = DateTime.Now;

                var userTransaction = new Transaction
                {
                    WalletId = userWallet.WalletId,
                    Amount = refundRequest.Amount,
                    FundBefore = 0,
                    FundAfter = 0,
                    RefundBefore = userWallet.Refund,
                    RefundAfter = userWallet.Refund - refundRequest.Amount,
                    TransactionTime = DateTime.Now,
                    Status = true,
                    Description = $"Rút {refundRequest.Amount}"
                };
                userWallet.Refund -= refundRequest.Amount;

                var adminTransaction = new Transaction
                {
                    WalletId = adminWallet.WalletId,
                    Amount = refundRequest.Amount,
                    FundBefore = 0,
                    FundAfter = 0,
                    RefundBefore = adminWallet.Refund,
                    RefundAfter = adminWallet.Refund - refundRequest.Amount,
                    TransactionTime = DateTime.Now,
                    Status = true,
                    Description = $"Rút {refundRequest.Amount} khỏi hệ thống"
                };
                adminWallet.Refund -= refundRequest.Amount;

                try
                {
                    _mailService.Send(user.Email,
                            "Yêu cầu rút tiền của bạn đã được phê duyệt",
                            "<p>Easy Publishing Xin chào <b> " + user.UserFullname + "</b>,</p>" +
                            "<b>Thông tin giao dịch Quý khách vừa thực hiện như sau:</b>" +
                            "<p>Ngân hàng: <b>" + refundRequest.BankId + "</b></p>" +
                            "<p>Số thẻ: <b>" + refundRequest.BankAccount + "</b></p>" +
                            "<p>Giao dịch: <b>Rút tiền khỏi hệ thống</b> </p>" +
                            "<p>Trạng thái giao dịch: <b>Thành công</b> </p>" +
                            "<p>Số tiền giao dịch: <b>" + (int)refundRequest.Amount + " TLT</b></p>" +
                            "<p>Số tiền sau quy đổi: <b>" + (int)refundRequest.Amount + ".000đ</b></p>" +
                            "<p>Số tiền giao dịch nhận được: <b>" + (int)(refundRequest.Amount * (decimal)0.85) + ".000đ</b></p>" +
                            "<p>Vào lúc: <b>" + DateTime.Now + "</b></p>" +
                            "<p>Cảm ơn bạn đã tin tưởng.</p>");
                }
                catch (Exception ex)
                {
                    throw new Exception("Hệ thống xảy ra lỗi", ex);
                }

                await _unitOfWork.TransactionRepository.AddAsync(userTransaction);
                await _unitOfWork.TransactionRepository.AddAsync(adminTransaction);
            }

            var affectedRows = await _unitOfWork.CompleteAsync();
            if (affectedRows > 0)
            {
                return ApiResponse<string>.Success("Phê duyệt rút tiền thành công");
            }
            else
            {
                return ApiResponse<string>.Failure("Phê duyệt rút tiền thất bại!. Vui lòng thử lại sau");
            }
        }
    }
}
