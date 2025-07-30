using EP.Application.Common;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Pagination;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Queries.Shelves
{
    public record GetStoriesDoneEachCateQuery : IRequest<ApiResponse<PaginatedResult<TopStoryDto>>>
    {
        public int CategoryId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = 10;
    }
    public class GetStoriesDoneEachCateQueryHandler : IRequestHandler<GetStoriesDoneEachCateQuery, ApiResponse<PaginatedResult<TopStoryDto>>>
    {
        private readonly IShelvesRepository _shelvesRepository;
        public GetStoriesDoneEachCateQueryHandler(IShelvesRepository shelvesRepository)
        {
            _shelvesRepository = shelvesRepository;
        }
        public async Task<ApiResponse<PaginatedResult<TopStoryDto>>> Handle(GetStoriesDoneEachCateQuery request, CancellationToken cancellationToken)
        {
            var data = await _shelvesRepository.GetStoriesDoneEachCate(request.CategoryId, request.PageIndex, request.PageSize);
            return ApiResponse<PaginatedResult<TopStoryDto>>.Success(data, "Truyện hoàn thành theo thể loại");
        }
    }
}
