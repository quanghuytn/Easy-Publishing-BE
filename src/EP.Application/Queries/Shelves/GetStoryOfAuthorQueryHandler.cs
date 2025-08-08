using EP.Application.Common;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Pagination;
using MediatR;

namespace EP.Application.Queries.Shelves
{
    public record GetStoryOfAuthorQuery : IRequest<ApiResponse<PaginatedResult<StoryOfAuthorDto>>>
    {
        public int AuthorId { get; set; }
        public string? Title { get; set; }
        public string? Sort { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = 10;
    }
    public class GetStoryOfAuthorQueryHandler : IRequestHandler<GetStoryOfAuthorQuery, ApiResponse<PaginatedResult<StoryOfAuthorDto>>>
    {
        private readonly IShelvesRepository _shelvesRepository;
        public GetStoryOfAuthorQueryHandler(IShelvesRepository shelvesRepository)
        {
            _shelvesRepository = shelvesRepository;
        }
        public async Task<ApiResponse<PaginatedResult<StoryOfAuthorDto>>> Handle(GetStoryOfAuthorQuery request, CancellationToken cancellationToken)
        {
            var data = await _shelvesRepository.GetStoryOfAuthor(request.AuthorId, request.Title, request.Sort, request.PageIndex, request.PageSize);

            return ApiResponse<PaginatedResult<StoryOfAuthorDto>>.Success("Get story of author successfully", data);
        }
    }
}
