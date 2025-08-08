using EP.Application.Common;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Pagination;
using MediatR;

namespace EP.Application.Queries.Shelves
{
    public record GetReadHistoryQuery : IRequest<ApiResponse<PaginatedResult<TopStoryDto>>>
    {
        public int UserId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
    public class GetReadHistoryQueryHandler : IRequestHandler<GetReadHistoryQuery, ApiResponse<PaginatedResult<TopStoryDto>>>
    {
        private readonly IShelvesRepository _shelvesRepository;
        public GetReadHistoryQueryHandler(IShelvesRepository shelvesRepository)
        {
            _shelvesRepository = shelvesRepository;
        }
        public async Task<ApiResponse<PaginatedResult<TopStoryDto>>> Handle(GetReadHistoryQuery request, CancellationToken cancellationToken)
        {
            var data = await _shelvesRepository.GetReadHistory(request.UserId, request.Page, request.PageSize);

            return ApiResponse<PaginatedResult<TopStoryDto>>.Success("Get read history successfully", data);
        }
    }
}
