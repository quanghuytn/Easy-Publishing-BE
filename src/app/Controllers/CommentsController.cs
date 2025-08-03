using EP.Application.Commands.Comments;
using EP.Application.Common.DTOs.Comment;
using EP.Application.Queries.Comments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace app.Controllers
{
    [Authorize]
    [Route("api/v1/comments")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CommentsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Authorize]
        [HttpPost("send")]
        public async Task<ActionResult> SendComment(SendCommentDto newComment)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var command = new AddCommentCommand
            {
                StoryId = newComment.StoryId,
                UserId = userId,
                ChapterId = newComment.ChapterId,
                CommentContent = newComment.CommentContent
            };
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("edit")]
        public async Task<ActionResult> EditComment(int commentId, [FromBody] CommentUpdateDto commentUpdate)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var command = new EditCommentCommand
            {
                CommentId = commentId,
                UserId = userId,
                CommentContent = commentUpdate.CommentContent
            };
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [Authorize]
        [HttpDelete("{commentId}")]
        public async Task<ActionResult> DeleteCommentByUser(int commentId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var command = new DeleteCommentByUserCommand
            {
                CommentId = commentId,
                UserId = userId
            };
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("delete_comment")]
        public async Task<ActionResult> DeleteComment(int commentId)
        {
            var command = new DeleteCommentByAdminCommand
            {
                CommentId = commentId
            };
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("story_detail")]
        public async Task<ActionResult> GetStoryComments(int storyId, int page, int pageSize)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var query = new GetStoryCommentsQuery
            {
                UserId = userId,
                StoryId = storyId,
                PageIndex = page,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        //[AllowAnonymous]
        //[HttpGet("chapter_content")]
        //public async Task<ActionResult> GetChapterComments(int chapterId, int page, int pageSize)
        //{
        //    int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        //    var comments = await _commentRepo.GetChapterComments(userId, chapterId);
        //    pageSize = pageSize == null ? 10 : pageSize;
        //    return _msgService.MsgPagingReturn("Bình luận của chương",
        //        comments.Skip(pageSize * (page - 1)).Take(pageSize), page, pageSize, comments.Count);
        //}

    }
}
