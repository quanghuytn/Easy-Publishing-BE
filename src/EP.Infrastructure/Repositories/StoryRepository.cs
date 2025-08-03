using EP.Application.Common.DTOs.Chapter;
using EP.Application.Common.DTOs.Interaction;
using EP.Application.Common.DTOs.Story;
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
    }
}
