using EP.Application.Commands.Categories;
using EP.Application.Common.DTOs.Category;
using EP.Application.Common.Interfaces.Repositories;
using EP.Domain.Models;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(Context context) : base(context)
        {
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategories()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(c => c.Stories)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryDescription = c.CategoryDescription.Substring(0, 50) + "...",
                    CategoryBanner = c.CategoryBanner,
                    StoriesNumber = c.Stories.Count,
                })
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetCategoryById(int id)
        {
            var category = await _dbSet.Where(c => c.CategoryId == id)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName.Replace("Truyện ", ""),
                    CategoryDescription = c.CategoryDescription,
                    CategoryBanner = c.CategoryBanner,
                    StoriesNumber = c.Stories.Count(),
                })
                .SingleOrDefaultAsync();

            return category;
        }

        public async Task<bool> AddCategory(AddCategoryCommand newCategory)
        {
            if ((_dbSet?.Any(e => e.CategoryName == newCategory.CategoryName) ?? false))
            {
                return false;
            }

            try
            {
                Category category = new()
                {
                    CategoryName = newCategory.CategoryName,
                    CategoryBanner = newCategory.CategoryBanner,
                    CategoryDescription = newCategory.CategoryDescription,
                };
                if (_dbSet != null)
                {
                    await _dbSet.AddAsync(category);
                }
                else
                {
                    throw new Exception("Categories DbSet is not initialized.");
                }
            }
            catch (Exception)
            {
                throw new Exception("Add Category Fail!");
            }
            return true;
        }
    }
}
