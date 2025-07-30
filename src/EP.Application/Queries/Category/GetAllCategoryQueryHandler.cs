using EP.Application.Common;
using EP.Application.Common.DTOs.Category;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Category
{
    public record GetAllCategoryQuery : IRequest<ApiResponse<IEnumerable<CategoryDto>>>;
    public class GetAllCategoryQueryHandler: IRequestHandler<GetAllCategoryQuery, ApiResponse<IEnumerable<CategoryDto>>>
    {
        private readonly ICategoryRepository _categoryRepository;
        public GetAllCategoryQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<ApiResponse<IEnumerable<CategoryDto>>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
        {
            var categoryList = await _categoryRepository.GetAllCategories();
            return ApiResponse<IEnumerable<CategoryDto>>.Success(categoryList, "Các thể loại truyện");
        }
    }
}
