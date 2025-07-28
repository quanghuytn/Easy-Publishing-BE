using app.Interface;
using app.Service;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers
{
    [Route("api/v1/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepo;
        private MsgService _msgService = new MsgService();
        public AuthorsController(IAuthorRepository authorRepository)
        {
            _authorRepo = authorRepository;
        }

        [HttpGet("story_detail")]
        public async Task<ActionResult> GetStoryRelateAuthor(int storyId)
        {
            var author = await _authorRepo.GetStoryRelateAuthor(storyId);
            return _msgService.MsgReturn(0, "Tác giả liên quan", author);
        }

        [HttpGet("author_detail")]
        public async Task<ActionResult> GetAuthor(int authorId)
        {
            var author = await _authorRepo.GetAuthorById(authorId);
            return _msgService.MsgReturn(0, "Thông tin tác giả", author);
        }
    }
}
