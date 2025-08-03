using EP.Application.Queries.Author;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers
{
    [Route("api/v1/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthorsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("story_detail")]
        public async Task<ActionResult> GetStoryRelateAuthor(int storyId)
        {
            var query = new GetStoryRelateAuthorQuery(storyId);
            var response = await _mediator.Send(query);

            return Ok(response);
        }

        [HttpGet("author_detail")]
        public async Task<ActionResult> GetAuthor(int authorId)
        {
            var query = new GetAuthorByIdQuery(authorId);
            var response = await _mediator.Send(query);

            return Ok(response);
        }
    }
}
