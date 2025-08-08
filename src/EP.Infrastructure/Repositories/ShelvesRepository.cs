using EP.Application.Common.DTOs.Author;
using EP.Application.Common.DTOs.Category;
using EP.Application.Common.DTOs.Interaction;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Pagination;
using EP.Domain.Models;
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

        public async Task<PaginatedResult<TopStoryDto>> GetFollowedStory(int userId, int page, int pageSize)
        {
            var baseQuery = _context.Stories
                .AsNoTracking()
                .Where(c => c.StoryFollowLikes.Any(u => u.UserId == userId && u.Follow == true) && c.Status > 0);

            int totalCount = await baseQuery.CountAsync();

            var stories = await baseQuery
                .Include(c => c.StoryFollowLikes)
                .Include(c => c.Chapters)
                .Include(c => c.StoryReads).ThenInclude(c => c.Chapter)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(s => new TopStoryDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryDescription = s.StoryDescription,
                    StoryCreateTime = s.CreateTime,
                    StoryChapterNumber = s.Chapters.Count,
                    StoryLatestChapter = s.Chapters.Where(c => c.Status > 0).OrderByDescending(c => c.ChapterNumber).Select(c => new ChapterShelfDto
                    {
                        ChapterId = c.ChapterId,
                        ChapterNumber = c.ChapterNumber,
                        ChapterTitle = c.ChapterTitle,
                        CreateTime = c.CreateTime
                    }).FirstOrDefault(),
                    StoryReadChapter = s.StoryReads
                                    .Where(c => c.UserId == userId && s.StoryId == c.StoryId)
                                    .OrderByDescending(sr => sr.ReadTime)
                                    .Select(c => new StoryReadChapter
                                    { 
                                        ChapterId = c.ChapterId, 
                                        ChapterNumber = c.Chapter.ChapterNumber, 
                                        ChapterTitle = c.Chapter.ChapterTitle, 
                                        CreateTime = c.Chapter.CreateTime, 
                                        ReadTime = c.ReadTime 
                                    }).FirstOrDefault(),
                    StoryPrice = s.StoryPrice,
                })
                .ToListAsync();

            return new PaginatedResult<TopStoryDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: stories);
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

        public async Task<PaginatedResult<TopStoryDto>> GetOwnedStory(int userId, int page, int pageSize)
        {
            var baseQuery = _context.Stories
                .AsNoTracking()
                .Where(c => c.Users.Any(u => u.UserId == userId) && c.Status > 0);

            int totalCount = await baseQuery.CountAsync();


            var stories = await baseQuery
                .Include(c => c.Author)
                .Include(c => c.Chapters)
                .Include(c => c.StoryReads)
                .Include(c => c.StoryInteraction)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(s => new TopStoryDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
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
                    StoryPrice = s.StoryPrice,
                })
                .ToListAsync();

            return new PaginatedResult<TopStoryDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: stories);
        }

        public async Task<PaginatedResult<TopStoryDto>> GetReadHistory(int userId, int page, int pageSize)
        {
            var baseQuery = _context.Stories
                .AsNoTracking()
                .Where(c => c.StoryReads.Any(u => u.UserId == userId) && c.Status > 0);

            int totalCount = await baseQuery.CountAsync();


            var stories = await baseQuery
                .Include(c => c.Author)
                .Include(c => c.Chapters)
                .Include(c => c.StoryReads).ThenInclude(c => c.Chapter)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(s => new TopStoryDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryDescription = s.StoryDescription,
                    StoryCreateTime = s.CreateTime,
                    StoryChapterNumber = s.Chapters.Count,
                    StoryLatestChapter = s.Chapters.Where(c => c.Status > 0).OrderByDescending(c => c.ChapterNumber).Select(c => new ChapterShelfDto
                    {
                        ChapterId = c.ChapterId,
                        ChapterNumber = c.ChapterNumber,
                        ChapterTitle = c.ChapterTitle,
                        CreateTime = c.CreateTime
                    }).FirstOrDefault(),
                    StoryReadChapter = s.StoryReads
                                    .Where(c => c.UserId == userId && s.StoryId == c.StoryId)
                                    .OrderByDescending(sr => sr.ReadTime)
                                    .Select(c => new StoryReadChapter
                                    {
                                        ChapterId = c.ChapterId,
                                        ChapterNumber = c.Chapter.ChapterNumber,
                                        ChapterTitle = c.Chapter.ChapterTitle,
                                        CreateTime = c.Chapter.CreateTime,
                                        ReadTime = c.ReadTime
                                    }).FirstOrDefault(),
                    StoryPrice = s.StoryPrice,
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

        public async Task<IEnumerable<CategoryWithStoryDto>> GetStoriesInCategoryShelf()
        {
            return await _context.Categories
            .AsNoTracking()
            .Select(c => new CategoryWithStoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Stories = c.Stories
                    .Where(s => s.Status >= 0)
                    .OrderByDescending(s => s.StoryInteraction.Read)
                    .Take(10)
                    .Select(s => new StoryInCategoryShelfDto
                    {
                        StoryId = s.StoryId,
                        StoryTitle = s.StoryTitle,
                        StoryInteraction = new MinimalInteractionDto
                        {
                            Like = s.StoryInteraction.Like,
                            Follow = s.StoryInteraction.Follow,
                            View = s.StoryInteraction.View,
                            Read = s.StoryInteraction.Read,
                        },
                        StoryCreateTime = s.CreateTime,
                    })
                    .ToList()
            })
            .Where(c => c.Stories.Any())
            .ToListAsync();
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

        public async Task<PaginatedResult<StoryOfAuthorDto>> GetStoryOfAuthor(int authorId, string? title, string? sort, int page, int pageSize)
        {
            var baseQuery = _context.Stories
                    .AsNoTracking()
                    .Where(c => c.AuthorId == authorId && c.Status >= 0);

            if (!string.IsNullOrEmpty(title))
            {
                baseQuery = baseQuery.Where(c => c.StoryTitle.ToLower().Contains(title.ToLower()));
            }
            int totalCount = await baseQuery.CountAsync();

            baseQuery = baseQuery.Include(c => c.Categories)
                    .Include(c => c.Users)
                    .Include(c => c.Chapters).ThenInclude(c => c.Users)
                    .Include(c => c.StoryInteraction);

            if (!string.IsNullOrEmpty(sort))
            {
                bool isDescending = sort.StartsWith("-");
                string sortField = isDescending ? sort.Substring(1) : sort;

                switch (sortField.ToLower())
                {
                    case "storytitle":
                        baseQuery = isDescending
                            ? baseQuery.OrderByDescending(c => c.StoryTitle)
                            : baseQuery.OrderBy(c => c.StoryTitle);
                        break;

                    case "userpurchasestory":
                        baseQuery = isDescending
                            ? baseQuery.OrderByDescending(c => c.Users.Count + c.Chapters.SelectMany(ch => ch.Users).Count())
                            : baseQuery.OrderBy(c => c.Users.Count + c.Chapters.SelectMany(ch => ch.Users).Count());
                        break;

                    case "storycreatetime":
                        baseQuery = isDescending
                            ? baseQuery.OrderByDescending(c => c.CreateTime)
                            : baseQuery.OrderBy(c => c.CreateTime);
                        break;

                    default:
                        baseQuery = baseQuery.OrderByDescending(c => c.StoryId);
                        break;
                }
            }
            else
            {
                baseQuery = baseQuery.OrderByDescending(c => c.StoryId);
            }

            var stories = await baseQuery
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(c => new StoryOfAuthorDto
                {
                    StoryId = c.StoryId,
                    StoryTitle = c.StoryTitle,
                    StoryImage = c.StoryImage,
                    StoryCreateTime = c.CreateTime,
                    StoryInteraction = new MinimalInteractionDto
                    {
                        Like = c.StoryInteraction.Like,
                        Follow = c.StoryInteraction.Follow,
                        View = c.StoryInteraction.View,
                        Read = c.StoryInteraction.Read,
                    },
                    StoryStatus = c.Status,
                    UserPurchaseStory = c.Users.Count + c.Chapters.SelectMany(ch => ch.Users).Count(),
                    UserPurchaseChapter = c.Chapters.SelectMany(ch => ch.Users).Count(),
                    ChapterNum = c.Chapters.Count,
                })
                .ToListAsync();

            return new PaginatedResult<StoryOfAuthorDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: stories);
        }

        public async Task<IEnumerable<TopAuthorRevenueDto>> GetTop6AuthorRevenue()
        {
            var topAuthors = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.WalletId != null)
                .GroupBy(t => t.WalletId)
                .Select(g => new
                {
                    WalletId = g.Key,
                    Revenue = g.Sum(t => t.Amount)
                })
                .OrderByDescending(g => g.Revenue)
                .Take(6)
                .Join(_context.Wallets.Include(w => w.User),
                    t => t.WalletId,
                    w => w.WalletId,
                    (t, w) => new TopAuthorRevenueDto
                    {
                        Author = new TopAuthorDto
                        {
                            AuthorFullname = w.User.UserFullname,
                            AuthorEmail = w.User.Email,
                            AuthorImage = w.User.UserImage
                        },
                        Revenue = t.Revenue * 1000
                    })
                .ToListAsync();

            return topAuthors;
        }

        public async Task<IEnumerable<TopStoryDto>> GetTop6StoriesPurchase()
        {
            var stories = await _context.Stories
                .AsNoTracking()
                .Where(c => c.Status > 0)
                .Include(c => c.Author)
                .Include(c => c.Users).Include(c => c.StoryInteraction)
                .Include(c => c.Chapters).ThenInclude(c => c.Users)
                .OrderByDescending(s => s.Users.Count)
                .ThenByDescending(s => s.Chapters.SelectMany(c => c.Users).Count())
                .Take(6)
                .Select(s => new TopStoryDto
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
                })
                .ToListAsync();
            return stories;
        }

        public async Task<IEnumerable<TopSaleDto>> GetTop6StoriesSale()
        {
            var topStories = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.StoryId != null)
                .GroupBy(t => t.StoryId)
                .Select(g => new
                {
                    StoryId = g.Key.Value,
                    Revenue = g.Sum(t => t.Amount)
                })
                .OrderByDescending(g => g.Revenue)
                .Take(6)
                .Join(_context.Stories,
                    t => t.StoryId,
                    s => s.StoryId,
                    (t, s) => new TopSaleDto
                    {
                        Story = new StorySaleInforDto
                        {
                            StoryId = s.StoryId,
                            StoryTitle = s.StoryTitle,
                            StoryImage = s.StoryImage,
                            AuthorName = s.Author.UserFullname
                        },
                        Revenue = t.Revenue * 1000
                    })
                .ToListAsync();

            return topStories;
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
        public async Task<PaginatedResult<TopStoryDto>> FilterStory(string? title, int? to, int? from, string? sort, List<int> cates, int? status, int page, int pageSize)
        {
            var baseQuery = _context.Stories
                .AsNoTracking()
                .Where(c => c.Status > 0);

            int totalCount = await baseQuery.CountAsync();
                   

            // Áp dụng các bộ lọc
            if (!string.IsNullOrEmpty(title))
            {
                baseQuery = baseQuery.Where(c => c.StoryTitle.ToLower().Contains(title.ToLower()));
            }

            if (from != null || to != null)
            {
                baseQuery = baseQuery.Where(c =>
                    (from == null || c.StoryPrice >= from) &&
                    (to == null || c.StoryPrice <= to));
            }

            if (cates != null && cates.Count > 0)
            {
                baseQuery = baseQuery.Where(c => c.Categories.Any(cat => cates.Contains(cat.CategoryId)));
            }

            if (status != null)
            {
                baseQuery = baseQuery.Where(c => c.Status == status);
            }

            baseQuery = baseQuery
                   .Include(c => c.Author)
                   .Include(c => c.Categories)
                   .Include(c => c.Chapters.Where(ch => ch.Status > 0));

            // Áp dụng sắp xếp
            if (!string.IsNullOrEmpty(sort))
            {
                baseQuery = sort == "sort"
                    ? baseQuery.OrderBy(c => c.StoryPrice)
                    : baseQuery.OrderByDescending(c => c.StoryPrice);
            }
            else
            {
                baseQuery = baseQuery
                    .OrderByDescending(c => c.Chapters.Max(ch => (int?)ch.ChapterId))
                    .ThenByDescending(c => c.StoryId);
            }

            
            // Thực hiện truy vấn và ánh xạ kết quả
            var stories = await baseQuery
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
                    StoryPrice = s.StoryPrice,
                    //Status = s.Status
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

        public async Task<IEnumerable<TopStoryDto>> GetTopFamousStoryOfAuthor(int authorId)
        {
            return await _context.Stories.Where(s => s.AuthorId == authorId && s.Status > 0)
                .Include(c => c.Users)
                .Include(c => c.Author)
                .Include(c => c.Chapters).ThenInclude(c => c.Users)
                .Include(c => c.StoryInteraction)
                .Select(s => new TopStoryDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryDescription = s.StoryDescription,
                    StoryDescriptionHtml = s.StoryDescriptionHtml,
                    StoryDescriptionMarkdown = s.StoryDescriptionMarkdown,
                    StoryCreateTime = s.CreateTime,
                    StoryChapterNumber = s.Chapters.Count,
                    StoryPrice = s.StoryPrice,
                    StoryInteraction = new MinimalInteractionDto
                    {
                        Like = s.StoryInteraction.Like,
                        Follow = s.StoryInteraction.Follow,
                        View = s.StoryInteraction.View,
                        Read = s.StoryInteraction.Read
                    },
                    UserPurchaseStory = s.Users.Count,
                    UserPurchaseChapter = s.Chapters.SelectMany(c => c.Users).Count(),
                }).OrderByDescending(s => s.UserPurchaseStory) // top famous compare
                .ThenByDescending(s => s.UserPurchaseChapter)
                .ThenByDescending(s => s.StoryInteraction.Read).ThenByDescending(s => s.StoryInteraction.Follow)
                .ThenByDescending(s => s.StoryInteraction.Like)
                .ToListAsync();
        }

        public async Task<IEnumerable<TopStoryDto>> GetTopPurchaseStoryOfAuthor(int authorId)
        {
            return await _context.Stories.Where(s => s.AuthorId == authorId && s.Status > 0)
                .Include(c => c.Users)
                .Include(c => c.Author)
                .Include(c => c.Chapters).ThenInclude(c => c.Users)
                .Include(c => c.StoryInteraction)
                .Select(s => new TopStoryDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryDescription = s.StoryDescription,
                    StoryDescriptionHtml = s.StoryDescriptionHtml,
                    StoryDescriptionMarkdown = s.StoryDescriptionMarkdown,
                    StoryCreateTime = s.CreateTime,
                    StoryChapterNumber = s.Chapters.Count,
                    StoryPrice = s.StoryPrice,
                    StoryInteraction = new MinimalInteractionDto
                    {
                        Like = s.StoryInteraction.Like,
                        Follow = s.StoryInteraction.Follow,
                        View = s.StoryInteraction.View,
                        Read = s.StoryInteraction.Read
                    },
                    UserPurchaseStory = s.Users.Count,
                    UserPurchaseChapter = s.Chapters.SelectMany(c => c.Users).Count(),
                }).OrderByDescending(s => s.UserPurchaseStory) // top famous compare
                .ThenByDescending(s => s.UserPurchaseChapter)
                .ToListAsync();
        }

        public async Task<IEnumerable<TopStoryDto>> GetNewestStoryOfAuthor(int authorId)
        {
            return await _context.Stories.Where(s => s.AuthorId == authorId && s.Status > 0)
                .Include(c => c.Users)
                .Include(c => c.Author)
                .Include(c => c.Chapters).ThenInclude(c => c.Users)
                .Include(c => c.StoryInteraction)
                .Select(s => new TopStoryDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryDescription = s.StoryDescription,
                    StoryDescriptionHtml = s.StoryDescriptionHtml,
                    StoryDescriptionMarkdown = s.StoryDescriptionMarkdown,
                    StoryCreateTime = s.CreateTime,
                    StoryChapterNumber = s.Chapters.Count,
                    StoryPrice = s.StoryPrice,
                    StoryInteraction = new MinimalInteractionDto
                    {
                        Like = s.StoryInteraction.Like,
                        Follow = s.StoryInteraction.Follow,
                        View = s.StoryInteraction.View,
                        Read = s.StoryInteraction.Read
                    },
                    UserPurchaseStory = s.Users.Count,
                    UserPurchaseChapter = s.Chapters.SelectMany(c => c.Users).Count(),
                }).OrderByDescending(s => s.StoryLatestChapter.ChapterId)
                .ThenByDescending(s => s.StoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TopStoryDto>> GetWrittenStoryOfAuthor(int authorId)
        {
            return await _context.Stories.Where(s => s.AuthorId == authorId && s.Status > 0)
                .Include(c => c.StoryInteraction)
                .Select(s => new TopStoryDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryDescription = s.StoryDescription,
                    StoryDescriptionHtml = s.StoryDescriptionHtml,
                    StoryDescriptionMarkdown = s.StoryDescriptionMarkdown,
                    StoryCreateTime = s.CreateTime,
                    StoryChapterNumber = s.Chapters.Count,
                    StoryPrice = s.StoryPrice,
                    StoryInteraction = new MinimalInteractionDto
                    {
                        Like = s.StoryInteraction.Like,
                        Follow = s.StoryInteraction.Follow,
                        View = s.StoryInteraction.View,
                        Read = s.StoryInteraction.Read
                    },
                }).OrderByDescending(c => c.StoryId)
                .ToListAsync();
        }
    }
}
