using EP.Application.Common.DTOs.Chapter;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Pagination;
using EP.Domain.Models;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Repositories
{
    public class ChapterRepository : Repository<Chapter>, IChapterRepository
    {
        public ChapterRepository(Context context) : base(context)
        {
        }

        public async Task<long> GetLastestChapterNumberInVoumeAsync(int storyId, long volumeId)
        {
            return await _dbSet
                    .Where(c => c.StoryId == storyId && c.VolumeId == volumeId && c.Status >= 0)
                    .MaxAsync(c => (long?)c.ChapterNumber) ?? 0;
        }

        public async Task<PaginatedResult<MinimalChapterDto>> GetStoryChapters(int storyId, int page, int pageSize)
        {
            var baseQuery = _dbSet
                .AsNoTracking()
                .Where(c => c.StoryId == storyId && c.Status > 0);

            int totalCount = await baseQuery.CountAsync();

            var chapters = await baseQuery
                .Include(c => c.Comments)
                .Include(c => c.Users)
                .OrderBy(c => c.ChapterNumber)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(c => new MinimalChapterDto
                {
                    ChapterId = c.ChapterId,
                    ChapterNumber = c.ChapterNumber,
                    ChapterTitle = c.ChapterTitle,
                    ChapterPrice = c.ChapterPrice,
                    CreateTime = c.CreateTime,
                    Comment = c.Comments.Count,
                    UserPurchaseChapter = c.Users.Count,
                })
                .ToListAsync();

            return new PaginatedResult<MinimalChapterDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: chapters);
        }

        public async Task RenumberChaptersAfterAddAsync(int storyId, long startChapterNumber)
        {
            var chapters = await _dbSet
                     .Where(c => c.StoryId == storyId && (c.Status >= 0 || c.Status == null) && c.ChapterNumber > startChapterNumber)
                     .OrderBy(c => c.Volume.VolumeNumber)
                     .ThenBy(c => c.ChapterNumber)
                     .ToListAsync();

            foreach (var chapter in chapters)
            {
                chapter.ChapterNumber++;
            }
        }

        public async Task<ChapterDto?> GetChapterInfor(int chapterId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.ChapterId == chapterId).Select(c => new ChapterDto
                {
                    ChapterId = c.ChapterId,
                    StoryId = c.Story.StoryId,
                    StoryTitle = c.Story.StoryTitle,
                    ChapterTitle = c.ChapterTitle,
                    ChapterContentHtml = c.ChapterContentHtml,
                    ChapterContentMarkdown = c.ChapterContentMarkdown,
                    ChapterNumber = c.ChapterNumber,
                    VolumeId = c.VolumeId,
                    ChapterPrice = c.ChapterPrice,

                }).FirstOrDefaultAsync();
        }
    }
}
