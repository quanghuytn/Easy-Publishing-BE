using EP.Application.Commands.Category;
using EP.Application.Common.DTOs.Category;
using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly Context _context;

        public CategoryRepository(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategories()
        {
            return await _context.Categories
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
            var category = await _context.Categories.Where(c => c.CategoryId == id)
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

        public async Task<OptionFilterDto> GetOptionFilter()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryDescription = c.CategoryDescription
                })
                .ToListAsync();

            var prices = await _context.Stories
                .AsNoTracking()
                .Select(s => s.StoryPrice)
                .ToListAsync();

            decimal from = 0, to = 0;
            if (prices.Count > 0)
            {
                from = prices.Min();
                to = prices.Max();
            }

            return new OptionFilterDto
            {
                Categories = categories,
                From = from,
                To = to
            };
        }

        public async Task UpdateCategory(Category category)
        {
            _context.Entry(category).State = EntityState.Modified;
        }

        public async Task<bool> AddCategory(AddCategoryCommand newCategory)
        {
            if ((_context.Categories?.Any(e => e.CategoryName == newCategory.CategoryName) ?? false))
            {
                return false;
            }

            try
            {
                Category category = new Category()
                {
                    CategoryName = newCategory.CategoryName,
                    CategoryBanner = newCategory.CategoryBanner,
                    CategoryDescription = newCategory.CategoryDescription,
                };
                if (_context.Categories != null)
                {
                    await _context.Categories.AddAsync(category);
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

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }
    }
}
