using EP.Application.Common;
using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using EP.Domain.Payment;
using MediatR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EP.Application.Commands.Transactions
{
    public record ProcessMomoTransactionCommand(MomoIPNRequest MomoIPNRequest, int UserId) : IRequest<ApiResponse<string>>;
    public class ProcessMomoTransactionCommandHandler : IRequestHandler<ProcessMomoTransactionCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProcessMomoTransactionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<string>> Handle(ProcessMomoTransactionCommand request, CancellationToken cancellationToken)
        {
            var momoRequest = request.MomoIPNRequest;
            var userWallet = await _unitOfWork.WalletRepository.FindAsync(w => w.UserId == request.UserId);
            var amount = momoRequest.Amount;

            var user_transaction = new Transaction
            {
                WalletId = userWallet.WalletId,
                Amount = amount,
                FundBefore = userWallet.Fund,
                FundAfter = userWallet.Fund + amount,
                RefundBefore = 0,
                RefundAfter = 0,
                TransactionTime = DateTime.Now,
                Status = true,
                Description = $"Nạp {amount}000 VND"
            };
            userWallet.Fund += amount;

            var adminWallet = await _unitOfWork.WalletRepository.FindAsync(w => true);
            var admin_transaction = new Transaction
            {
                WalletId = adminWallet.WalletId,
                Amount = amount,
                FundBefore = 0,
                FundAfter = 0,
                RefundBefore = adminWallet.Refund,
                RefundAfter = adminWallet.Refund + amount,
                TransactionTime = DateTime.Now,
                Status = true,
                Description = $"Nạp {amount}000 VND vào hệ thống"
            };
            adminWallet.Fund += amount;

            await _unitOfWork.TransactionRepository.AddAsync(admin_transaction);
            await _unitOfWork.TransactionRepository.AddAsync(user_transaction);

            var affectedRows = await _unitOfWork.CompleteAsync();
            if (affectedRows > 0)
            {
                return ApiResponse<string>.Success("Nạp tiền thành công");
            }
            else
            {
                return ApiResponse<string>.Failure("Hệ thống xảy ra lỗi!");
            }
        }
    }
}
