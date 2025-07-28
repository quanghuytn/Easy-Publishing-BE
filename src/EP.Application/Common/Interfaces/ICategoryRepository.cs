using EP.Application.Commands.Category;
using EP.Application.Common.DTOs.Category;
using EP.Domain.Models;

namespace EP.Application.Common.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDto>> GetAllCategories();
        Task<CategoryDto?> GetCategoryById(int id);
        Task<OptionFilterDto> GetOptionFilter();
        Task UpdateCategory(Category category);
        Task<bool> AddCategory(AddCategoryCommand newCategory);
        Task<Category?> GetByIdAsync(int id);
    }
}
