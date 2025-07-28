using Azure;
using EP.Application.Common.DTOs.Category;
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
    public record GetMinimalTopStoriesReadQuery : IRequest<PaginatedResult<TopReadStoryDto>>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
    public class GetMinimalTopStoriesReadQueryHandler : IRequestHandler<GetMinimalTopStoriesReadQuery, PaginatedResult<TopReadStoryDto>>
    {
        private readonly IShelvesRepository _shelvesRepository;
        public GetMinimalTopStoriesReadQueryHandler(IShelvesRepository shelvesRepository)
        {
            _shelvesRepository = shelvesRepository;
        }
        public async Task<PaginatedResult<TopReadStoryDto>> Handle(GetMinimalTopStoriesReadQuery request, CancellationToken cancellationToken)
        {
            return await _shelvesRepository.GetMinimalTopStoriesRead(request.PageIndex, request.PageSize);
        }
    }
}
