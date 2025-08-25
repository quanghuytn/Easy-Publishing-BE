using EP.Application.Common;
using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EP.Application.Commands.Tickets
{
    public record DenyReviewerRequestCommand : IRequest<ApiResponse<string>>
    {
        public int TicketId { get; set; }
    }
    internal class DenyReviewerRequestCommandHandler : IRequestHandler<DenyReviewerRequestCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailService _mailService;

        public DenyReviewerRequestCommandHandler(IUnitOfWork unitOfWork, IMailService mailService)
        {
            _unitOfWork = unitOfWork;
            _mailService = mailService;
        }
        public async Task<ApiResponse<string>> Handle(DenyReviewerRequestCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _unitOfWork.TicketRepository.GetByIdAsync(request.TicketId) ?? throw new Exception("Ticket không tồn tại!");

            if (ticket.Status == true)
            {
                return ApiResponse<string>.Failure("This request has already been approved");
            }
            else if (ticket.Status == null)
            {
                return ApiResponse<string>.Failure("This request has already been denied");
            }

            try
            {
                ticket.Status = null;
                var ticketUser = await _unitOfWork.UserRepository.GetByIdAsync(ticket.UserId);

                if (ticketUser.RoleId == 3)
                {
                    return ApiResponse<string>.Failure("This user is already a reviewer");
                }
                ticketUser.RoleId = 3;

                _mailService.Send(ticketUser.Email,
                            "Easy Publishing: Yêu cầu trở thành reviewer bị từ chối",
                            "<p>Xin chào <b>" + ticketUser.Username + ",</b></p>" +
                            "<p>Yêu cầu trở thành reviewer của bạn đã bị từ chối.</p>");

                var affectedRows = await _unitOfWork.CompleteAsync();
                if (affectedRows > 0)
                {
                    return ApiResponse<string>.Success("Deny reviewer request successfully");
                }
                else
                {
                    return ApiResponse<string>.Failure("Deny reviewer request fail. Please try again later.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Hệ thống xảy ra lỗi", ex);
            }
        }
    }
}
