using EP.Application.Commands.Interactions;
using MediatR;
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
        private readonly IMediator _mediator;
        public InteractionsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Authorize]
        [HttpPut("story_like")]
        public async Task<ActionResult> LikeStory(int storyId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var command = new LikeStoryCommand
            {
                StoryId = storyId,
                UserId = userId
            };
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [Authorize]
        [HttpPut("story_follow")]
        public async Task<ActionResult> FollowStory(int storyId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var command = new FollowStoryCommand
            {
                StoryId = storyId,
                UserId = userId
            };
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPut("chapter_like")]
        public async Task<ActionResult> LikeChapter(int storyId, int chapterNumber)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var command = new LikeChapterCommand
            {
                StoryId = storyId,
                ChapterNumber = chapterNumber,
                UserId = userId
            };
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        //[AllowAnonymous]
        //[HttpGet("author_manage/story")]
        //public async Task<ActionResult> GetStoryData(int storyId)
        //{

        //    var storyInteraction = await _interactionRepo.GetStoryInteraction(storyId);

        //    return _msgService.MsgReturn(0, "Truyện của tác giả", storyInteraction);
        //}

        //[AllowAnonymous]
        //[HttpGet("author_manage/chapter")]
        //public async Task<ActionResult> GetStoryChaptersData(int storyId, int from, int to)
        //{

        //    var interaction = await _interactionRepo.GetStoryChaptersInteraction(storyId);
        //    if (interaction.Count < 1) return _msgService.MsgReturn(-1, "Truyện chưa có chương", new { interaction });
        //    if (interaction.Count == 1) return _msgService.MsgReturn(0, "Truyện có 1 chương", new { interaction });
        //    var min = interaction.First().ChapterNumber;
        //    var max = interaction.Last().ChapterNumber;
        //    if (from != null && to != null && from < to) interaction = interaction.Where(c => c.ChapterNumber >= from && c.ChapterNumber <= to).ToList();

        //    return _msgService.MsgReturn(0, "Truyện của tác giả", new { interaction, min, max });
        //}

    }
}
