using app.DTOs.Review;
using app.Interface;
using app.Models;
using app.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace app.Controllers
{
    [Authorize]
    [Route("api/v1/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IStoryRepository _storyRepository;
        private MailService mailService = new MailService();
        private MsgService msgService = new MsgService();
        private int pagesize = 10;

        public ReviewsController(IReviewRepository reviewRepository, IChapterRepository chapterRepository, IStoryRepository storyRepository)
        {
            _storyRepository = storyRepository;
            _reviewRepository = reviewRepository;
            _chapterRepository = chapterRepository;
        }

        [Authorize(Roles = "Reviewer")]
        [HttpPost("send")]
        public async Task<ActionResult> SendReview([FromBody] SendReviewDto data)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var chapter = await _chapterRepository.GetChapter(data.ChapterId);
            if (chapter == null)
            {
                return new JsonResult(new
                {
                    EC = 2,
                    EM = "Chương không tồn tại"
                });
            }
            var story = await _storyRepository.GetStory(chapter.StoryId);
            if (story == null)
            {
                return new JsonResult(new
                {
                    EC = 3,
                    EM = "Truyện của chương không tồn tại"
                });
            }
            var review = await _reviewRepository.GetReviewByChapter(data.ChapterId);
            if (review != null)
            {
                return new JsonResult(new
                {
                    EC = 4,
                    EM = "Chương này đã được review"
                });
            }
            // chapter status
            bool[] errorList = {
                    data.SpellingError,
                    data.LengthError,
                    data.PoliticalContentError,
                    data.DistortHistoryError,
                    data.SecretContentError,
                    data.OffensiveContentError,
                    data.UnhealthyContentError,
             };
            bool hasError = false;
            foreach (var item in errorList)
            {
                if (!item)
                {
                    hasError = true;
                    break;
                }
            }
            if (hasError && string.IsNullOrEmpty(data.ReviewContent))
            {
                return new JsonResult(new
                {
                    EC = 5,
                    EM = "Yêu cầu nhập nội dung review"
                });
            }
            if (hasError)
            {
                chapter.Status = null;
            }
            else
            {
                chapter.Status = 1;
                if (story.Status == 0)
                {
                    story.Status = 1;
                }
            }

            try
            {
                await _reviewRepository.SendReview(userId, data);
            }
            catch (Exception)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Hệ thống xảy ra lỗi!"
                });
            }

            // send mail
            try
            {
                var link = "https://genesis-easy-publishing.vercel.app/author/review-a-chapter?mode=readOnly&storyId=" + story.StoryId + "&chapterId=" + chapter.ChapterId;
                mailService.Send(story.Author.Email,
                        "Easy Publishing: Truyện của bạn đã được review",
                        "<p>Xin chào <b>" + story.Author.Username + "</b>,</p>" +
                        "<p>Chương <b>" + chapter.ChapterTitle + "</b> của Truyện <b>" + story.StoryTitle + "</b> của bạn đã được review.</p> " +
                        "<p>Chi tiết vui lòng truy cập:</p> " +
                        "<a href = " + link + ">Xem kết quả review</a>");
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    EC = 6,
                    EM = "Error: " + ex.Message
                });
            }
            return new JsonResult(new
            {
                EC = 0,
                EM = "Gửi review thành công"
            });
        }

        [HttpGet("review_detail")]
        public async Task<ActionResult> getReviewDetail(int chapterId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var chapter = await _chapterRepository.GetChapter(chapterId);
            if (chapter == null)
            {
                return new JsonResult(new
                {
                    EC = 1,
                    EM = "Chương không tồn tại"
                });
            }
            var story = await _storyRepository.GetStory(chapter.StoryId);
            if (story == null)
            {
                return new JsonResult(new
                {
                    EC = 2,
                    EM = "Truyện của chương không tồn tại"
                });
            }
            var review = await _reviewRepository.GetReviewDetail(chapterId);
            if (review == null)
            {
                return new JsonResult(new
                {
                    EC = 3,
                    EM = "Chương chưa được review"
                });
            }
            if (story.AuthorId != userId && review.Reviewer.UserId != userId)
            {
                return new JsonResult(new
                {
                    EC = 4,
                    EM = "Bạn không có quyền truy cập"
                });
            }
            return new JsonResult(new
            {
                EC = 0,
                EM = "Thông tin review của chương",
                DT = new
                {
                    review = review
                }
            });
        }

        [HttpGet("chapter_review_author")]
        public async Task<ActionResult> GetChapterNotReviewOfAuthor(int page, int pageSize)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var chapters = await _reviewRepository.GetChapterNotReviewOfAuthor(userId);

            page = page == null || page == 0 ? 1 : page;
            pageSize = pageSize == null || pageSize == 0 ? pagesize : pageSize;
            return msgService.MsgPagingReturn("Danh sách chương chưa review",
            chapters.Skip(pageSize * (page - 1)).Take(pageSize), page, pageSize, chapters.Count);
        }

        [Authorize(Roles ="Reviewer")]
        [HttpGet("chapter_review")]
        public async Task<ActionResult> GetChapterNotReview(int page, int pageSize)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var chapters = await _reviewRepository.GetChapterNotReview(userId);

            page = page == null || page == 0 ? 1 : page;
            pageSize = pageSize == null || pageSize == 0 ? pagesize : pageSize;
            return msgService.MsgPagingReturn("Danh sách chương chưa review",
            chapters.Skip(pageSize * (page - 1)).Take(pageSize), page, pageSize, chapters.Count);
        }

        [Authorize(Roles= "Reviewer")]
        [HttpGet("story_list")]
        public async Task<ActionResult> GetStoriesReview(int page, int pageSize)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var stories = await _reviewRepository.GetStoryReview(userId);
            page = page == null || page == 0 ? 1 : page;
            pageSize = pageSize == null || pageSize == 0 ? pagesize : pageSize;
            return msgService.MsgPagingReturn("Danh sách truyện có chương cần review",
                stories.Skip(pageSize * (page - 1)).Take(pageSize), page, pageSize, stories.Count);
        }

        [Authorize(Roles ="Reviewer")]
        [HttpGet("volume_list")]
        public async Task<ActionResult> GetVolume(int storyId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var volumes = await _reviewRepository.GetVolumeReview(storyId, userId);
            if (volumes.Count() == 0)
            {
                return msgService.MsgActionReturn(2, "Không có tập chứa chương cần review");
            }
            return msgService.MsgReturn(0, "Danh sách các tập của truyện", volumes);
        }

        [Authorize(Roles = "Reviewer")]
        [HttpGet("chapter_information")]
        public async Task<ActionResult> GetChapterInfor(int chapterId)
        {
            var chapter = await _reviewRepository.GetChapterInformationReview(chapterId);
            if (chapter == null)
            {
                return msgService.MsgActionReturn(3, "Chương không tồn tại");
            }
            if (chapter.ChapterStatus != 0)
            {
                return msgService.MsgActionReturn(4, "Chương đã được review");
            }
            return msgService.MsgReturn(0, "Thông tin chương", chapter);
        }

        [Authorize(Roles = "Reviewer")]
        [HttpGet("story_admin")]
        public async Task<ActionResult> GetStoriesAdmin()
        {

            var stories = await _reviewRepository.GetStoryReviewAdmin();
            return msgService.MsgReturn(0, "Danh sách truyện có chương cần review",
                new
                {
                    stories = stories,
                });
        }
    }
}