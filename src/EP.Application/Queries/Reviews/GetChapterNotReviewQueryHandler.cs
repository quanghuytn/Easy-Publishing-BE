using EP.Application.Common;
using EP.Application.Common.DTOs.Chapter;
using EP.Application.Common.Interfaces;
using EP.Application.Common.Pagination;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Queries.Reviews
{
    public record GetChapterNotReviewQuery : IRequest<ApiResponse<PaginatedResult<ChapterReviewDto>>>
    {
        public int AuthorId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = 10;
    }
    public class GetChapterNotReviewQueryHandler : IRequestHandler<GetChapterNotReviewQuery, ApiResponse<PaginatedResult<ChapterReviewDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetChapterNotReviewQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<ApiResponse<PaginatedResult<ChapterReviewDto>>> Handle(GetChapterNotReviewQuery request, CancellationToken cancellationToken)
        {
            var data = await _unitOfWork.ChapterRepository
                .GetChapterNotReview(request.AuthorId, request.PageIndex, request.PageSize);

            return ApiResponse<PaginatedResult<ChapterReviewDto>>.Success("Danh sách chương chưa review", data);
        }
    }
}
