using app.DTOs.Author;
using app.DTOs.Interaction;
using app.DTOs.Shelves;
using app.Interface;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Repository
{
    public class ShelvesRepository : IShelvesRepository
    {
        private readonly EasyPublishingContext _context;

        public ShelvesRepository(EasyPublishingContext context)
        {
            _context = context;
        }

        public async Task<List<Top6PurchaseDto>> GetTop6StoriesPurchase()
        {
            var stories = await _context.Stories
                .AsNoTracking()
                .Where(c => c.Status > 0)
                .Include(c => c.Author)
                .Include(c => c.Users).Include(c => c.StoryInteraction)
                .Include(c => c.Chapters).ThenInclude(c => c.Users)
                .Select(s => new Top6PurchaseDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryDescription = s.StoryDescription,
                    StoryAuthor = new MinimalAuthorDto { UserId = s.Author.UserId, UserFullname = s.Author.UserFullname },
                    StoryCreateTime = s.CreateTime,
                    StoryChapterNumber = s.Chapters.Count(c => c.Status > 0),
                    StoryInteraction = new MinimalInteractionDto
                    {
                        Like = s.StoryInteraction.Like,
                        Follow = s.StoryInteraction.Follow,
                        View = s.StoryInteraction.View,
                        Read = s.StoryInteraction.Read
                    },
                    UserCount = s.Users.Count,
                    UserPurchaseChapter = s.Chapters.SelectMany(c => c.Users).Count(),
                })
                .OrderByDescending(s => s.UserCount)
                .ThenByDescending(s => s.UserPurchaseChapter).Take(6).ToListAsync();
            return stories;
        }

        public async Task<List<StoryShelfDto>> GetTopFamousStories()
        {
            var stories = await _context.Stories
                .AsNoTracking()
                .Where(c => c.Status > 0)
                .Include(c => c.Author)
                .Include(c => c.Categories)
                .Include(c => c.Users)
                .Include(c => c.Chapters).ThenInclude(c => c.Users)
                .Include(c => c.StoryInteraction)
                .Select(s => new StoryShelfDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryDescription = s.StoryDescription,
                    StoryDescriptionHtml = s.StoryDescriptionHtml,
                    StoryDescriptionMarkdown = s.StoryDescriptionMarkdown,
                    StoryCategories = s.Categories.Select(c => new CategoryShelfDto
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName
                    }).ToList(),
                    StoryAuthor = new MinimalAuthorDto{ UserId =  s.Author.UserId, UserFullname = s.Author.UserFullname },
                    StoryCreateTime = s.CreateTime,
                    StoryChapterNumber = s.Chapters.Count,
                    StoryLatestChapter = s.Chapters.Where(c => c.Status > 0).OrderByDescending(c => c.ChapterNumber).Select(c => new ChapterShelfDto
                    {
                        ChapterId = c.ChapterId,
                        ChapterNumber = c.ChapterNumber,
                        ChapterTitle = c.ChapterTitle,
                        CreateTime = c.CreateTime
                    }).FirstOrDefault(),
                    StoryInteraction = new MinimalInteractionDto
                    {
                        Like = s.StoryInteraction.Like,
                        Follow = s.StoryInteraction.Follow,
                        View = s.StoryInteraction.View,
                        Read = s.StoryInteraction.Read
                    },
                    UserPurchaseStory = s.Users.Count,
                    UserPurchaseChapter = s.Chapters.SelectMany(c => c.Users).Count(),
                })
                .OrderByDescending(s => s.UserPurchaseStory) // top famous compare
                .ThenByDescending(s => s.UserPurchaseChapter)
                .ThenByDescending(s => s.StoryInteraction.Read).ThenByDescending(s => s.StoryInteraction.Follow)
                .ThenByDescending(s => s.StoryInteraction.Like)
                .ToListAsync();

            return stories;
        }

        public async Task<List<TopPriceStoryDto>> GetTopPriceStories()
        {
            var stories = await _context.Stories
                .AsNoTracking()
                .Where(c => c.Status > 0)
                .Include(c => c.Author)
                .Include(c => c.Categories)
                .Include(c => c.Chapters)
                .Select(s => new TopPriceStoryDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryDescription = s.StoryDescription,
                    StoryDescriptionHtml = s.StoryDescriptionHtml,
                    StoryDescriptionMarkdown = s.StoryDescriptionMarkdown,
                    StoryCategories = s.Categories.Select(c => new CategoryShelfDto
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName
                    }).ToList(),
                    StoryAuthor = new MinimalAuthorDto { UserId = s.Author.UserId, UserFullname = s.Author.UserFullname },
                    StoryCreateTime = s.CreateTime,
                    StoryChapterNumber = s.Chapters.Count,
                    StoryLatestChapter = s.Chapters.Where(c => c.Status > 0).OrderByDescending(c => c.ChapterNumber).Select(c => new ChapterShelfDto
                    {
                        ChapterId = c.ChapterId,
                        ChapterNumber = c.ChapterNumber,
                        ChapterTitle = c.ChapterTitle,
                        CreateTime = c.CreateTime
                    }).FirstOrDefault(),
                    StoryPrice = s.StoryPrice,
                    ChaptersPrice = s.Chapters.Select(c => c.ChapterPrice).Sum(),
                }).OrderBy(c => c.StoryPrice)      
                .ThenBy(c => c.ChaptersPrice).ToListAsync();
            return stories;
        }

        public async Task<List<TopLatestStoryDto>> GetTopLatestStoriesByChapter()
        {
            var stories = await _context.Stories
                .AsNoTracking()
                .Where(c => c.Status > 0)
                .Include(c => c.Author)
                .Include(c => c.Categories)
                .Include(c => c.Chapters)
                .Select(s => new TopLatestStoryDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryDescription = s.StoryDescription,
                    StoryDescriptionHtml = s.StoryDescriptionHtml,
                    StoryDescriptionMarkdown = s.StoryDescriptionMarkdown,
                    StoryCategories = s.Categories.Select(c => new CategoryShelfDto
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName
                    }).ToList(),
                    StoryAuthor = new MinimalAuthorDto { UserId = s.Author.UserId, UserFullname = s.Author.UserFullname },
                    StoryCreateTime = s.CreateTime,
                    StoryChapterNumber = s.Chapters.Count,
                    StoryLatestChapter = s.Chapters.Where(c => c.Status > 0).OrderByDescending(c => c.ChapterNumber).Select(c => new ChapterShelfDto
                    {
                        ChapterId = c.ChapterId,
                        ChapterNumber = c.ChapterNumber,
                        ChapterTitle = c.ChapterTitle,
                        CreateTime = c.CreateTime
                    }).FirstOrDefault(),
                })
                .OrderByDescending(c => c.StoryLatestChapter.ChapterId) // latest by chapters
                .ToListAsync();
            return stories;
        }

        public async Task<List<TopReadStoryDto>> GetTopStoriesRead()
        {
            var stories = await _context.Stories
                .AsNoTracking()
                .Where(c => c.Status > 0)
                .Include(c => c.StoryInteraction)
                .Include(c => c.Author)
                .Include(c => c.Categories)
                .Include(c => c.Chapters)
                .Select(s => new TopReadStoryDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryDescription = s.StoryDescription,
                    StoryDescriptionHtml = s.StoryDescriptionHtml,
                    StoryDescriptionMarkdown = s.StoryDescriptionMarkdown,
                    StoryCategories = s.Categories.Select(c => new CategoryShelfDto
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName
                    }).ToList(),
                    StoryAuthor = new MinimalAuthorDto { UserId = s.Author.UserId, UserFullname = s.Author.UserFullname },
                    StoryCreateTime = s.CreateTime,
                    StoryChapterNumber = s.Chapters.Count,
                    StoryLatestChapter = s.Chapters.Where(c => c.Status > 0).OrderByDescending(c => c.ChapterNumber).Select(c => new ChapterShelfDto
                    {
                        ChapterId = c.ChapterId,
                        ChapterNumber = c.ChapterNumber,
                        ChapterTitle = c.ChapterTitle,
                        CreateTime = c.CreateTime
                    }).FirstOrDefault(),
                    StoryInteraction = new MinimalInteractionDto
                    {
                        Like = s.StoryInteraction.Like,
                        Follow = s.StoryInteraction.Follow,
                        View = s.StoryInteraction.View,
                        Read = s.StoryInteraction.Read
                    },
                })
                .OrderByDescending(c => c.StoryInteraction.Read).ToListAsync();
            return stories;
        }


    }
}
