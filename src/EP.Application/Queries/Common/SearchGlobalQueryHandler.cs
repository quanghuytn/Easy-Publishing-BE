using EP.Application.Common;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Common
{
    public record SearchGlobalQuery(string? search, int? authorId, int? from, int? to, int? status, List<int> cates) : IRequest<ApiResponse<IEnumerable<TopStoryDto>>>;
    public class SearchGlobalQueryHandler : IRequestHandler<SearchGlobalQuery, ApiResponse<IEnumerable<TopStoryDto>>>
    {
        private readonly IStoryRepository _storyRepository;
        public SearchGlobalQueryHandler(IStoryRepository storyRepository)
        {
            _storyRepository = storyRepository;
        }
        public async Task<ApiResponse<IEnumerable<TopStoryDto>>> Handle(SearchGlobalQuery request, CancellationToken cancellationToken)
        {
            var stories = await _storyRepository.SearchGlobal(request.search, request.authorId, request.from, request.to, request.status, request.cates);

            return ApiResponse<IEnumerable<TopStoryDto>>.Success("Search results retrieved successfully.", stories);
        }
    }
}
