using app.DTOs.Chapter;
using app.DTOs.Review;
using app.Interface;
using app.Models;
using app.Service;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<List<ChapterReviewDto>> GetChapterNotReview(int authorId)
        {
            var chapters = await _context.Chapters.Where(c => c.Status == 0 && c.Story.AuthorId != authorId)
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
            var chapters = await _context.Chapters.Where(c => (c.Status == 0 || c.Status == null) && c.Story.AuthorId == authorId)
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
            return await _context.Reviews.Where(r => r.ChapterId == chapterId).FirstOrDefaultAsync();
        }

        public async Task<ReviewDto?> GetReviewDetail(int chapterId)
        {
            var review = await _context.Reviews.Where(r => r.ChapterId == chapterId)
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
