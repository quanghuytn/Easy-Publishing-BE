using app.DTOs.Report;
using app.Interface;
using app.Models;
using app.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace app.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly EasyPublishingContext _context;
        private MailService _mailService = new MailService();

        public ReportRepository(EasyPublishingContext context)
        {
            _context = context;
        }

        public async Task<List<ReportDetailDto>> GetAllReports()
        {
            var reports = await _context.ReportContents
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
            return reports;
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
                return "https://genesis-easy-publishing.vercel.app/story/read/" + storyId + "/" + cleanedName + ".chapter-" + chapterNumber;
            }
            else
            {
                return "https://genesis-easy-publishing.vercel.app/story/detail/" + storyId + "/" + cleanedName;
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

        public async Task<List<ReportType>> GetReportTypes()
        {
            var types = await _context.ReportTypes.Select(c => new ReportType{ ReportTypeId = c.ReportTypeId, ReportTypeContent = c.ReportTypeContent }).ToListAsync();
            return types;
        }

        public async Task<ReportDto?> GetReportById(int id)
        {
            var report = await _context.ReportContents.Where(r => r.ReportId == id)
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
            return report;
        }

        public async Task<bool> SendReport(int userId, SendReportDTO newReport)
        {
            var user_report = 0;
            var mail_content = "";
            var link = "";
            if (newReport.CommentId != null)
            {
                var comment = await _context.Comments.Where(c => c.CommentId == newReport.CommentId).FirstOrDefaultAsync();
                if (comment == null) return false;
                var storyId = comment.StoryId;
                
                link = $"https://genesis-easy-publishing.vercel.app/story/detail/{storyId}/di-the-ta-quan";
                mail_content = $"<p>Nội dung của bạn: <b>{comment.CommentContent}</b></p>" +
                               $"<p>Xin hãy chỉnh sửa sớm nhất thông qua đường link dưới</p>" +
                               $"<a href=\"{link}\">Link chỉnh sửa</a>";
                user_report = comment.UserId;
            }
            else if (newReport.StoryId != null)
            {

                link = $"https://genesis-easy-publishing.vercel.app/author/write-story?mode=edit&storyId={newReport.StoryId}";
                if (newReport.ChapterId != null) link =
                 $"https://genesis-easy-publishing.vercel.app/author/write-chapter?mode=edit&storyId={newReport.StoryId}&chapterId={newReport.ChapterId}";

                mail_content = $"<p>Nội dung bạn đăng tải đã vi phạm tiêu chí trên" +
                               $"<p>Xin hãy chỉnh sửa sớm nhất thông qua đường link dưới</p>" +
                               $"<a href=\"{link}\">Link chỉnh sửa</a>";
                var author = await _context.Stories.FirstOrDefaultAsync(c => c.StoryId == newReport.StoryId);
                user_report = author.AuthorId;

            }

            var report_type = await _context.ReportTypes.FirstOrDefaultAsync(c => c.ReportTypeId == newReport.ReportTypeId);
            var user = await _context.Users.FirstOrDefaultAsync(c => c.UserId == user_report);
            var name = user.UserFullname == null ? user.Email : user.UserFullname;
            try
            {
                _mailService.Send(user.Email,
                        "Bạn vi phạm nguyên tắc cộng đồng",
                        "<p>Easy Publishing Xin chào <b> " + name + "</b>,</p>" +
                        "<b>Thông tin vi phạm như sau:</b>" +
                        "<p>Nguyên nhân: <b>" + report_type.ReportTypeContent + "</b></p>" +
                        mail_content +
                        "<p>Cảm ơn bạn đã tin tưởng.</p>");
            }
            catch (Exception ex)
            {
                throw;
            }
            try
            {
                ReportContent report = new ReportContent()
                {
                    UserId = userId,
                    ReportTypeId = newReport.ReportTypeId,
                    StoryId = newReport.StoryId,
                    ChapterId = newReport.ChapterId,
                    CommentId = newReport.CommentId,
                    ReportContent1 = newReport.ReportContent,
                    ReportDate = DateTime.Now,
                    Status = false,
                };
                _context.ReportContents.Add(report);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

        public async Task<string> ChangeReportStatus(int id)
        {
            var report = await _context.ReportContents.FirstOrDefaultAsync(r => r.ReportId == id);
            string msg = "Resolved report successfully!";
            try
            {
                if (report.Status == null || report.Status == false)
                {
                    report.Status = true;
                }
                else
                {
                    msg = "Unsolved report successfully!";
                    report.Status = false;
                }
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
