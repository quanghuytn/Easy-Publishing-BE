using EP.Application.Common.DTOs.Author;
using EP.Application.Common.DTOs.Interaction;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Pagination;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Repositories
{
    public class ShelvesRepository : IShelvesRepository
    {
        private readonly Context _context;

        public ShelvesRepository(Context context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<TopStoryDto>> GetMinimalTopFamousStories(int page, int pageSize)
        {
            var baseQuery = _context.Stories
                .AsNoTracking()
                .Where(c => c.Status > 0);

            int totalCount = await baseQuery.CountAsync();

            var stories = await baseQuery
                .Include(c => c.Author)
                .Include(c => c.Categories)
                .Include(c => c.Users)
                .Include(c => c.Chapters).ThenInclude(c => c.Users)
                .Include(c => c.StoryInteraction)
                .OrderByDescending(s => s.Users.Count)
                .ThenByDescending(s => s.Chapters.SelectMany(c => c.Users).Count())
                .ThenByDescending(s => s.StoryInteraction.Read)
                .ThenByDescending(s => s.StoryInteraction.Follow)
                .ThenByDescending(s => s.StoryInteraction.Like)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(s => new TopStoryDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryCategories = s.Categories.Take(1).Select(c => new CategoryShelfDto
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName
                    }).ToList(),
                    StoryAuthor = new MinimalAuthorDto { UserId = s.Author.UserId, UserFullname = s.Author.UserFullname },
                    StoryCreateTime = s.CreateTime,
                    StoryChapterNumber = s.Chapters.Count,
                })
                .ToListAsync();
            return new PaginatedResult<TopStoryDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: stories);
        }

        public async Task<PaginatedResult<TopStoryDto>> GetMinimalTopLatestStories(int page, int pageSize)
        {
            var baseQuery = _context.Stories
                .AsNoTracking()
                .Where(c => c.Status > 0);

            int totalCount = await baseQuery.CountAsync();

            var stories = await baseQuery
                .Include(c => c.Author)
                .Include(c => c.Categories)
                .OrderByDescending(c => c.StoryId)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(s => new TopStoryDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryCategories = s.Categories.Take(1).Select(c => new CategoryShelfDto
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName
                    }).ToList(),
                    StoryAuthor = new MinimalAuthorDto { UserId = s.Author.UserId, UserFullname = s.Author.UserFullname },
                    StoryCreateTime = s.CreateTime,
                })
                .ToListAsync();
            return new PaginatedResult<TopStoryDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: stories);
        }

        public async Task<PaginatedResult<TopStoryDto>> GetMinimalTopLatestStoriesByChapter(int page, int pageSize)
        {
            var baseQuery = _context.Stories
                .AsNoTracking()
                .Where(s => s.Status > 0);

            int totalCount = await baseQuery.CountAsync();

            var stories = await baseQuery
                .Include(s => s.Author)
                .Include(s => s.Categories)
                .Include(s => s.Chapters.Where(ch => ch.Status > 0))
                .OrderByDescending(s => s.Chapters.OrderByDescending(c => c.ChapterNumber).FirstOrDefault().ChapterId)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(s => new TopStoryDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryCategories = s.Categories.Take(1).Select(c => new CategoryShelfDto
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName
                    }).ToList(),
                    StoryAuthor = new MinimalAuthorDto { UserId = s.Author.UserId, UserFullname = s.Author.UserFullname },
                    StoryCreateTime = s.CreateTime,
                    StoryChapterNumber = s.Chapters.Count
                })
                .ToListAsync();
            return new PaginatedResult<TopStoryDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: stories);
        }

        public async Task<PaginatedResult<TopStoryDto>> GetMinimalTopStoriesRead(int page, int pageSize)
        {
            var baseQuery = _context.Stories
                .AsNoTracking()
                .Where(c => c.Status > 0);

            int totalCount = await baseQuery.CountAsync();

            var stories = await baseQuery
                .Include(c => c.StoryInteraction)
                .Include(c => c.Author)
                .Include(c => c.Categories)
                .OrderByDescending(c => c.StoryInteraction.Read)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(s => new TopStoryDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryCategories = s.Categories.Take(1).Select(c => new CategoryShelfDto
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName
                    }).ToList(),
                    StoryAuthor = new MinimalAuthorDto { UserId = s.Author.UserId, UserFullname = s.Author.UserFullname },
                    StoryCreateTime = s.CreateTime,
                    StoryChapterNumber = s.Chapters.Count,
                    StoryInteraction = new MinimalInteractionDto
                    {
                        Like = s.StoryInteraction.Like,
                        Follow = s.StoryInteraction.Follow,
                        View = s.StoryInteraction.View,
                        Read = s.StoryInteraction.Read
                    },
                })
                .ToListAsync();
            return new PaginatedResult<TopStoryDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: stories);
        }

        public async Task<PaginatedResult<TopStoryDto>> GetStoriesDoneEachCate(int categoryId, int page, int pageSize)
        {
            var baseQuery = _context.Stories
                .AsNoTracking()
                .Where(c => c.Status == 2 && c.Categories.Any(u => u.CategoryId == categoryId));

            int totalCount = await baseQuery.CountAsync();

            var stories = await baseQuery
                .Include(c => c.Users)
                .Include(c => c.Author)
                .Include(c => c.Categories)
                .Include(c => c.Chapters)
                .Include(c => c.StoryInteraction)
                .OrderByDescending(s => s.Chapters.OrderByDescending(c => c.ChapterNumber).FirstOrDefault().ChapterId)
                .ThenByDescending(s => s.StoryId)
                .ThenByDescending(s => s.StoryInteraction.Read)
                .ThenByDescending(s => s.StoryInteraction.Follow)
                .ThenByDescending(s => s.StoryInteraction.Like)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(s => new TopStoryDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryDescription = s.StoryDescription,
                    StoryCategories = s.Categories.Take(1).Select(c => new CategoryShelfDto
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName
                    }).ToList(),
                    StoryAuthor = new MinimalAuthorDto { UserId = s.Author.UserId, UserFullname = s.Author.UserFullname },
                    StoryCreateTime = s.CreateTime,
                    StoryChapterNumber = s.Chapters.Count,
                    StoryInteraction = new MinimalInteractionDto
                    {
                        Like = s.StoryInteraction.Like,
                        Follow = s.StoryInteraction.Follow,
                        View = s.StoryInteraction.View,
                        Read = s.StoryInteraction.Read
                    },
                })
                .ToListAsync();
            return new PaginatedResult<TopStoryDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: stories);
        }

        public async Task<PaginatedResult<TopStoryDto>> GetStoriesEachCate(int categoryId, int page, int pageSize)
        {
            var baseQuery = _context.Stories
                .AsNoTracking()
                .Where(c => c.Categories.Any(u => u.CategoryId == categoryId) && c.Status > 0);

            int totalCount = await baseQuery.CountAsync();

            var stories = await baseQuery
                .Include(c => c.Author)
                .Include(c => c.Chapters)
                .Include(c => c.StoryInteraction)
                .OrderByDescending(s => s.Chapters.OrderByDescending(c => c.ChapterNumber).FirstOrDefault().ChapterId)
                .ThenByDescending(s => s.StoryId)
                .ThenByDescending(s => s.StoryInteraction.Read)
                .ThenByDescending(s => s.StoryInteraction.Follow)
                .ThenByDescending(s => s.StoryInteraction.Like)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(s => new TopStoryDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryDescription = s.StoryDescription,
                    StoryAuthor = new MinimalAuthorDto { UserId = s.Author.UserId, UserFullname = s.Author.UserFullname },
                    StoryCreateTime = s.CreateTime,
                    StoryChapterNumber = s.Chapters.Count,
                    StoryLatestChapter = s.Chapters.Where(c => c.Status > 0).OrderByDescending(c => c.ChapterNumber).Select(c => new ChapterShelfDto
                    {
                        ChapterId = c.ChapterId,
                        ChapterNumber = c.ChapterNumber,
                        ChapterTitle = c.ChapterTitle,
                        CreateTime = c.CreateTime
                    }).FirstOrDefault()
                })
                .ToListAsync();
            return new PaginatedResult<TopStoryDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: stories);
        }

        public async Task<IEnumerable<TopStoryDto>> GetStoriesTopCate(int cateId)
        {
            var baseQuery = _context.Stories
                .AsNoTracking()
                .Where(c => c.Categories.Any(u => u.CategoryId == cateId) && c.Status > 0);

            int totalCount = await baseQuery.CountAsync();

            var stories = await baseQuery
                .Include(s => s.StoryInteraction)
                .OrderByDescending(s => s.StoryInteraction.Read)
                .ThenByDescending(s => s.StoryInteraction.Follow)
                .ThenByDescending(s => s.StoryInteraction.Like)
                .Take(5)
                .Select(s => new TopStoryDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryDescription = s.StoryDescription,
                    StoryCreateTime = s.CreateTime,
                })
                .ToListAsync();
            return stories;
        }

        public async Task<PaginatedResult<TopStoryDto>> GetTopFamousStories(int page, int pageSize)
        {
            var baseQuery = _context.Stories
                .AsNoTracking()
                .Where(c => c.Status > 0);

            int totalCount = await baseQuery.CountAsync();

            var stories = await baseQuery
                .Include(c => c.Author)
                .Include(c => c.Categories)
                .Include(c => c.Users)
                .Include(c => c.Chapters).ThenInclude(c => c.Users)
                .Include(c => c.StoryInteraction)
                .OrderByDescending(s => s.Users.Count)
                .ThenByDescending(s => s.Chapters.SelectMany(c => c.Users).Count())
                .ThenByDescending(s => s.StoryInteraction.Read)
                .ThenByDescending(s => s.StoryInteraction.Follow)
                .ThenByDescending(s => s.StoryInteraction.Like)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(s => new TopStoryDto
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
                    UserPurchaseStory = s.Users.Count,
                    UserPurchaseChapter = s.Chapters.SelectMany(c => c.Users).Count(),
                })
                .ToListAsync();
            return new PaginatedResult<TopStoryDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: stories);
        }

        public async Task<PaginatedResult<TopStoryDto>> GetTopLatestStories(int page, int pageSize)
        {
            var baseQuery = _context.Stories
                .AsNoTracking()
                .Where(c => c.Status > 0);

            int totalCount = await baseQuery.CountAsync();

            var stories = await baseQuery
                .Include(c => c.StoryInteraction)
                .Include(c => c.Author)
                .Include(c => c.Categories)
                .Include(c => c.Chapters)
                .OrderByDescending(c => c.StoryId)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(s => new TopStoryDto
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
                    }).FirstOrDefault()
                })
                .ToListAsync();
            return new PaginatedResult<TopStoryDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: stories);
        }

        public async Task<PaginatedResult<TopStoryDto>> GetTopLatestStoriesByChapter(int page, int pageSize)
        {
            var baseQuery = _context.Stories
                .AsNoTracking()
                .Where(s => s.Status > 0);

            int totalCount = await baseQuery.CountAsync();

            var stories = await baseQuery
                .Include(s => s.Author)
                .Include(s => s.Categories)
                .Include(s => s.Chapters.Where(ch => ch.Status > 0))
                .OrderByDescending(s => s.Chapters.OrderByDescending(c => c.ChapterNumber).FirstOrDefault().ChapterId)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(s => new TopStoryDto
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
                .ToListAsync();
            return new PaginatedResult<TopStoryDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: stories);
        }

        public async Task<PaginatedResult<TopStoryDto>> GetTopStoriesRead(int page, int pageSize)
        {
            var baseQuery = _context.Stories
                .AsNoTracking()
                .Where(c => c.Status > 0);

            int totalCount = await baseQuery.CountAsync();

            var stories = await baseQuery
                .Include(c => c.StoryInteraction)
                .Include(c => c.Author)
                .Include(c => c.Categories)
                .Include(c => c.Chapters)
                .OrderByDescending(c => c.StoryInteraction.Read)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(s => new TopStoryDto
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
                .ToListAsync();
            return new PaginatedResult<TopStoryDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: stories);
        }

        public async Task<IEnumerable<TopStoryDto>> GetTopStoriesReadShelves(int cateId)
        {
            var baseQuery = _context.Stories
                .AsNoTracking()
                .Where(c => c.Categories.Any(u => u.CategoryId == cateId) && c.Status > 0);

            int totalCount = await baseQuery.CountAsync();

            var stories = await baseQuery
                .Include(c => c.StoryInteraction)
                .Include(c => c.Author)
                .Include(c => c.Categories)
                .Include(c => c.Chapters)
                .OrderByDescending(c => c.StoryInteraction.Read)
                .Take(10)
                .Select(s => new TopStoryDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryCategories = s.Categories.Take(1).Select(c => new CategoryShelfDto
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName
                    }).ToList(),
                    StoryAuthor = new MinimalAuthorDto { UserId = s.Author.UserId, UserFullname = s.Author.UserFullname },
                    StoryCreateTime = s.CreateTime,
                    StoryChapterNumber = s.Chapters.Count,
                    StoryInteraction = new MinimalInteractionDto
                    {
                        Like = s.StoryInteraction.Like,
                        Follow = s.StoryInteraction.Follow,
                        View = s.StoryInteraction.View,
                        Read = s.StoryInteraction.Read
                    },
                })
                .ToListAsync();
            return stories;
        }
    }
}
