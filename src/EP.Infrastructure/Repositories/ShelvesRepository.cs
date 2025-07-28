using EP.Application.Common.DTOs.Author;
using EP.Application.Common.DTOs.Interaction;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces;
using EP.Application.Common.Pagination;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Infrastructure.Repositories
{
    public class ShelvesRepository : IShelvesRepository
    {
        private readonly Context _context;

        public ShelvesRepository(Context context)
        {
            _context = context;
        }
        public async Task<PaginatedResult<TopReadStoryDto>> GetMinimalTopStoriesRead(int page, int pageSize)
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
                .Select(s => new TopReadStoryDto
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
            return new PaginatedResult<TopReadStoryDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: stories);
        }

        public async Task<PaginatedResult<TopReadStoryDto>> GetTopStoriesRead(int page, int pageSize)
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
                .ToListAsync();
            return new PaginatedResult<TopReadStoryDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: stories);
        }
    }
}
