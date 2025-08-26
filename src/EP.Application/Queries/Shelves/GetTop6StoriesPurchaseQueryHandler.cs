using EP.Application.Common;
using EP.Application.Common.DTOs.Category;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Interfaces.Services.Cache;
using MediatR;

namespace EP.Application.Queries.Shelves
{
    public class GetTop6StoriesPurchaseQuery() : IRequest<ApiResponse<IEnumerable<TopStoryDto>>>;
    public class GetTop6StoriesPurchaseQueryHandler : IRequestHandler<GetTop6StoriesPurchaseQuery, ApiResponse<IEnumerable<TopStoryDto>>>
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly IShelvesRepository _shelvesRepository;
        public GetTop6StoriesPurchaseQueryHandler(IShelvesRepository shelvesRepository, IRedisCacheService redisCacheService)
        {
            _shelvesRepository = shelvesRepository;
            _redisCacheService = redisCacheService;
        }
        public async Task<ApiResponse<IEnumerable<TopStoryDto>>> Handle(GetTop6StoriesPurchaseQuery request, CancellationToken cancellationToken)
        {
            const string cacheKey = "stories:top6purchase";

            var cachedStories = await _redisCacheService.StringGetAsync<IEnumerable<TopStoryDto>>(cacheKey);
            if (cachedStories != null && cachedStories.Any())
            {
                return ApiResponse<IEnumerable<TopStoryDto>>.Success("Get top 6 stories purchase successfully.", cachedStories);
            }

            var stories = await _shelvesRepository.GetTop6StoriesPurchase();

            await _redisCacheService.StringSetAsync(cacheKey, stories, TimeSpan.FromHours(1));

            return ApiResponse<IEnumerable<TopStoryDto>>.Success("Get top 6 stories purchase successfully.", stories);
        }
    }
}
