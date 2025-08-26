using EP.Application.Common;
using EP.Application.Common.DTOs.Category;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Interfaces.Services.Cache;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EP.Application.Queries.Category
{
    public record GetAllCategoryQuery : IRequest<ApiResponse<IEnumerable<CategoryDto>>>;
    public class GetAllCategoryQueryHandler: IRequestHandler<GetAllCategoryQuery, ApiResponse<IEnumerable<CategoryDto>>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IRedisCacheService _redisCacheService;
        private readonly ILogger<GetAllCategoryQueryHandler> _logger;
        public GetAllCategoryQueryHandler(ICategoryRepository categoryRepository, IRedisCacheService redisCacheService, ILogger<GetAllCategoryQueryHandler> logger)
        {
            _categoryRepository = categoryRepository;
            _redisCacheService = redisCacheService;
            _logger = logger;
        }
        public async Task<ApiResponse<IEnumerable<CategoryDto>>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
        {
            const string cacheKey = "catogories:all";

            var cachedCategories = await _redisCacheService.StringGetAsync<IEnumerable<CategoryDto>>(cacheKey);
            if (cachedCategories != null && cachedCategories.Any()) {
                _logger.LogInformation("Cache hit for {CacheKey}", cacheKey);

                return ApiResponse<IEnumerable<CategoryDto>>.Success("Các thể loại truyện", cachedCategories);
            }

            _logger.LogInformation("Cache miss for {CacheKey}", cacheKey);

            _logger.LogInformation("Đang lấy dữ liệu từ database...");
            var categoryList = await _categoryRepository.GetAllCategories();
            
            await _redisCacheService.StringSetAsync(cacheKey, categoryList, TimeSpan.FromHours(1));
            _logger.LogInformation("Đã lưu dữ liệu vào cache");

            return ApiResponse<IEnumerable<CategoryDto>>.Success("Các thể loại truyện", categoryList);
        }
    }
}
