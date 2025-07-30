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
    public record GetMinimalTopLatestStoriesQuery : IRequest<ApiResponse<PaginatedResult<TopStoryDto>>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = 10;
    }
    public class GetMinimalTopLatestStoriesQueryHandler : IRequestHandler<GetMinimalTopLatestStoriesQuery, ApiResponse<PaginatedResult<TopStoryDto>>>
    {
        private readonly IShelvesRepository _shelvesRepository;
        public GetMinimalTopLatestStoriesQueryHandler(IShelvesRepository shelvesRepository)
        {
            _shelvesRepository = shelvesRepository;
        }
        public async Task<ApiResponse<PaginatedResult<TopStoryDto>>> Handle(GetMinimalTopLatestStoriesQuery request, CancellationToken cancellationToken)
        {
            var data = await _shelvesRepository.GetMinimalTopLatestStories(request.PageIndex, request.PageSize);
            return ApiResponse<PaginatedResult<TopStoryDto>>.Success(data, "Truyện mới thêm");
        }
    }
}
