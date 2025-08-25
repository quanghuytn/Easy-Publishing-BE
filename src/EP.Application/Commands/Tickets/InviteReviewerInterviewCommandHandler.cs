using EP.Application.Common;
using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Services;
using FluentValidation;
using MediatR;

namespace EP.Application.Commands.Tickets
{
    public record InviteReviewerInterviewCommand : IRequest<ApiResponse<string>>
    {
        public int TicketId { get; set; }
        public string Location { get; set; }
        public string Time { get; set; }
    }
    public class InviteReviewerInterviewCommandValidator : AbstractValidator<InviteReviewerInterviewCommand>
    {
        public InviteReviewerInterviewCommandValidator()
        {
            RuleFor(x => x.TicketId)
                .GreaterThan(0).WithMessage("TicketId is invalid.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(200).WithMessage("Location cannot exceed 200 characters.");

            RuleFor(x => x.Time)
                .NotEmpty().WithMessage("Time is required.")
                .Must(BeValidDateTime).WithMessage("Time must be a valid date and time format.");
        }

        private bool BeValidDateTime(string time)
        {
            return DateTime.TryParse(time, out _);
        }
    }
    public class InviteReviewerInterviewCommandHandler : IRequestHandler<InviteReviewerInterviewCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork; 
        private readonly IMailService _mailService;

        public InviteReviewerInterviewCommandHandler(IUnitOfWork unitOfWork, IMailService mailService)
        {
            _unitOfWork = unitOfWork;
            _mailService = mailService;
        }
        public async Task<ApiResponse<string>> Handle(InviteReviewerInterviewCommand request, CancellationToken cancellationToken)
        {
            DateTime dt = DateTime.Parse(request.Time);
            var date = dt.ToString("dd/MM/yyyy");
            var time = dt.ToString("hh:mm tt");
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(request.TicketId);
            if (ticket == null)
            {
                throw new Exception("Ticket is not available");
            }

            var ticketUser = await _unitOfWork.UserRepository.GetByIdAsync(ticket.UserId);
            if(ticketUser == null)
            {
                return ApiResponse<string>.Failure("Tài khoản của người phỏng vấn không tồn tại!.");
            }

            try
            {
                _mailService.Send(ticketUser.Email,
                        "Easy Publishing: Thư mời phỏng vấn",
                        "<p>Xin chào <b>" + ticketUser.Username + ",</b></p>" +
                        "<p>Chúng tôi đã nhận được yêu cầu trở thành reviewer của bạn.</p>" +
                        "<p>Chúng tôi trân trọng kính mời bạn đến tham gia buổi phỏng vấn của công ty chúng tôi tại:</p>" +
                        "<ul><li>Thời gian: " + time + ", " + date + "</li>" +
                        "<li>Địa điểm: " + request.Location + "</li></ul>" +
                        "<p>Để buổi phỏng vấn được diễn ra thuận lợi, bạn vui lòng phản hồi lại email này trong 24h kể từ khi nhận được.</p>" +
                        "<p>Chúc bạn sẽ có một buổi phỏng vấn thành công.</p>");
            }
            catch (Exception ex)
            {
               throw new Exception("Hệ thống xảy ra lỗi", ex);
            }

            ticket.Seen = true;
            await _unitOfWork.TicketRepository.UpdateAsync(ticket);

            var affectedRows = await _unitOfWork.CompleteAsync();
            if (affectedRows > 0)
            {
                return ApiResponse<string>.Success("Send interview invitations successfully!");
            }
            else
            {
                return ApiResponse<string>.Failure("Send interview invitations fail. Please try again later.");
            }
        }
    }
}
