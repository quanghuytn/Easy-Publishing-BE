using EP.Application.Common.Interfaces.Services;
using FluentValidation;
using MediatR;

namespace EP.Application.Commands.Transactions
{
    public record SendVNPayRequestCommand : IRequest<string>
    {
        public DateTime PaymentDate { get; init; }
        public DateTime ExpireDate { get; init; }
        public decimal RequiredAmount { get; init; }
        public string PaymentCurrency { get; init; }
        public string PaymentContent { get; init; }
    }

    public class SendVNPayRequestCommandValidator : AbstractValidator<SendVNPayRequestCommand>
    {
        public SendVNPayRequestCommandValidator()
        {
            RuleFor(x => x.PaymentDate)
                .NotEmpty().WithMessage("PaymentDate is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("PaymentDate cannot be in the future.");

            RuleFor(x => x.ExpireDate)
                .NotEmpty().WithMessage("ExpireDate is required.")
                .GreaterThan(x => x.PaymentDate).WithMessage("ExpireDate must be later than PaymentDate.");

            RuleFor(x => x.RequiredAmount)
                .GreaterThan(0).WithMessage("RequiredAmount must be greater than 0.");

            RuleFor(x => x.PaymentCurrency)
                .NotEmpty().WithMessage("PaymentCurrency is required.")
                .MaximumLength(3).WithMessage("PaymentCurrency must not exceed 3 characters.");

            RuleFor(x => x.PaymentContent)
                .NotEmpty().WithMessage("PaymentContent is required.")
                .MaximumLength(200).WithMessage("PaymentContent must not exceed 200 characters.");
        }
    }
    public class SendVNPayRequestCommandHandler : IRequestHandler<SendVNPayRequestCommand, string>
    {
        private readonly IVNPayService _vnPayService;
        public SendVNPayRequestCommandHandler(IVNPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        public Task<string> Handle(SendVNPayRequestCommand request, CancellationToken cancellationToken)
        {
            var paymentUrl = _vnPayService.CreatePaymentRequest(request.RequiredAmount, request.PaymentCurrency, request.PaymentContent, request.PaymentDate, request.ExpireDate);

            return Task.FromResult(paymentUrl);
        }
    }
}
