using EP.Application.Common;
using EP.Application.Common.Interfaces.Services.Payment;
using FluentValidation;
using MediatR;

namespace EP.Application.Commands.Transactions
{
    public record CreateMomoPaymentCommand : IRequest<(bool success, string paymentUrl, string message)>
    {
        public decimal RequiredAmount { get; init; }
        public string? PaymentContent { get; init; }
        public string? ExtraData { get; init; }
        public int UserId { get; set; }
    }

    public class CreateMomoPaymentCommandValidator : AbstractValidator<CreateMomoPaymentCommand>
    {
        public CreateMomoPaymentCommandValidator()
        {
            RuleFor(x => x.RequiredAmount)
                .GreaterThan(0)
                .WithMessage("Số tiền phải lớn hơn 0");

            RuleFor(x => x.PaymentContent)
                .NotEmpty()
                .WithMessage("Nội dung thanh toán không được để trống")
                .MaximumLength(255)
                .WithMessage("Nội dung thanh toán không vượt quá 255 ký tự");
        }
    }
    public class CreateMomoPaymentCommandHandler : IRequestHandler<CreateMomoPaymentCommand, (bool success, string paymentUrl, string message)>
    {
        private readonly IMomoService _momoService;


        public CreateMomoPaymentCommandHandler(IMomoService momoService)
        {
            _momoService = momoService;
        }
        public async Task<(bool success, string paymentUrl, string message)> Handle(CreateMomoPaymentCommand request, CancellationToken cancellationToken)
        {
            var (success, paymentUrl, message) = await _momoService.CreatePaymentLinkAsync(
                request.RequiredAmount,
                request.PaymentContent ?? string.Empty,
                request.UserId,
                request.ExtraData);

            return (success, paymentUrl, message);
        }
    }
}
