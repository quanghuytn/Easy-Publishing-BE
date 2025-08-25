using EP.Application.Commands.Stories;
using EP.Application.Common.DTOs.Story;
using EP.Application.Queries.Common;
using EP.Application.Queries.Stories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace app.Controllers
{
    [Route("api/v1/story")]
    [ApiController]
    public class StoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StoriesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("story_detail")]
        public async Task<ActionResult> GetStoryDetail(int storyId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var query = new GetStoryDetailQuery(storyId, userId);
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [HttpGet("story_detail/related")]
        public async Task<ActionResult> GetStoryDetailRelate(int storyId)
        {
            var query = new GetRelatedStoriesQuery(storyId);
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("prints")]
        public async Task<ActionResult> CreatePrint(int storyId, int authorId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var query = new GetStoryForPrintQuery(storyId, userId);
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [HttpGet("searchOptions")]
        public async Task<ActionResult> GetOptionFilter()
        {
            var query = new GetSearchGlobalOptionQuery();
            var response = await _mediator.Send(query);

            return Ok(response);
        }
        //[HttpGet("test")]
        //public async Task<ActionResult> Test()
        //{
        //    var stories = await _context.Stories
        //    .Include(s => s.Author)
        //    .Include(s => s.Categories)
        //        .Include(s => s.StoryInteraction)
        //        .Select(s => new
        //        {
        //            StoryId = s.StoryId,
        //            StoryTitle = s.StoryTitle,
        //            StoryImage = s.StoryImage,
        //            StoryDescription = s.StoryDescription.Substring(0, 100) + "...",
        //            StoryCategories = s.Categories.Select(c => new { CategoryId = c.CategoryId.ToString(), c.CategoryName }).ToList(),
        //            StoryAuthor = new { s.Author.UserId, s.Author.UserFullname },
        //            StoryCreateTime = s.CreateTime,
        //            StoryPrice = s.StoryPrice,
        //            Status = s.Status,

        //        })
        //        .ToListAsync();
        //    foreach(var story in stories)
        //    {
        //        await _cache.AddStoryAsync(story.StoryId, story);
        //    }
        //    return _msgService.MsgReturn(0, "Kết quả tìm kiếm", stories);
        //}

        [HttpGet("search_global")]
        public async Task<ActionResult> SearchGlobal(string? search, int? authorId, int? from, int? to, int? status, [FromQuery] List<int> cates)
        {
            var query = new SearchGlobalQuery(search, authorId, from, to, status, cates);
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("story_information")]
        public async Task<ActionResult> GetStoryInfor(int storyId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var query = new GetStoryInformationQuery(storyId, userId);
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("upload_image")]
        public async Task<IActionResult> GetImage([FromForm] GetStoryImageDto data)
        {
            try
            {
                if (data.image.Length > 0)
                {
                    using var stream = data.image.OpenReadStream();
                    var command = new UploadStoryImageCommand
                    {
                        FileName = data.image.FileName,
                        FileStream = stream,
                        PreviousFilename = data.previousImage
                    };
                    var response = await _mediator.Send(command);
                    
                    return Ok(command);
                }
                else
                {
                    return new JsonResult(new
                    {
                        EC = -1,
                        EM = "File không tồn tại"
                    });
                }
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

        [Authorize]
        [HttpPost("save_story")]
        public async Task<ActionResult> SaveStory(AddStoryCommand command)
        {
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("update_story")]
        public async Task<ActionResult> EditStory(EditStoryCommand command)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            command.UserId = userId;
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [HttpGet("authorAndStoryNumber")]
        public async Task<ActionResult> GetAuthorAndStoryNumber()
        {
            var query = new GetAuthorAndStoryNumberQuery();
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [Authorize]
        [HttpDelete("{storyId}")]
        public async Task<ActionResult> DeleteStory(int storyId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var command = new DeleteStoryCommand
            {
                StoryId = storyId,
                UserId = userId
            };
            var response = await _mediator.Send(command);

            return Ok(response);
        }

        [HttpGet("all_stories")]
        public async Task<ActionResult> GetStories()
        {
            var query = new GetAllStoriesQuery();
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("story_image")]
        public async Task<IActionResult> ChangeStoryImage([FromForm] StoryImageDto data)
        {
            try
            {
                int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                using var stream = data.image.OpenReadStream();
                var command = new UploadStoryImageCommand
                {
                    FileName = data.image.FileName,
                    FileStream = stream,
                    StoryId = data.StoryId,
                    UserId = userId
                };
                var response = await _mediator.Send(command);

                return Ok(command);
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
    }
}
