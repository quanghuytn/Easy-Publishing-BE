using EP.Application.Common.DTOs.Chapter;
using EP.Application.Common.DTOs.Review;
using EP.Application.Common.Interfaces.Repositories;
using EP.Domain.Models;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EP.Infrastructure.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(Context context) : base(context)
        {
        }

        public async Task<ReviewDto?> GetReviewDetail(int chapterId)
        {
            return await _dbSet.Where(r => r.ChapterId == chapterId)
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
        }
    }
}
