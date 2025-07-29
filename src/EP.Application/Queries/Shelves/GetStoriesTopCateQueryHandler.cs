using EP.Application.Common;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces;
using EP.Application.Common.Pagination;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Queries.Shelves
{
    public record GetStoriesTopCateQuery : IRequest<ApiResponse<IEnumerable<TopStoryDto>>>
    {
        public int CategoryId { get; set; }
    }
    public class GetStoriesTopCateQueryHandler : IRequestHandler<GetStoriesTopCateQuery, ApiResponse<IEnumerable<TopStoryDto>>>
    {
        private readonly IShelvesRepository _shelvesRepository;
        public GetStoriesTopCateQueryHandler(IShelvesRepository shelvesRepository)
        {
            _shelvesRepository = shelvesRepository;
        }
        public async Task<ApiResponse<IEnumerable<TopStoryDto>>> Handle(GetStoriesTopCateQuery request, CancellationToken cancellationToken)
        {
            var data = await _shelvesRepository.GetStoriesTopCate(request.CategoryId);
            return ApiResponse<IEnumerable<TopStoryDto>>.Success(data, "Top theo thể loại");
        }
    }
}
