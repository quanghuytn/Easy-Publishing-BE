using EP.Application.Commands.Categories;
using EP.Application.Commands.Categories;
using EP.Application.Common.DTOs.Category;
using EP.Domain.Models;

namespace EP.Application.Common.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<CategoryDto>> GetAllCategories();
        Task<CategoryDto?> GetCategoryById(int id);
        Task<bool> AddCategory(AddCategoryCommand newCategory);
    }
}
