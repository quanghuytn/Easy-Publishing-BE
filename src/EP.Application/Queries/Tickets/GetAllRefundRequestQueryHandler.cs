using EP.Application.Common;
using EP.Application.Common.DTOs.RefundRequest;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;
using System.Collections.Generic;

namespace EP.Application.Queries.Tickets
{
    public record GetAllRefundRequestQuery : IRequest<ApiResponse<IEnumerable<RefundRequestListDto>>>;
    public class GetAllRefundRequestQueryHandler : IRequestHandler<GetAllRefundRequestQuery, ApiResponse<IEnumerable<RefundRequestListDto>>>
    {
        private readonly IRefundRequestsRepository _refundRequestsRepository;
        public GetAllRefundRequestQueryHandler(IRefundRequestsRepository refundRequestsRepository)
        {
            _refundRequestsRepository = refundRequestsRepository;
        }
        public async Task<ApiResponse<IEnumerable<RefundRequestListDto>>> Handle(GetAllRefundRequestQuery request, CancellationToken cancellationToken)
        {
            var requests = await _refundRequestsRepository.GetAllRefundRequestsAsync();

            return ApiResponse<IEnumerable<RefundRequestListDto>>.Success("Yêu cầu rút tiền", requests);
        }
    }
}
