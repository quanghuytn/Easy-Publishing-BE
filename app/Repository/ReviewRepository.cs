using app.DTOs.Chapter;
using app.DTOs.Interaction;
using app.DTOs.Review;
using app.Interface;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly EasyPublishingContext _context;

        public ReviewRepository(EasyPublishingContext context)
        {
            _context = context;
        }

        public async Task<ChapterInformationReviewDto?> GetChapterInformationReview(int chapterId)
        {
            var chapter = await _context.Chapters.AsNoTracking().Where(c => c.ChapterId == chapterId).Select(c => new ChapterInformationReviewDto
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

        public async Task<List<ChapterReviewDto>> GetChapterNotReview(int authorId)
        {
            var chapters = await _context.Chapters.AsNoTracking().Where(c => c.Status == 0 && c.Story.AuthorId != authorId)
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
                }).OrderBy(v => v.CreateTime)
                .ToListAsync();
            return chapters;
        }

        public async Task<List<ChapterReviewDto>> GetChapterNotReviewOfAuthor(int authorId)
        {
            var chapters = await _context.Chapters.AsNoTracking().Where(c => (c.Status == 0 || c.Status == null) && c.Story.AuthorId == authorId)
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
               }).OrderBy(v => v.CreateTime)
               .ToListAsync();
            return chapters;
        }

        public async Task<Review?> GetReviewByChapter(int chapterId)
        {
            return await _context.Reviews.AsNoTracking().Where(r => r.ChapterId == chapterId).FirstOrDefaultAsync();
        }

        public async Task<ReviewDto?> GetReviewDetail(int chapterId)
        {
            var review = await _context.Reviews.Where(r => r.ChapterId == chapterId)
                .AsNoTracking()
                .Include(r => r.User)
                .Include(r => r.Chapter)
                .Select(r => new ReviewDto
                {
                    ReviewDate = r.ReviewDate,
                    SpellingError = r.SpellingError,
                    LengthError = r.LengthError,
                    PoliticalContentError = r.PoliticalContentError,
                    DistortHistoryError = r.DistortHistoryError,
                    SecretContentError = r.SecretContentError,
                    OffensiveContentError = r.OffensiveContentError,
                    UnhealthyContentError = r.UnhealthyContentError,
                    ReviewContent = r.ReviewContent,
                    Chapters = new ChapterDto
                    {
                        ChapterId = r.Chapter.ChapterId,
                        ChapterNumber = r.Chapter.ChapterNumber,
                        ChapterTitle = r.Chapter.ChapterTitle,
                        ChapterPrice = r.Chapter.ChapterPrice,
                        CreateTime = r.Chapter.CreateTime,
                        ChapterContentMarkdown = r.Chapter.ChapterContentMarkdown,
                        ChapterContentHtml = r.Chapter.ChapterContentHtml
                    },
                    Reviewer = new ReviewerDto
                    {
                        UserId = r.UserId,
                        Email = r.User.Email,
                        Username = r.User.Username,
                        UserFullname = r.User.UserFullname,
                        Gender = r.User.Gender == true ? "Male" : "Female",
                        Dob = r.User.Dob,
                        Address = r.User.Address,
                        Phone = r.User.Phone,
                        Status = r.User.Status == true ? "Active" : "Inactive",
                        UserImage = r.User.UserImage,
                        DescriptionMarkdown = r.User.DescriptionMarkdown,
                        DescriptionHTML = r.User.DescriptionHtml,
                    }
                }).FirstOrDefaultAsync();
            return review;
        }

        public async Task<List<StoryReviewDto>> GetStoryReview(int userId)
        {
            var stories = await _context.Stories.AsNoTracking()
                .Include(s => s.Categories)
                .Include(s => s.Users)
                .Include(s => s.Chapters).ThenInclude(c => c.Users)
                .Include(s => s.StoryInteraction)
                .Where(s => s.Chapters.Any(c => c.Status == 0) && s.AuthorId != userId)
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
            return stories;
        }

        public async Task<List<StoryReviewAdminDto>> GetStoryReviewAdmin()
        {
            var stories = await _context.Stories
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
            return stories;
        }

        public async Task<List<VolumeReviewDto>> GetVolumeReview(int storyId, int userId)
        {
            var volumes = await _context.Volumes
                .AsNoTracking()
                .Include(v => v.Chapters)
                .Include(v => v.Story)
                .Where(v => v.StoryId == storyId && v.Story.AuthorId != userId && v.Chapters.Any(c => c.Status == 0))
                .Select(v => new VolumeReviewDto
                {
                    VolumeId = v.VolumeId,
                    VolumeNumber = v.VolumeNumber,
                    VolumeTitle = v.VolumeTitle,
                    StoryId = v.StoryId,
                    CreateTime = v.CreateTime,
                    Chapters = v.Chapters.Where(c => c.Status == 0).Select(c => new ChapterVolumeReviewDto
                    {
                        ChapterId = c.ChapterId,
                        Status = c.Status,
                        ChapterNumber = c.ChapterNumber,
                        ChapterTitle = c.ChapterTitle,
                        ChapterPrice = c.ChapterPrice,
                        CreateTime = c.CreateTime,
                    }).OrderBy(c => c.ChapterNumber).ToList()
                }).OrderBy(v => v.VolumeNumber)
                .ToListAsync();
            return volumes;
        }

        public async Task<bool> SendReview(int userId, SendReviewDto data)
        {
            try
            {
                Review newReview = new Review()
                {
                    UserId = userId,
                    ChapterId = data.ChapterId,
                    ReviewDate = DateTime.Now,
                    SpellingError = data.SpellingError,
                    LengthError = data.LengthError,
                    PoliticalContentError = data.PoliticalContentError,
                    DistortHistoryError = data.DistortHistoryError,
                    SecretContentError = data.SecretContentError,
                    OffensiveContentError = data.OffensiveContentError,
                    UnhealthyContentError = data.UnhealthyContentError,
                    ReviewContent = data.ReviewContent
                };
                await _context.Reviews.AddAsync(newReview);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }
    }
}
