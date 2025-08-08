using EP.Application.Common;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Pagination;
using MediatR;

namespace EP.Application.Queries.Shelves
{
    public class GetOwnedStoryQuery : IRequest<ApiResponse<PaginatedResult<TopStoryDto>>>
    {
        public int UserId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; } = 10;
    }
    public class GetOwnedStoryQueryHandler : IRequestHandler<GetOwnedStoryQuery, ApiResponse<PaginatedResult<TopStoryDto>>>
    {
        private readonly IShelvesRepository _shelvesRepository;
        public GetOwnedStoryQueryHandler(IShelvesRepository shelvesRepository)
        {
            _shelvesRepository = shelvesRepository;
        }
        public async Task<ApiResponse<PaginatedResult<TopStoryDto>>> Handle(GetOwnedStoryQuery request, CancellationToken cancellationToken)
        {
            var data = await _shelvesRepository.GetOwnedStory(request.UserId, request.Page, request.PageSize);

            return ApiResponse<PaginatedResult<TopStoryDto>>.Success("Get owned stories successfully", data);
        }
    }
}
