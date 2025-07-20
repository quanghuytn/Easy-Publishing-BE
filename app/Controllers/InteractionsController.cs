using app.Interface;
using app.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace app.Controllers
{
    [Authorize]
    [Route("api/v1/interaction")]
    [ApiController]
    public class InteractionsController : ControllerBase
    {
        private readonly IInteractionRepository _interactionRepo;
        private MsgService _msgService = new MsgService();

        public InteractionsController(IInteractionRepository interactionRepository)
        {
            _interactionRepo = interactionRepository;
        }

        [HttpPut("story_like")]
        public async Task<ActionResult> LikeStory(int storyId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                var msg = await _interactionRepo.LikeStory(userId, storyId);
                return _msgService.MsgActionReturn(0, msg);

            }
            catch (Exception)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Hệ thống xảy ra lỗi!"
                });
            }

        }

        [HttpPut("story_follow")]
        public async Task<ActionResult> FollowStory(int storyId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            try
            {
                var msg = await _interactionRepo.FollowStory(userId, storyId);
                return _msgService.MsgActionReturn(0, msg);
            }
            catch (Exception)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Hệ thống xảy ra lỗi!"
                });
            }
        }

        [HttpPut("chapter_like")]
        public async Task<ActionResult> LikeChapter(int storyId, int chapterNumber)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                var msg = await _interactionRepo.LikeChapter(userId, storyId, chapterNumber);
                return _msgService.MsgActionReturn(0, msg);
            }
            catch (Exception)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Hệ thống xảy ra lỗi!"
                });
            }
        }

        [AllowAnonymous]
        [HttpGet("author_manage/story")]
        public async Task<ActionResult> GetStoryData(int storyId)
        {

            var storyInteraction = await _interactionRepo.GetStoryInteraction(storyId);

            return _msgService.MsgReturn(0, "Truyện của tác giả", storyInteraction);
        }

        [AllowAnonymous]
        [HttpGet("author_manage/chapter")]
        public async Task<ActionResult> GetStoryChaptersData(int storyId, int from, int to)
        {

            var interaction = await _interactionRepo.GetStoryChaptersInteraction(storyId);
            if (interaction.Count < 1) return _msgService.MsgReturn(-1, "Truyện chưa có chương", new { interaction });
            if (interaction.Count == 1) return _msgService.MsgReturn(0, "Truyện có 1 chương", new { interaction });
            var min = interaction.First().ChapterNumber;
            var max = interaction.Last().ChapterNumber;
            if (from != null && to != null && from < to) interaction = interaction.Where(c => c.ChapterNumber >= from && c.ChapterNumber <= to).ToList();

            return _msgService.MsgReturn(0, "Truyện của tác giả", new { interaction, min, max });
        }

    }
}
