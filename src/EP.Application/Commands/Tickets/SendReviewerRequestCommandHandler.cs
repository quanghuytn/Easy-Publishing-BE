using EP.Application.Common;
using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EP.Application.Commands.Tickets
{
    public record SendReviewerRequestCommand(int UserId) : IRequest<ApiResponse<string>>;
    public class SendReviewerRequestCommandHandler : IRequestHandler<SendReviewerRequestCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public SendReviewerRequestCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<string>> Handle(SendReviewerRequestCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new ArgumentException("Hệ thống xảy ra lỗi. Vui lòng thử lại sau.");
            }

            if (user.RoleId == 3)
                return ApiResponse<string>.Failure("Bạn hiện đã là Reviewer");

            bool hasPendingTicket = await _unitOfWork.TicketRepository
                        .CheckExist(t => t.UserId == request.UserId);
            if (hasPendingTicket)
                return ApiResponse<string>.Failure("Hiện đã có 1 yêu cầu của bạn đang chờ xử lý, vui lòng đợi phản hồi");

            try
            {
                Ticket newTicket = new Ticket()
                {
                    UserId = request.UserId,
                    Status = false,
                    Seen = false,
                    TicketDate = DateTime.Now,
                };
                await _unitOfWork.TicketRepository.AddAsync(newTicket);

                var affectedRows = await _unitOfWork.CompleteAsync();
                if (affectedRows > 0)
                {
                    return ApiResponse<string>.Success("Gửi yêu cầu trờ thành reviewer thành công, vui lòng chờ phản hồi từ chúng tôi");
                }
                else
                {
                    return ApiResponse<string>.Failure("Gửi yêu cầu thất bại!. Vui lòng thử lại sau.");
                }
            }
            catch (Exception)
            {
                throw new Exception("Hệ thống xảy ra lỗi!");
            }
        }
    }
}
