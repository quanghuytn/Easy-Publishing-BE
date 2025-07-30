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
    public record GetStoriesEachCateQuery : IRequest<ApiResponse<PaginatedResult<TopStoryDto>>>
    {
        public int CategoryId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = 10;
    }
    public class GetStoriesEachCateQueryHandler : IRequestHandler<GetStoriesEachCateQuery, ApiResponse<PaginatedResult<TopStoryDto>>>
    {
        private readonly IShelvesRepository _shelvesRepository;
        public GetStoriesEachCateQueryHandler(IShelvesRepository shelvesRepository)
        {
            _shelvesRepository = shelvesRepository;
        }
        public async Task<ApiResponse<PaginatedResult<TopStoryDto>>> Handle(GetStoriesEachCateQuery request, CancellationToken cancellationToken)
        {
            var data = await _shelvesRepository.GetStoriesEachCate(request.CategoryId, request.PageIndex, request.PageSize);
            return ApiResponse<PaginatedResult<TopStoryDto>>.Success(data, "Truyện theo thể loại");
        }
    }
}
