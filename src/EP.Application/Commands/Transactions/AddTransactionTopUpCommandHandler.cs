using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using FluentValidation;
using MediatR;

namespace EP.Application.Commands.Transactions
{
    public record AddTransactionTopUpCommand : IRequest<int>
    {
        public int Amount { get; set; }
        public int UserId { get; set; }
    }

    public class AddTransactionTopUpCommandValidator : AbstractValidator<AddTransactionTopUpCommand>
    {
        public AddTransactionTopUpCommandValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("UserId is invalid.");
        }
    }
    public class AddTransactionTopUpCommandHandler : IRequestHandler<AddTransactionTopUpCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddTransactionTopUpCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<int> Handle(AddTransactionTopUpCommand request, CancellationToken cancellationToken)
        {
            var userWallet = await _unitOfWork.WalletRepository.FindAsync(w => w.UserId == request.UserId);
            var amount = request.Amount;

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
            userWallet.Fund +=amount;

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
                return amount;
            }
            else
            {
                throw new Exception("Nạp tiền thất bại. Vui lòng thử lại sau!");
            }
        }
    }
}
