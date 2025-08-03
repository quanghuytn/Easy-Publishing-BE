using EP.Application.Commands.Reviews;
using EP.Application.Common.DTOs.Review;
using EP.Application.Queries.Reviews;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace app.Controllers
{
    [Authorize]
    [Route("api/v1/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReviewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Reviewer")]
        [HttpPost("send")]
        public async Task<ActionResult> SendReview([FromBody] SendReviewDto data)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var command = new SendReviewCommand
            {
                UserId = userId,
                ChapterId = data.ChapterId,
                SpellingError = data.SpellingError,
                LengthError = data.LengthError,
                PoliticalContentError = data.PoliticalContentError,
                DistortHistoryError = data.DistortHistoryError,
                SecretContentError = data.SecretContentError,
                OffensiveContentError = data.OffensiveContentError,
                UnhealthyContentError = data.UnhealthyContentError,
                ReviewContent = data.ReviewContent
            };
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [HttpGet("review_detail")]
        public async Task<ActionResult> getReviewDetail(int chapterId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var query = new GetReviewDetailQuery(userId, chapterId);
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [HttpGet("chapter_review_author")]
        public async Task<ActionResult> GetChapterNotReviewOfAuthor(int page, int pageSize)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var query = new GetChapterNotReviewOfAuthorQuery
            {
                AuthorId = userId,
                PageIndex = page,
                PageSize = pageSize
            };
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [Authorize(Roles ="Reviewer")]
        [HttpGet("chapter_review")]
        public async Task<ActionResult> GetChapterNotReview(int page, int pageSize)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var qry = new GetChapterNotReviewQuery
            {
                AuthorId = userId,
                PageIndex = page,
                PageSize = pageSize
            };
            var response = await _mediator.Send(qry);

            return Ok(response);
        }

        [Authorize(Roles= "Reviewer")]
        [HttpGet("story_list")]
        public async Task<ActionResult> GetStoriesReview(int page, int pageSize)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var query = new GetStoriesReviewQuery
            {
                UserId = userId,
                PageIndex = page,
                PageSize = pageSize
            };
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [Authorize(Roles ="Reviewer")]
        [HttpGet("volume_list")]
        public async Task<ActionResult> GetVolume(int storyId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var query = new GetVolumeReviewQuery
            {
                StoryId = storyId,
                UserId = userId
            };
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [Authorize(Roles = "Reviewer")]
        [HttpGet("chapter_information")]
        public async Task<ActionResult> GetChapterInfor(int chapterId)
        {
            var query = new GetChapterInformationToReviewQuery(chapterId);
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [Authorize(Roles = "Reviewer")]
        [HttpGet("story_admin")]
        public async Task<ActionResult> GetStoriesAdmin()
        {
            var query = new GetStoryReviewAdminQuery();
            var response = await _mediator.Send(query);

            return Ok(response);
            //return msgService.MsgReturn(0, "Danh sách truyện có chương cần review",
            //    new
            //    {
            //        stories = stories,
            //    });
        }
    }
}