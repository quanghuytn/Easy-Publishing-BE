using EP.Application.Common;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Pagination;
using MediatR;

namespace EP.Application.Queries.Shelves
{
    public record GetFollowedStoryQuery : IRequest<ApiResponse<PaginatedResult<TopStoryDto>>>
    {
        public int UserId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = 10;
    }
    public class GetFollowedStoryQueryHandler : IRequestHandler<GetFollowedStoryQuery, ApiResponse<PaginatedResult<TopStoryDto>>>
    {
        private readonly IShelvesRepository _shelvesRepository;
        public GetFollowedStoryQueryHandler(IShelvesRepository shelvesRepository)
        {
            _shelvesRepository = shelvesRepository;
        }
        public async Task<ApiResponse<PaginatedResult<TopStoryDto>>> Handle(GetFollowedStoryQuery request, CancellationToken cancellationToken)
        {
            var data = await _shelvesRepository.GetFollowedStory(request.UserId, request.PageIndex, request.PageSize);

            return ApiResponse<PaginatedResult<TopStoryDto>>.Success("Get followed stories successfully", data);
        }
    }
}
