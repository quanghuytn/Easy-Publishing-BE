using app.DTOs.Category;
using app.Models;

namespace app.Interface
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDto>> GetAllCategories();
        Task<CategoryDto?> GetCategoryById(int id);
        Task<OptionFilterDto> GetOptionFilter();
        Task UpdateCategory(Category category);
        Task<bool> AddCategory(addCategoryDto newCategory);
    }
}
