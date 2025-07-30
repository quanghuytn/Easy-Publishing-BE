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
    public record GetTopStoriesReadShelvesQuery : IRequest<ApiResponse<IEnumerable<TopStoryDto>>>
    {
        public int CategoryId { get; set; }
    }
    public class GetTopStoriesReadShelvesQueryHandler : IRequestHandler<GetTopStoriesReadShelvesQuery, ApiResponse<IEnumerable<TopStoryDto>>>
    {
        private readonly IShelvesRepository _shelvesRepository;
        public GetTopStoriesReadShelvesQueryHandler(IShelvesRepository shelvesRepository)
        {
            _shelvesRepository = shelvesRepository;
        }
        public async Task<ApiResponse<IEnumerable<TopStoryDto>>> Handle(GetTopStoriesReadShelvesQuery request, CancellationToken cancellationToken)
        {
            var data = await _shelvesRepository.GetTopStoriesReadShelves(request.CategoryId);
            return ApiResponse<IEnumerable<TopStoryDto>>.Success(data, "Top lượt đọc theo thể loại");
        }
    }
}
