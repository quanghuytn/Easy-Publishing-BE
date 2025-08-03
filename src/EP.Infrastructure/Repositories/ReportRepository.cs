using EP.Application.Common.DTOs.Report;
using EP.Application.Common.Interfaces.Repositories;
using EP.Domain.Models;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace EP.Infrastructure.Repositories
{
    public class ReportRepository : Repository<ReportContent>, IReportRepository
    {
        public ReportRepository(Context context) : base(context)
        {
        }

        public async Task<IEnumerable<ReportDetailDto>> GetAllReports()
        {
            return await _dbSet
                .AsNoTracking()
                .Include(r => r.ReportType)
                .Include(r => r.Chapter)
                .Include(r => r.Story)
                .Include(r => r.Comment)
                .Include(r => r.User)
                .Select(r => new ReportDetailDto
                {
                    ReportId = r.ReportId,
                    UserName = r.User.Username,
                    StoryId = r.StoryId,
                    ChapterId = r.ChapterId,
                    ReportTypeContent = r.ReportType.ReportTypeContent,
                    ChapterTitle = r.Chapter.ChapterTitle,
                    Link = FormatLink(r.Story.StoryId, r.Story.StoryTitle != null ? r.Story.StoryTitle : null, r.Chapter != null ? r.Chapter.ChapterNumber : 0),
                    StoryTitle = r.Story.StoryTitle,
                    CommentContent = r.Comment.CommentContent,
                    CommentId = r.CommentId,
                    ReportContent1 = r.ReportContent1,
                    ReportDate = r.ReportDate.ToString("dd/MM/yyyy HH:mm:ss"),
                    Status = (r.Status == null || r.Status == false) ? "Unsolved" : "Resolved"
                })
                .ToListAsync();
        }

        private static string FormatLink(int? storyId, string storyTitle, long chapterNumber)
        {
            if (storyTitle == null)
            {
                return null;
            }
            storyTitle = Regex.Replace(storyTitle, @"\s+", " ").Trim();

            string cleanedName = string.Concat(storyTitle.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)));

            cleanedName = cleanedName.ToLower();

            cleanedName = cleanedName.Replace(" ", "-");
            cleanedName = RemoveDiacritics(cleanedName);
            if (chapterNumber != 0)
            {
                return "https://easy-publishing.vercel.app/story/read/" + storyId + "/" + cleanedName + ".chapter-" + chapterNumber;
            }
            else
            {
                return "https://easy-publishing.vercel.app/story/detail/" + storyId + "/" + cleanedName;
            }
        }

        private static string RemoveDiacritics(string input)
        {
            string normalizedString = input.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public async Task<ReportDto?> GetReportById(int id)
        {
            return await _dbSet.Where(r => r.ReportId == id)
                .Include(r => r.ReportType)
                .Include(r => r.Chapter)
                .Include(r => r.Story)
                .Include(r => r.Comment)
                .Include(r => r.User)
                .Select(r => new ReportDto
                {
                    ReportId = r.ReportId,
                    UserName = r.User.Username,
                    ReportTypeContent = r.ReportType.ReportTypeContent,
                    ChapterTitle = r.Chapter.ChapterTitle,
                    StoryTitle = r.Story.StoryTitle,
                    CommentContent = r.Comment.CommentContent,
                    ReportContent1 = r.ReportContent1,
                    ReportDate = r.ReportDate,
                    Status = r.Status
                })
                .FirstOrDefaultAsync();
        }
    }
}
