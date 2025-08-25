using EP.Application.Common;
using EP.Application.Common.DTOs.RefundRequest;
using EP.Application.Common.Interfaces;
using MediatR;

namespace EP.Application.Queries.RefundRequests
{
    public record GetPendingRefundRequestQuery : IRequest<ApiResponse<IEnumerable<RefundExportDto>>>;
    public class GetPendingRefundRequestQueryHandler : IRequestHandler<GetPendingRefundRequestQuery, ApiResponse<IEnumerable<RefundExportDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetPendingRefundRequestQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<IEnumerable<RefundExportDto>>> Handle(GetPendingRefundRequestQuery request, CancellationToken cancellationToken)
        {
            var inProgressRequests = await _unitOfWork.RefundRequestsRepository.GetInProgressRefundRequestExportAsync();
            if (inProgressRequests.ToList().Count > 0)
            {
                return ApiResponse<IEnumerable<RefundExportDto>>.Success("Xử lý các yêu cầu đang dở", inProgressRequests);
            }

            var requests = (await _unitOfWork.RefundRequestsRepository.GetPendingRefundRequestExportAsync()).ToList();
            if (!requests.Any())
                return ApiResponse<IEnumerable<RefundExportDto>>.Failure("Yêu cầu đã được phê duyệt rồi");

            var now = DateTime.Now;
            requests.ForEach(r => r.ResponseTime = now);
            _unitOfWork.RefundRequestsRepository.UpdateRange(requests);

            var affectedRows = await _unitOfWork.CompleteAsync();
            if (affectedRows == 0)
            {
                return ApiResponse<IEnumerable<RefundExportDto>>.Failure("Xử lý yêu cầu thất bại. Vui lòng thử lại sau!");
            }
            
            var exportList = requests.Select(c => new RefundExportDto
            {
                UserFullname = c.Wallet.User.UserFullname,
                BankId = c.BankId,
                BankAccount = c.BankAccount,
                Amount = ((int)c.Amount * 1000).ToString(),
                RequestTime = c.RequestTime.ToString("yyyy-MM-dd HH:mm:ss"),
                ResponseTime = c.ResponseTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""
            }).ToList();
            
            return ApiResponse<IEnumerable<RefundExportDto>>.Success("Yêu cầu phê duyệt", exportList);
        }
    }
}
