using EP.Application.Common;
using EP.Application.Common.DTOs.Transaction;
using EP.Application.Common.Interfaces;
using EP.Application.Common.Interfaces.Repositories;
using EP.Domain.Models;
using MediatR;

namespace EP.Application.Queries.Transactions
{
    public record GetInformationToBuyChapterQuery(int StoryId, int UserId) : IRequest<ApiResponse<GetInfoPurchaseChapterResponse>>;
    public class GetInformationToBuyChapterQueryHandler : IRequestHandler<GetInformationToBuyChapterQuery, ApiResponse<GetInfoPurchaseChapterResponse>>
    {
        private readonly IChapterRepository _chapterRepository;
        public GetInformationToBuyChapterQueryHandler(IChapterRepository chapterRepository)
        {
            _chapterRepository = chapterRepository;
        }
        public async Task<ApiResponse<GetInfoPurchaseChapterResponse>> Handle(GetInformationToBuyChapterQuery request, CancellationToken cancellationToken)
        {
            if (request.UserId == 0)
            {
                return ApiResponse<GetInfoPurchaseChapterResponse>.Failure("Yêu cầu đăng nhập!");
            }

            var data = await _chapterRepository.GetInfoPurchaseChapter(request.StoryId);
            if(data == null)
            {
                return ApiResponse<GetInfoPurchaseChapterResponse>.Failure("Tải thông tin thất bại!. Vui lòng thử lại sau");
            }

            return ApiResponse<GetInfoPurchaseChapterResponse>.Success("Thông tin để mua chương", data);
        }
    }
}
