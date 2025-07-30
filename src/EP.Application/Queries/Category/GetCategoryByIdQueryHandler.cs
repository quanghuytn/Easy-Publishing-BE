using EP.Application.Common;
using EP.Application.Common.DTOs.Category;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Category 
{ 
    public record GetCategoryByIdQuery : IRequest<ApiResponse<CategoryDto>>
    {
        public int CategoryId { get; set; }
    }
    public class GetCategoryByIdQueryHandler: IRequestHandler<GetCategoryByIdQuery, ApiResponse<CategoryDto>>
    {
        private readonly ICategoryRepository _categoryRepository;
        public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<ApiResponse<CategoryDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetCategoryById(request.CategoryId);
            if (category == null)
            {
                throw new KeyNotFoundException($"Thể loại không tồn tại!");
            }
            return ApiResponse<CategoryDto>.Success(category, "Chi tiết thể loại");
        }
    }
}
