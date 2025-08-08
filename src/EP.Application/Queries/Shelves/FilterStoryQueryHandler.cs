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
    public record FilterStoryQuery : IRequest<ApiResponse<PaginatedResult<TopStoryDto>>>
    {
        public string? Title { get; set; }
        public int? To { get; set; }
        public int? From { get; set; }
        public string? Sort { get; set; }
        public List<int> Cates { get; set; } = new List<int>();
        public int? Status { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = 10;
    }
    public class FilterStoryQueryHandler : IRequestHandler<FilterStoryQuery, ApiResponse<PaginatedResult<TopStoryDto>>>
    {
        private readonly IShelvesRepository _shelvesRepository;
        public FilterStoryQueryHandler(IShelvesRepository shelvesRepository)
        {
            _shelvesRepository = shelvesRepository;
        }
        public async Task<ApiResponse<PaginatedResult<TopStoryDto>>> Handle(FilterStoryQuery request, CancellationToken cancellationToken)
        {
            var data = await _shelvesRepository.FilterStory(
                request.Title, 
                request.To, 
                request.From, 
                request.Sort, 
                request.Cates, 
                request.Status, 
                request.PageIndex, 
                request.PageSize);

            return ApiResponse<PaginatedResult<TopStoryDto>>.Success("Filtered stories successfully", data);
        }
    }
}
