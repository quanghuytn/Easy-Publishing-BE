using EP.Application.Common.DTOs.Chapter;
using EP.Application.Common.DTOs.Volume;
using EP.Application.Common.Interfaces.Repositories;
using EP.Domain.Models;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Repositories
{
    public class VolumeRepository : Repository<Volume>, IVolumeRepository
    {
        public VolumeRepository(Context context) : base(context)
        {
        }

        public async Task<int> GetLatestVolumeNumber(int storyId)
        {
            return await _dbSet
                        .Where(v => v.StoryId == storyId)
                        .MaxAsync(v => (int?)v.VolumeNumber) ?? 0;
        }

        public async Task<IEnumerable<VolumeChapterDto>> GetVolumes(int storyId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(v => v.StoryId == storyId)
                .Include(v => v.Chapters)
                .Select(v => new VolumeChapterDto
                {
                    VolumeId = v.VolumeId,
                    VolumeNumber = v.VolumeNumber,
                    VolumeTitle = v.VolumeTitle,
                    StoryId = v.StoryId,
                    CreateTime = v.CreateTime,
                    Chapters = v.Chapters.Where(c => c.Status >= 0 || c.Status == null).Select(c => new MinimalChapterDto
                    {
                        ChapterId = c.ChapterId,
                        ChapterNumber = c.ChapterNumber,
                        ChapterTitle = c.ChapterTitle,
                        ChapterPrice = c.ChapterPrice,
                        CreateTime = c.CreateTime,
                        Status = c.Status
                    }).OrderBy(c => c.ChapterNumber).ToList()
                }).OrderBy(v => v.VolumeNumber)
                .ToListAsync();
        }

        public async Task<bool> HasValidPreviousVolumeAsync(int storyId, int previousVolumeNumber)
        {
            return await _dbSet
                        .Where(v => v.VolumeNumber == previousVolumeNumber &&
                                   v.StoryId == storyId)
                        .Select(v => v.Chapters.Count >= 2)
                        .FirstOrDefaultAsync();
        }
    }
}
