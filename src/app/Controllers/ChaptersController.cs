using EP.Application.Commands.Chapters;
using EP.Application.Commands.Volumes;
using EP.Application.Queries.Chapter;
using EP.Application.Queries.Volume;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace app.Controllers
{
    [Route("api/v1/chapters")]
    [ApiController]
    public class ChaptersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ChaptersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost("add_volume")]
        public async Task<ActionResult> AddVolume(AddVolumeCommand command)
        {
            var affectedRows = await _mediator.Send(command);

            if (affectedRows > 0)
            {
                return new JsonResult(new
                {
                    EC = 0,
                    EM = "Thêm tập mới thành công"
                });
            }
            else
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Thêm tập mới thất bại!. Vui lòng thử lại sau."
                });
            }
        }

        [HttpPut("update_volume")]
        public async Task<ActionResult> UpdateVolume(UpdateVolumeCommand command)
        {
            var affectedRows = await _mediator.Send(command);

            if (affectedRows > 0)
            {
                return new JsonResult(new
                {
                    EC = 0,
                    EM = "Cập nhật tập thành công"
                });
            }
            else
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Cập nhật tập thất bại!. Vui lòng thử lại sau."
                });
            }
        }

        [HttpGet("volume_list")]
        public async Task<ActionResult> GetVolumeName(int storyId)
        {
            var query = new GetVolumeInStoryQuery { StoryId = storyId };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("story_volume")]
        public async Task<ActionResult> GetVolume(int storyId)
        {
            var query = new GetVolumesQuery(storyId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("add_chapter")]
        public async Task<ActionResult> AddChapter(AddChapterCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpGet("story_detail")]
        public async Task<ActionResult> GetStoryChapters(int storyId, int page, int pageSize)
        {
            var query = new GetStoryChapterQuery
            {
                StoryId = storyId,
                PageIndex = page,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }

        [Authorize]
        [HttpGet("chapter_information")]
        public async Task<ActionResult> GetChapterInfor(int chapterId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var query = new GetChapterToEditQuery { ChapterId = chapterId, UserId = userId };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [Authorize]
        [HttpPut("update_chapter")]
        public async Task<ActionResult> EditChapter(UpdateChapterCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [Authorize]
        [HttpPut("delete_chapter")]
        public async Task<ActionResult> DeleteChapter(int chapterId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var command = new DeleteChapterCommand
            {
                ChapterId = chapterId,
                UserId = userId
            };

            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpGet("chapter_content/{storyId}/{chapterNumber}")]
        public async Task<ActionResult> GetChapterContent(long chapterNumber, int storyId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var query = new GetChapterContentQuery
            {
                ChapterNumber = chapterNumber,
                StoryId = storyId,
                UserId = userId
            };
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}
