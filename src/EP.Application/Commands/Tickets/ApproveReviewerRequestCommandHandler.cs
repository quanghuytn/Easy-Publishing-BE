using EP.Application.Common;
using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Services;
using MediatR;

namespace EP.Application.Commands.Tickets
{
    public record ApproveReviewerRequestCommand : IRequest<ApiResponse<string>>
    {
        public int TicketId { get; set; }
    }
    public class ApproveReviewerRequestCommandHandler : IRequestHandler<ApproveReviewerRequestCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;

        public ApproveReviewerRequestCommandHandler(IUnitOfWork unitOfWork, IMailService mailService)
        {
            _unitOfWork = unitOfWork;
            _mailService = mailService;
        }
        public async Task<ApiResponse<string>> Handle(ApproveReviewerRequestCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(request.TicketId);
            if (ticket == null)
                throw new Exception("Ticket không tồn tại!");

            if (ticket.Status == true)
            {
                return ApiResponse<string>.Failure("This request has already been approved");
            }

            try
            {
                ticket.Status = true;
                var ticketUser = await _unitOfWork.UserRepository.GetByIdAsync(ticket.UserId);
                if (ticketUser.RoleId == 3)
                {
                    return ApiResponse<string>.Failure("This user is already a reviewer");
                }
                ticketUser.RoleId = 3;

                _mailService.Send(ticketUser.Email,
                            "Easy Publishing: Yêu cầu trở thành reviewer đã được phê duyệt",
                            "<p>Chúc mừng <b>" + ticketUser.Username + ",</b></p>" +
                            "<p>Bạn đã được phê duyệt trở thành reviewer.</p>");

                var affectedRows = await _unitOfWork.CompleteAsync();
                if (affectedRows > 0)
                {
                    return ApiResponse<string>.Success("Approved reviewer request successfully");
                }
                else
                {
                    return ApiResponse<string>.Failure("Approved reviewer request fail. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Hệ thống xảy ra lỗi", ex);
            }
        }
    }
}
