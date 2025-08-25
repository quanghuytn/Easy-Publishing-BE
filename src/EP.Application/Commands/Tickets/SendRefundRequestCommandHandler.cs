using EP.Application.Common;
using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using FluentValidation;
using MediatR;

namespace EP.Application.Commands.Tickets
{
    public record SendRefundRequestCommand : IRequest<ApiResponse<string>>
    {
        public int UserId { get; set; }
        public string BankId { get; set; }
        public string BankAccount { get; set; }
        public decimal Amount { get; set; }
    }
    public class SendRefundRequestCommandValidator : AbstractValidator<SendRefundRequestCommand>
    {
        public SendRefundRequestCommandValidator()
        {
            RuleFor(x => x.BankId)
                .NotEmpty().WithMessage("BankId is required.")
                .MaximumLength(50).WithMessage("BankId cannot exceed 50 characters.");

            RuleFor(x => x.BankAccount)
                .NotEmpty().WithMessage("BankAccount is required.")
                .MaximumLength(100).WithMessage("BankAccount cannot exceed 100 characters.")
                .Matches(@"^[0-9-]+$").WithMessage("BankAccount must contain only numbers and hyphens.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");
        }
    }

    public class SendRefundRequestCommandHandler : IRequestHandler<SendRefundRequestCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public SendRefundRequestCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<string>> Handle(SendRefundRequestCommand request, CancellationToken cancellationToken)
        {
            var wallet = await _unitOfWork.WalletRepository
                        .FindAsync(w => w.UserId == request.UserId);

            if (wallet == null)
                return ApiResponse<string>.Failure("Không tìm thấy ví của bạn");

            if (request.Amount < 100)
                return ApiResponse<string>.Failure("Rút tối thiểu 100 TLT!");

            if (request.Amount > wallet.Refund)
                return ApiResponse<string>.Failure("Bạn không đủ số dư!");

            var existingRequest = await _unitOfWork.RefundRequestsRepository
                                .FindAsync(r => r.WalletId == wallet.WalletId && r.Status == null);

            if (existingRequest != null)
                return ApiResponse<string>.Failure("Yêu cầu trước đó của bạn vẫn đang xử lý!");

            var refundRequest = new RefundRequest
            {
                WalletId = wallet.WalletId,
                BankId = request.BankId,
                BankAccount = request.BankAccount,
                Amount = request.Amount,
                RequestTime = DateTime.Now
            };

            await _unitOfWork.RefundRequestsRepository.AddAsync(refundRequest);
            var affectedRows = await _unitOfWork.CompleteAsync();
            if (affectedRows > 0)
            {
                return ApiResponse<string>.Success("Yêu cầu rút tiền của bạn đã được gửi đi!");
            }
            else
            {
                return ApiResponse<string>.Failure("Hệ thống xảy ra lỗi. Vui lòng thử lại sau!");
            }
        }
    }
}
