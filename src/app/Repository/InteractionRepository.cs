using app.DTOs.Interaction;
using app.Interface;
using app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace app.Repository
{
    public class InteractionRepository : IInteractionRepository
    {
        private readonly EasyPublishingContext _context;

        public InteractionRepository(EasyPublishingContext context)
        {
            _context = context;
        }

        public async Task<string> FollowStory(int userId, int storyId)
        {
            var interaction = await _context.StoryFollowLikes.FirstOrDefaultAsync(c => c.StoryId == storyId && c.UserId == userId);
            var story_interaction = await _context.StoryInteractions.FirstOrDefaultAsync(c => c.StoryId == storyId);
            var msg = interaction == null || interaction.Follow == false ? "Bạn đã theo dõi truyện" : "Bạn đã bỏ theo dõi truyện";
            try
            {
                if (interaction != null)
                {
                    story_interaction.Follow = interaction.Follow == true ? story_interaction.Follow - 1 : story_interaction.Follow + 1;
                    interaction.Follow = !interaction.Follow;
                    _context.Entry(interaction).State = EntityState.Modified;
                }
                else
                {
                    story_interaction.Follow += 1;
                    StoryFollowLike storyFollowLike = new StoryFollowLike { UserId = userId, StoryId = storyId, Follow = true, Like = false };
                    _context.StoryFollowLikes.Add(storyFollowLike);
                }
                _context.Entry(story_interaction).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return msg;
        }

        public async Task<List<ChapterInteractionDto>> GetStoryChaptersInteraction(int storyId)
        {
            var storyChapters = await _context.Chapters
                .AsNoTracking()
                .Where(c => c.StoryId == storyId)
               .OrderBy(c => c.ChapterId)
               .Include(c => c.Users)
               .Include(c => c.Comments)
               .Include(c => c.ReportContents)
               .Select(s => new ChapterInteractionDto
               {
                   ChapterId = s.ChapterId,
                   ChapterNumber = s.ChapterNumber,
                   ChapterTitle = s.ChapterTitle,
                   PurchaseChapter = s.Users.Count,
                   CommentChapter = s.Comments.Count,
                   ReportChapter = s.ReportContents.Count,
               }).ToListAsync();
            return storyChapters;
        }

        public async Task<StoryInteractionDto?> GetStoryInteraction(int storyId)
        {
            var storyInteraction = await _context.Stories.Where(c => c.StoryId == storyId)
               .Include(c => c.Users).Include(c => c.StoryInteraction)
               .Include(c => c.Chapters).ThenInclude(c => c.Users).ThenInclude(c => c.Comments).ThenInclude(c => c.ReportContents)
               .Include(c => c.Comments)
               .Include(c => c.ReportContents)
               .Select(s => new StoryInteractionDto
               {
                   StoryId = s.StoryId,
                   StoryTitle = s.StoryTitle,
                   Like = s.StoryInteraction.Like,
                   Follow = s.StoryInteraction.Follow,
                   View = s.StoryInteraction.View,
                   Read = s.StoryInteraction.Read,
                   PurchaseStory = s.Users.Count,
                   PurchaseChapter = s.Chapters.SelectMany(c => c.Users).Count(),
                   CommentStory = s.Comments.Count,
                   CommentChapter = s.Chapters.SelectMany(c => c.Comments).Count(),
                   ReportStory = s.ReportContents.Count,
                   ReportChapter = s.Chapters.SelectMany(c => c.ReportContents).Count(),
               }).SingleOrDefaultAsync();
            return storyInteraction;
        }

        public async Task<string> LikeChapter(int userId, int storyId, int chapterNumber)
        {
            var chapter = await _context.Chapters.FirstOrDefaultAsync(c => c.StoryId == storyId && c.ChapterNumber == chapterNumber);
            var interaction = await _context.ChapterLikeds.FirstOrDefaultAsync(c => c.ChapterId == chapter.ChapterId && c.UserId == userId);
            var story_interaction = await _context.StoryInteractions.FirstOrDefaultAsync(c => c.StoryId == storyId);

            var msg = interaction == null ? "Bạn đã thích chương" : "Bạn đã bỏ thích chương";
            try
            {
                if (interaction != null)
                {
                    story_interaction.Like -= 1;
                    _context.ChapterLikeds.Remove(interaction);
                }
                else
                {
                    story_interaction.Like += 1;
                    ChapterLiked chapterLiked = new ChapterLiked { UserId = userId, ChapterId = chapter.ChapterId, Status = null };
                    _context.ChapterLikeds.Add(chapterLiked);
                }

                _context.Entry(story_interaction).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return msg;
        }

        public async Task<string> LikeStory(int userId, int storyId)
        {
            var interaction = await _context.StoryFollowLikes.FirstOrDefaultAsync(c => c.StoryId == storyId && c.UserId == userId);
            var story_interaction = await _context.StoryInteractions.FirstOrDefaultAsync(c => c.StoryId == storyId);
            var msg = interaction == null || interaction.Follow == false ? "Bạn đã thích truyện" : "Bạn đã bỏ thích truyện";
            try
            {
                if (interaction != null)
                {
                    story_interaction.Like = interaction.Like == true ? story_interaction.Like - 1 : story_interaction.Like + 1;
                    interaction.Like = !interaction.Like;
                    _context.Entry(interaction).State = EntityState.Modified;

                }
                else
                {
                    story_interaction.Like += 1;
                    StoryFollowLike storyFollowLike = new StoryFollowLike { UserId = userId, StoryId = storyId, Follow = false, Like = true };
                    _context.StoryFollowLikes.Add(storyFollowLike);

                }

                _context.Entry(story_interaction).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return msg;
        }
    }
}
