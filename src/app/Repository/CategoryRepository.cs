using app.DTOs.Category;
using app.Interface;
using app.Models;
using app.Service.Caching;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace app.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EasyPublishingContext _context;
        private readonly IRedisCacheService _cache;

        public CategoryRepository(EasyPublishingContext context, IRedisCacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<bool> AddCategory(addCategoryDto newCategory)
        {
            if ((_context.Categories?.Any(e => e.CategoryName == newCategory.CategoryName)).GetValueOrDefault())
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
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategories()
        {
            var cacheKey = "categories";
            var cate = await _cache.StringGetAsync<IEnumerable<CategoryDto>>(cacheKey);
            if (cate is null)
            {
                cate = await _context.Categories
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

                _cache.StringSetAsync(cacheKey, cate);
            }

            return cate;
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
    }
}
