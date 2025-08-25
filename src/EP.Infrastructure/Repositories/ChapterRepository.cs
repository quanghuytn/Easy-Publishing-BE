using EP.Application.Common.DTOs.Author;
using EP.Application.Common.DTOs.Chapter;
using EP.Application.Common.DTOs.Story;
using EP.Application.Common.DTOs.Transaction;
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

        /// <summary>
        /// Renumber chapters after adding or deleting a chapter.
        /// </summary>
        /// <param name="storyId">Story id</param>
        /// <param name="startChapterNumber">chapter number immediately preceding the chapters which need to be renumbered</param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        public async Task RenumberChaptersAfterAddAsync(int storyId, long startChapterNumber, bool isDelete = false)
        {
            var chapters = await _dbSet
                     .Where(c => c.StoryId == storyId && (c.Status >= 0 || c.Status == null) && c.ChapterNumber > startChapterNumber)
                     .OrderBy(c => c.Volume.VolumeNumber)
                     .ThenBy(c => c.ChapterNumber)
                     .ToListAsync();
            long newNumber = isDelete ? startChapterNumber : startChapterNumber + 2;

            foreach (var chapter in chapters)
            {
                chapter.ChapterNumber = newNumber++;
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

        public async Task<ChapterContentDto?> GetChapterContent(int userId, long chapterNumber, int storyId, bool hasPurchased)
        {
            var nextChapterNum = await GetNextChapterAsync(chapterNumber, storyId);
            var prevChapterNum = await GetPreviousChapterAsync(chapterNumber, storyId);

            var chapter = await _dbSet
                        .AsNoTracking()
                        .Where(c => c.StoryId == storyId && c.ChapterNumber == chapterNumber && c.Status > 0)
                        .Include(c => c.Story)
                            .ThenInclude(s => s.Author)
                        .Include(c => c.Comments)
                        .Include(c => c.ChapterLikeds)
                        .Select(c => new ChapterContentDto
                        {
                            Story = new StoryChapterDto
                            {
                                StoryId = c.StoryId,
                                StoryTitle = c.Story.StoryTitle,
                                StoryPrice = c.Story.StoryPrice
                            },
                            Author = new MinimalAuthorDto
                            {
                                UserId = c.Story.Author.UserId,
                                UserFullname = c.Story.Author.UserFullname
                            },
                            Content = (c.ChapterPrice == 0 || c.ChapterPrice == null || userId == c.Story.Author.UserId || hasPurchased)
                                        ? c.ChapterContentHtml
                                        : null,
                            ChapterId = c.ChapterId,
                            ChapterNumber = c.ChapterNumber,
                            ChapterTitle = c.ChapterTitle,
                            ChapterPrice = c.ChapterPrice,
                            CreateTime = c.CreateTime,
                            UpdateTime = c.UpdateTime,
                            Comment = c.Comments.Count,
                            UserPurchaseChapter = c.Users.Count,
                            PreviousChapterNumber = prevChapterNum,
                            NextChapterNumber = nextChapterNum,
                            Owned = (c.ChapterPrice == 0 || c.ChapterPrice == null || userId == c.Story.Author.UserId || hasPurchased),
                            UserLike = c.ChapterLikeds.Any(cl => cl.UserId == userId)
                        }).FirstOrDefaultAsync();

            return chapter;
        }

        private async Task<long> GetNextChapterAsync(long currentChapterNumber, int storyId)
        {
            return await _dbSet
                .Where(c => c.StoryId == storyId && c.ChapterNumber > currentChapterNumber && c.Status > 0)
                .OrderBy(c => c.ChapterNumber)
                .Select(c => (long?)c.ChapterNumber)
                .FirstOrDefaultAsync() ?? -1;
        }

        private async Task<long> GetPreviousChapterAsync(long currentChapterNumber, int storyId)
        {
            return await _dbSet
                .Where(c => c.StoryId == storyId && c.ChapterNumber < currentChapterNumber && c.Status > 0)
                .OrderByDescending(c => c.ChapterNumber)
                .Select(c => (long?)c.ChapterNumber)
                .FirstOrDefaultAsync() ?? -1;
        }

        public async Task<PaginatedResult<ChapterReviewDto>> GetChapterNotReviewOfAuthor(int authorId, int page, int pageSize)
        {
            var baseQuery = _dbSet
                .AsNoTracking()
                .Where(c => (c.Status == 0 || c.Status == null) && c.Story.AuthorId == authorId);

            int totalCount = await baseQuery.CountAsync();

            var chapters = await baseQuery
                .OrderBy(v => v.CreateTime)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(c => new ChapterReviewDto
                {
                    StoryId = c.StoryId,
                    ChapterId = c.ChapterId,
                    VolumeId = c.VolumeId,
                    StoryTitle = c.Story.StoryTitle,
                    VolumeTitle = c.Volume.VolumeTitle,
                    VolumeNumber = c.Volume.VolumeNumber,
                    ChapterTitle = c.ChapterTitle,
                    ChapterNumber = c.ChapterNumber,
                    CreateTime = c.CreateTime,
                    Status = c.Status
                })
               .ToListAsync();

            return new PaginatedResult<ChapterReviewDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: chapters);
        }

        public async Task<PaginatedResult<ChapterReviewDto>> GetChapterNotReview(int authorId, int page, int pageSize)
        {
            var baseQuery = _dbSet
                .AsNoTracking()
                .Where(c => (c.Status == 0 || c.Status == null) && c.Story.AuthorId != authorId);

            int totalCount = await baseQuery.CountAsync();

            var chapters = await baseQuery
                .OrderBy(v => v.CreateTime)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(c => new ChapterReviewDto
                {
                    StoryId = c.StoryId,
                    ChapterId = c.ChapterId,
                    VolumeId = c.VolumeId,
                    StoryTitle = c.Story.StoryTitle,
                    VolumeTitle = c.Volume.VolumeTitle,
                    VolumeNumber = c.Volume.VolumeNumber,
                    ChapterTitle = c.ChapterTitle,
                    ChapterNumber = c.ChapterNumber,
                    CreateTime = c.CreateTime,
                    Status = c.Status
                })
               .ToListAsync();

            return new PaginatedResult<ChapterReviewDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: chapters);
        }

        public async Task<ChapterInformationReviewDto?> GetChapterInformationToReview(int chapterId)
        {
            var chapter = await _dbSet
                .AsNoTracking()
                .Where(c => c.ChapterId == chapterId)
                .Select(c => new ChapterInformationReviewDto
                {
                    ChapterId = c.ChapterId,
                    ChapterStatus = c.Status,
                    StoryId = c.Story.StoryId,
                    StoryTitle = c.Story.StoryTitle,
                    ChapterTitle = c.ChapterTitle,
                    ChapterContentHtml = c.ChapterContentHtml,
                    ChapterContentMarkdown = c.ChapterContentMarkdown,
                    ChapterNumber = c.ChapterNumber,
                    VolumeId = c.VolumeId,
                    ChapterPrice = c.ChapterPrice,
                }).FirstOrDefaultAsync();

            return chapter;
        }

        public async Task<GetInfoPurchaseChapterResponse?> GetInfoPurchaseChapter(int storyId)
        {
            var chapters = await _dbSet
                    .Where(ch => ch.StoryId == storyId)
                    .OrderBy(ch => ch.ChapterNumber)
                    .Select(ch => new
                    {
                        ChapterId = ch.ChapterId,
                        ChapterNumber = ch.ChapterNumber,
                        StoryId = ch.StoryId
                    })
                    .ToListAsync();

            return new GetInfoPurchaseChapterResponse
            {
                Chapter_story_max = chapters.Max(c => c.ChapterNumber),
                User_chapter = chapters.Count()
            };
        }
    }
}
