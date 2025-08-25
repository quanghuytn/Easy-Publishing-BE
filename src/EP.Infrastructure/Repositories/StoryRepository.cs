using EP.Application.Common.DTOs.Author;
using EP.Application.Common.DTOs.Chapter;
using EP.Application.Common.DTOs.Interaction;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.DTOs.Story;
using EP.Application.Common.DTOs.Transaction;
using EP.Application.Common.DTOs.Volume;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Pagination;
using EP.Domain.Models;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Repositories
{
    public class StoryRepository : Repository<Story>, IStoryRepository
    {
        public StoryRepository(Context context) : base(context)
        {
        }

        public async Task<IEnumerable<StoryListDto>> GetAllStories()
        {
            return await _dbSet
                .AsNoTracking()
                .Select(c => new StoryListDto
                {
                    StoryId = c.StoryId,
                    StoryTitle = c.StoryTitle,
                    StoryImage = c.StoryImage,
                    StoryDescription = c.StoryDescriptionHtml.Substring(0, 90) + "...",
                    StoryPrice = c.StoryPrice,
                    StorySale = c.StorySale,
                    CreateTime = c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    StoryCategories = string.Join(",", c.Categories.Select(c => c.CategoryName).ToList()),
                    StoryAuthor = c.Author.UserFullname,
                    StoryChapterNumber = c.Chapters.Count,
                    StoryChapters = c.Chapters.Where(c => c.Status > 0).Count(),
                    StoryReads = c.StoryReads.Count(),
                    Volumes = c.Volumes.Count(),
                    UserOwned = c.Users.Count(),
                    Status = c.Status,
                    UserFollow = c.StoryFollowLikes.Where(c => c.Follow == true).Count(),
                    UserLike = c.StoryFollowLikes.Where(c => c.Like == true).Count()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TopStoryDto>> GetRelatedStories(int storyId)
        {
            var categoryIds = await _dbSet
                    .Where(s => s.StoryId == storyId)
                    .SelectMany(s => s.Categories.Select(cat => cat.CategoryId))
                    .ToListAsync();

            return await _dbSet
                                .Where(s => s.StoryId != storyId
                                            && s.Status > 0
                                            && s.Categories.Any(cat => categoryIds.Contains(cat.CategoryId))
                                            && s.Chapters.Any(ch => ch.Status > 0)) // lọc có chapter hợp lệ
                                .OrderByDescending(s => s.StoryId)
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
                                    StoryChapterNumber = s.Chapters.Count(ch => ch.Status > 0)
                                })
                                .Take(3) // Lấy tối đa 3 truyện
                                .ToListAsync();
        }

        public async Task<StoryDetailDto?> GetStoryDetail(int storyId, int userId)
        {
            return await _dbSet
                    .Where(s => s.StoryId == storyId && s.Status > 0)
                    .Select(s => new StoryDetailDto
                    {
                        StoryId = s.StoryId,
                        StoryTitle = s.StoryTitle,
                        StoryImage = s.StoryImage,
                        StoryDescription = s.StoryDescriptionHtml,
                        StoryPrice = s.StoryPrice,
                        StorySale = s.StorySale,
                        CreateTime = s.CreateTime,
                        StoryCategories = s.Categories.Select(cat => new CategoryShelfDto
                        {
                            CategoryId = cat.CategoryId,
                            CategoryName = cat.CategoryName
                        }).ToList(),
                        StoryAuthor = new MinimalAuthorDto
                        {
                            UserId = s.Author.UserId,
                            UserFullname = s.Author.UserFullname
                        },
                        StoryChapterNumber = s.Chapters.Count(ch => ch.Status > 0),
                        StoryChapters = s.Chapters
                            .Where(ch => ch.Status > 0)
                            .OrderByDescending(ch => ch.ChapterNumber)
                            .Take(3)
                            .Select(ch => new ChapterShelfDto
                            {
                                ChapterId = ch.ChapterId,
                                ChapterNumber = ch.ChapterNumber,
                                ChapterTitle = ch.ChapterTitle,
                                CreateTime = ch.CreateTime
                            }).ToList(),
                        UserPurchaseStory = s.Users.Count(),
                        StoryInteraction = new MinimalInteractionDto
                        {
                            Like = s.StoryInteraction.Like,
                            Follow = s.StoryInteraction.Follow,
                            View = s.StoryInteraction.View,
                            Read = s.StoryInteraction.Read
                        },
                        AuthorOwned = userId == s.AuthorId,
                        UserOwned = s.Users.Any(u => u.UserId == userId),
                        UserFollow = s.StoryFollowLikes.Any(f => f.UserId == userId && f.Follow == true),
                        UserLike = s.StoryFollowLikes.Any(f => f.UserId == userId && f.Like == true)
                    })
                    .FirstOrDefaultAsync();
        }

        public async Task<StoryPrintDto?> GetStoryForPrint(int storyId, int authorId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(s => s.StoryId == storyId && s.AuthorId == authorId)
                    .Select(c => new StoryPrintDto
                    {
                        StoryTitle = c.StoryTitle,
                        StoryImage = c.StoryImage,
                        StoryDescription = c.StoryDescription,
                        StoryDescriptionHtml = c.StoryDescriptionHtml,
                        StoryDescriptionMarkdown = c.StoryDescriptionMarkdown,
                        StoryPrice = c.StoryPrice,
                        StoryVolumes = c.Volumes.Select(s => new VolumeWithChapterDto
                        {
                            VolumeNumber = s.VolumeNumber,
                            VolumeTitle = s.VolumeTitle,
                            VolumeChapters = s.Chapters.Select(ch => new ChapterDto
                            {
                                ChapterNumber = ch.ChapterNumber,
                                ChapterTitle = ch.ChapterTitle,
                                ChapterContentMarkdown = ch.ChapterContentMarkdown,
                                ChapterContentHtml = ch.ChapterContentHtml
                            }).ToList()
                        }).ToList(),
                    }).FirstOrDefaultAsync();
        }

        public async Task<StoryInformationDto?> GetStoryInformation(int storyId, int authorId)
        {
            return await _dbSet
                .Where(s => s.StoryId == storyId && s.AuthorId == authorId)
                .Select(s => new StoryInformationDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryDescription = s.StoryDescription,
                    StoryDescriptionMarkdown = s.StoryDescriptionMarkdown,
                    StoryDescriptionHtml = s.StoryDescriptionHtml,
                    StoryCategories = s.Categories.Select(c => new CategoryShelfDto { CategoryId = c.CategoryId, CategoryName = c.CategoryName }).ToList(),
                    StoryImage = s.StoryImage,
                    StoryPrice = s.StoryPrice,
                    StorySale = s.StorySale,
                    Status = s.Status,
                    Reviewed = s.Chapters.Any(c => c.Status == 1)
                }).FirstOrDefaultAsync();
        }

        public async Task<PaginatedResult<StoryReviewDto>> GetStoryReview(int userId, int page, int pageSize)
        {
            var baseQuery = _dbSet
                .AsNoTracking()
                .Where(s => s.Chapters.Any(c => c.Status == 0) && s.AuthorId != userId);

            int totalCount = await baseQuery.CountAsync();

            var stories = await baseQuery
                .Include(s => s.Categories)
                .Include(s => s.Users)
                .Include(s => s.Chapters).ThenInclude(c => c.Users)
                .Include(s => s.StoryInteraction)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(s => new StoryReviewDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryCreateTime = s.CreateTime,
                    StoryStatus = s.Status,
                    StoryInteraction = new MinimalInteractionDto
                    {
                        Like = s.StoryInteraction.Like,
                        Follow = s.StoryInteraction.Follow,
                        View = s.StoryInteraction.View,
                        Read = s.StoryInteraction.Read,
                    },
                    UserPurchaseStory = s.Users.Count,
                    UserPurchaseChapter = s.Chapters.SelectMany(c => c.Users).Count(),
                })
                .ToListAsync();

            return new PaginatedResult<StoryReviewDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: stories);
        }

        public async Task<IEnumerable<StoryReviewAdminDto>> GetStoryReviewAdmin()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(s => s.Author)
                .Include(s => s.Volumes).ThenInclude(v => v.Chapters)
                .Where(s => s.Chapters.Any(c => c.Status == 0))
                .Select(s => new StoryReviewAdminDto
                {
                    Tt_key = s.StoryId + 0.1,
                    Tt_parent = 0,
                    StoryId = s.StoryId,
                    Title = s.StoryTitle,
                    CreateTime = s.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Status = s.Status,
                    Author = s.Author.Username,
                    Volumes = s.Volumes.Where(v => v.StoryId == s.StoryId && v.Chapters.Any(c => c.Status == 0)).Select(v => new VolumeReviewAdminDto
                    {
                        Tt_key = v.VolumeId + 0.2,
                        Tt_parent = v.StoryId + 0.1,
                        VolumeId = v.VolumeId,
                        VolumeNumber = v.VolumeNumber,
                        Title = "Volume " + v.VolumeNumber + ": " + v.VolumeTitle,
                        CreateTime = v.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        Chapters = v.Chapters.Where(c => c.VolumeId == v.VolumeId && c.Status == 0).Select(c => new ChapterReviewAdminDto
                        {
                            Tt_key = c.ChapterId,
                            Tt_parent = c.VolumeId + 0.2,
                            ChapterId = c.ChapterId,
                            ChapterNumber = c.ChapterNumber,
                            Title = "Chaper " + c.ChapterNumber + ": " + c.ChapterTitle,
                            CreateTime = c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss")
                        }).OrderBy(c => c.ChapterNumber).ToList()
                    }).OrderBy(v => v.VolumeNumber).ToList()
                })
                .ToListAsync();
        }
        public async Task<AuthorAndStoryNumberDto?> GetAuthorAndStoryNumber()
        {
            return await _dbSet
                   .GroupBy(s => 1)
                   .Select(g => new AuthorAndStoryNumberDto
                   {
                       AuthorNumber = g.Select(s => s.AuthorId).Distinct().Count(),
                       StoryNumber = g.Count()
                   })
                   .FirstOrDefaultAsync();
        }
        public async Task<Story?> GetStoryWithCategory(int storyId, int authorId)
        {
            return await _dbSet
                .Include(s => s.Categories)
                .FirstOrDefaultAsync(s => s.StoryId == storyId && s.AuthorId == authorId);
        }

        public async Task<IEnumerable<TopStoryDto>> SearchGlobal(string? search, int? authorId, int? from, int? to, int? status, List<int> cates)
        {
            if (search != null)
            {
                search = search.ToLower();
            }

            var query = _dbSet
               .Where(s => s.Status > 0)
               .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(s => s.StoryTitle.ToLower().Contains(search));
            }

            if (authorId.HasValue)
                query = query.Where(s => s.AuthorId == authorId);

            if (status.HasValue)
                query = query.Where(s => s.Status == status);

            if (from.HasValue)
                query = query.Where(s => s.StoryPrice >= from);

            if (to.HasValue)
                query = query.Where(s => s.StoryPrice <= to);

            if (cates != null && cates.Count > 0)
            {
                query = query.Where(s => s.Categories.Any(c => cates.Contains(c.CategoryId)));
            }

            return await query
               .OrderByDescending(s => s.StoryInteraction.Read)
               .Take(10) // Giới hạn kết quả trả về
               .Select(s => new TopStoryDto
               {
                   StoryId = s.StoryId,
                   StoryTitle = s.StoryTitle,
                   StoryImage = s.StoryImage,
                   StoryDescription = s.StoryDescription,
                   StoryCategories = s.Categories
                            .Select(c => new CategoryShelfDto
                            {
                                CategoryId = c.CategoryId,
                                CategoryName = c.CategoryName
                            }).ToList(),
                   StoryAuthor = new MinimalAuthorDto
                   {
                       UserId = s.Author.UserId,
                       UserFullname = s.Author.UserFullname
                   },
                   StoryCreateTime = s.CreateTime,
                   StoryPrice = s.StoryPrice,
               })
               .ToListAsync();
        }

        public async Task<StoryPurchaseDto?> GetStoryPurchaseInfoAsync(int storyId)
        {
            return await _dbSet
                .Where(s => s.StoryId == storyId)
                .Select(s => new StoryPurchaseDto
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryPrice = s.StoryPrice,
                    StorySale = s.StorySale,
                    AuthorId = s.AuthorId,
                    AuthorWallet = s.Author.Wallets.Select(w =>
                        new AuthorWalletDto
                        {
                            WalletId = w.WalletId,
                            Refund = w.Refund
                        }
                    ).FirstOrDefault()
                })
                .FirstOrDefaultAsync();
        }
    }
}
