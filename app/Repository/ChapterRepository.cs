using app.DTOs.Author;
using app.DTOs.Chapter;
using app.DTOs.Story;
using app.Interface;
using app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Drawing.Printing;

namespace app.Repository
{
    public class ChapterRepository : IChapterRepository
    {
        private readonly EasyPublishingContext _context;

        public ChapterRepository(EasyPublishingContext context)
        {
            _context = context;
        }

        public async Task AddChapter(AddChapterDto newChapter)
        {
            Chapter chapter = new Chapter()
            {
                ChapterContentHtml = newChapter.ChapterContentHtml,
                ChapterContentMarkdown = newChapter.ChapterContentMarkdown,
                StoryId = newChapter.StoryId,
                VolumeId = newChapter.VolumeId,
                ChapterTitle = newChapter.ChapterTitle,
                ChapterPrice = newChapter.ChapterPrice
            };
            chapter.CreateTime = DateTime.Now;
            chapter.Status = 0;

            try
            {
                long nextChapterNum = _context.Chapters.Where(c => c.StoryId == chapter.StoryId && c.VolumeId == chapter.VolumeId && c.Status >= 0).Select(c => c.ChapterNumber).ToList().DefaultIfEmpty(0).Max() + 1;
                chapter.ChapterNumber = nextChapterNum;
                await _context.Chapters.AddAsync(chapter);
                _context.SaveChanges();
                // renumber chapter number
                var chapters = _context.Chapters.Where(c => c.StoryId == chapter.StoryId && (c.Status >= 0 || c.Status == null)).OrderBy(c => c.Volume.VolumeNumber).ThenBy(c => c.ChapterNumber).ToList();
                for (int i = 0; i < chapters.Count; i++)
                {
                    chapters[i].ChapterNumber = i + 1;
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> AddVolume(AddVolumeDto newVolume)
        {
            int volumeNumber =  _context.Volumes.Where(v => v.StoryId == newVolume.StoryId).Select(v => v.VolumeNumber).ToList().DefaultIfEmpty(0).Max() + 1;
            if (volumeNumber >= 2)
            {
                var h = await _context.Volumes.Where(v => v.VolumeNumber == (volumeNumber - 1) && v.StoryId == newVolume.StoryId).Include(v => v.Chapters).Select(v => new
                {
                    numberChapter = v.Chapters.Count()
                }).FirstOrDefaultAsync();
                if (h == null || h.numberChapter < 2)
                {
                    return false;
                }
            }
            try
            {
                Volume volume = new Volume()
                {
                    StoryId = newVolume.StoryId,
                    VolumeTitle = newVolume.VolumeTitle,
                    VolumeNumber = volumeNumber,
                    CreateTime = DateTime.Now
                };
                await _context.Volumes.AddAsync(volume);
                await _context.SaveChangesAsync();
            }
            catch(Exception)
            {
                throw;
            }
            return true;
        }

        public async Task<ChapterDto?> GetChapterInfor(int chapterId)
        {
            var chapter = await _context.Chapters
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

            return chapter;
        }

        public async Task<List<MinimalChapterDto>> GetStoryChapters(int storyId)
        {
            var chapters = await _context.Chapters
                .AsNoTracking()
                .Where(c => c.StoryId == storyId && c.Status > 0)
                .Include(c => c.Comments)
                .Include(c => c.Users)
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
                .OrderBy(c => c.ChapterNumber)
                .ToListAsync();
            return chapters;
        }

        public async Task<List<VolumeChapterDto>> GetVolumes(int storyId)
        {
            var volumes = await _context.Volumes
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
            return volumes;
        }

        public async Task<List<VolumeDto>> GetVolumesByStory(int storyId)
        {
            var volumes = await _context.Volumes
                .AsNoTracking()
                .Where(v => v.StoryId == storyId)
                .Select(v => new VolumeDto
                {
                    VolumeId = v.VolumeId,
                    VolumeNumber = v.VolumeNumber,
                    VolumeTitle = v.VolumeTitle
                })
                .ToListAsync();
            return volumes;
        }

        public async Task<bool> UpdateVolume(VolumeDto volume)
        {
            var currentVolume = await _context.Volumes.FirstOrDefaultAsync(v => v.VolumeId == volume.VolumeId);
            try
            {
                if (currentVolume != null)
                {
                    currentVolume.VolumeTitle = volume.VolumeTitle;
                    currentVolume.UpdateTime = DateTime.Now;
                }
                else
                {
                    return false;
                }
                _context.Entry<Volume>(currentVolume).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

        public async Task<bool> CheckReadPermission(int userId, int storyId, int chapterId)
        {
            var user = await _context.Users.Include(u => u.Chapters).Include(u => u.Stories).FirstOrDefaultAsync(u => u.UserId == userId);
            if (!user.Stories.Any(s => s.StoryId == storyId))
            {
                if (!user.Chapters.Any(c => c.ChapterId == chapterId))
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> UpdateChapter(UpdateChapterDto chapter)
        {
            var currentChapter = await _context.Chapters.FindAsync(chapter.ChapterId);
            try
            {
                if (currentChapter != null)
                {
                    currentChapter.ChapterTitle = chapter.ChapterTitle;
                    currentChapter.ChapterContentHtml = chapter.ChapterContentHtml;
                    currentChapter.ChapterContentMarkdown = chapter.ChapterContentMarkdown;
                    currentChapter.ChapterPrice = chapter.ChapterPrice;
                    currentChapter.UpdateTime = DateTime.Now;
                }
                else
                {
                    return false;
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

        public async Task<bool> DeleteChapter(int chapterId)
        {
            var currentChapter = await _context.Chapters.FindAsync(chapterId);
            if (currentChapter == null || currentChapter.Status == -1)
            {
                return false;
            }
            int storyId = currentChapter.StoryId;
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                currentChapter.Status = -1;
                await _context.SaveChangesAsync();

                var chapters = await _context.Chapters.Where(c => c.StoryId == storyId && (c.Status >= 0 || c.Status == null)).OrderBy(c => c.Volume.VolumeNumber).ThenBy(c => c.ChapterNumber).ToListAsync();
                for (int i = 0; i < chapters.Count; i++)
                {
                    chapters[i].ChapterNumber = i + 1;
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            return true;
        }

        public async Task<ChapterContentDto?> GetChapterContent(int userId, long chapterNumber, int storyId)
        {
            long nextChapterNum = NextChapter(chapterNumber, storyId);
            long prevChapterNum = PreviousChapter(chapterNumber, storyId);

            var chapter = await _context.Chapters
                .AsNoTracking()
                .Where(c => c.StoryId == storyId && c.ChapterNumber == chapterNumber && c.Status > 0)
                .Include(c => c.Story)
                .Include(c => c.Comments)
                .Include(c => c.ChapterLikeds)
                .Select(c => new ChapterContentDto
                {
                    Story = new StoryChapterDto { StoryId= c.StoryId, StoryTitle = c.Story.StoryTitle, StoryPrice = c.Story.StoryPrice },
                    Author = new MinimalAuthorDto { UserId = c.Story.Author.UserId, UserFullname = c.Story.Author.UserFullname },
                    Content = (c.ChapterPrice == 0 || c.ChapterPrice == null || userId == c.Story.Author.UserId || CheckPurchase(userId, chapterNumber, storyId)) ? c.ChapterContentHtml : null,
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
                    Owned = (c.ChapterPrice == 0 || c.ChapterPrice == null || userId == c.Story.Author.UserId || CheckPurchase(userId, chapterNumber, storyId)),
                    UserLike = c.ChapterLikeds.Any(c => c.UserId == userId && c.ChapterId == c.ChapterId),
                }).FirstOrDefaultAsync();

            if (chapter != null && chapter.Owned == true && userId != 0)
            {
                var story_interaction = await _context.StoryInteractions.FirstOrDefaultAsync(c => c.StoryId == storyId);
                story_interaction.Read += 1;
                _context.Entry(story_interaction).State = EntityState.Modified;

                var story_read = await _context.StoryReads.FirstOrDefaultAsync(c => c.UserId == userId && c.StoryId == storyId);
                if (story_read != null)
                {
                    story_read.ChapterId = chapter.ChapterId;
                    story_read.ReadTime = DateTime.Now;
                    _context.Entry(story_read).State = EntityState.Modified;
                }
                else _context.StoryReads.Add(new StoryRead
                {
                    StoryId = chapter.Story.StoryId,
                    UserId = userId,
                    ChapterId = chapter.ChapterId,
                    ReadTime = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }
            return chapter;
        }

        private bool CheckPurchase(int? userId, long chapterNum, int storyId)
        {
            if (userId == null)
            {
                return false;
            }
            var user = _context.Users.Where(u => u.UserId == userId).Select(u => new
            {
                UserId = u.UserId,
                RoleId = u.RoleId,
                Stories = u.StoriesNavigation.Select(sn => new { StoryId = sn.StoryId }).ToList(),
                Chapters = u.Chapters.Select(c => new { chapterId = c.ChapterId, ChapterNumber = c.ChapterNumber, StoryId = c.StoryId }).ToList()
            }).FirstOrDefault();
            if (user == null)
            {
                return false;
            }
            if (user.RoleId == 1)
            {
                return true;
            }
            if (user.Chapters.Any(c => c.ChapterNumber == chapterNum && c.StoryId == storyId) || user.Stories.Any(s => s.StoryId == storyId))
            {
                return true;
            }
            return false;
        }

        private long NextChapter(long currentChapterNumber, int storyId)
        {
            var nextChapter = _context.Chapters.Where(c => c.StoryId == storyId && c.ChapterNumber > currentChapterNumber && c.Status > 0)
                              .OrderBy(c => c.ChapterNumber)
                                .Select(c => new
                                {
                                    ChapterNumber = c.ChapterNumber
                                })
                              .FirstOrDefault();

            if (nextChapter == null)
            {
                return -1;
            }
            return nextChapter.ChapterNumber;
        }

        private long PreviousChapter(long currentChapterNumber, int storyId)
        {
            var nextChapter = _context.Chapters.Where(c => c.StoryId == storyId && c.ChapterNumber < currentChapterNumber && c.Status > 0)
                              .OrderByDescending(c => c.ChapterNumber)
                                .Select(c => new
                                {
                                    ChapterNumber = c.ChapterNumber
                                })
                              .FirstOrDefault();

            if (nextChapter == null)
            {
                return -1;
            }
            return nextChapter.ChapterNumber;
        }

        public async Task<Chapter?> GetChapter(int chapterId)
        {
            return await _context.Chapters.FindAsync(chapterId);
        }
    }
}
