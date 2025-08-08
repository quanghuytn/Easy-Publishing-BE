using EP.Application.Queries.Shelves;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Security.Claims;

namespace app.Controllers
{
    [Route("api/v1/shelves")]
    [ApiController]
    public class ShelvesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShelvesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[HttpGet("top_famous")]
        //[EnableQuery]
        //public async Task<ActionResult> GetTopFamousStories(int page)
        //{
        //    var stories = await _shelvesRepository.GetTopFamousStories();
        //    return _msgService.MsgPagingReturn("Top nổi bật",
        //        stories.Skip(pagesize * (page - 1)).Take(pagesize), page, pagesize, stories.Count);
        //}

        [HttpGet("minimal_top_famous")]
        [EnableQuery]
        public async Task<ActionResult> GetMinimalTopFamousStories(int page)
        {
            var query = new GetMinimalTopFamousStoriesQuery { PageIndex = page };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        //[HttpGet("top_latest_by_chapter")]
        //[EnableQuery]
        //public async Task<ActionResult> GetTopLatestStoriesByChapter(int page)
        //{
        //    var stories = await _shelvesRepository.GetTopLatestStoriesByChapter();
        //    return _msgService.MsgPagingReturn("Truyện mới update",
        //        stories.Skip(pagesize * (page - 1)).Take(pagesize), page, pagesize, stories.Count);
        //}

        [HttpGet("minimal_top_latest_by_chapter")]
        public async Task<ActionResult> GetMinimalTopLatestStoriesByChapter(int page)
        {
            var query = new GetMinimalTopLatestStoriesByChapterQuery { PageIndex = page };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("top6_purchase")]
        public async Task<ActionResult> GetTop6StoriesBuy()
        {
            var query = new GetTop6StoriesPurchaseQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("top6_sale")]
        public async Task<ActionResult> GetTop6StoriesSale()
        {
            var query = new GetTop6StoriesSaleQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("top6_authorRevenue")]
        public async Task<ActionResult> GetTop6AuthorRevenue()
        {
            var query = new GetTop6AuthorRevenueQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        //// GET: api/Stories : top read story
        //[HttpGet("top_read")]
        //[EnableQuery]
        //public async Task<ActionResult> GetTopStoriesRead(int page)
        //{
        //    var stories = await _shelvesRepository.GetTopStoriesRead();
        //    return _msgService.MsgPagingReturn("Top lượt đọc",
        //        stories.Skip(pagesize * (page - 1)).Take(pagesize), page, pagesize, stories.Count);
        //}

        [HttpGet("minimal_top_read")]
        public async Task<ActionResult> GetMinimalTopStoriesRead(int page)
        {
            var query = new GetMinimalTopStoriesReadQuery { PageIndex = page };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        

        [HttpGet("minimal_top_newest")]
        public async Task<ActionResult> GetMinimalTopLatestStories(int page)
        {
            var query = new GetMinimalTopLatestStoriesQuery { PageIndex = page};
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        // GET: api/Stories : stories of each cate
        [HttpGet("cate_stories")]
        public async Task<ActionResult> GetStoriesFollowCategories()
        {
            var query = new GetStoriesInCategoryShelfQuery();
            var result = await _mediator.Send(query);

            return Ok(result);
        }


        // GET: api/Stories : top read shelves cate
        [HttpGet("topcate_read")]
        [EnableQuery]
        public async Task<ActionResult> GetTopStoriesReadShelves(int cateId)
        {
            var query = new GetTopStoriesReadShelvesQuery { CategoryId = cateId };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        // get stories each cate
        [HttpGet("topcate_shelves")]
        [EnableQuery]
        public async Task<ActionResult> GetStoriesTopCate(int cateId)
        {
            var query = new GetStoriesTopCateQuery { CategoryId = cateId};
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        // get stories each cate
        [HttpGet("cate_shelves")]
        [EnableQuery]
        public async Task<ActionResult> GetStoriesEachCate(int cateId, int page, int pageSize)
        {
            var query = new GetStoriesEachCateQuery { CategoryId = cateId, PageIndex = page};
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        // get stories each cate
        [HttpGet("cate_shelves_done")]
        [EnableQuery]
        public async Task<ActionResult> GetStoriesDoneEachCate(int cateId, int page, int pageSize)
        {
            var query = new GetStoriesDoneEachCateQuery { CategoryId = cateId, PageIndex = page, PageSize = pageSize };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        // get stories by filter
        [HttpGet("filter")]
        [EnableQuery]
        public async Task<ActionResult> GetFilter(string? title, int? to, int? from, string? sort, [FromQuery] List<int> cates,
            int? status, int page, int pageSize)
        {
            var query = new FilterStoryQuery
            {
                Title = title,
                To = to,
                From = from,
                Sort = sort,
                Cates = cates,
                Status = status,
                PageIndex = page,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("author_detail/written")]
        public async Task<ActionResult> GetStoryByAuthorId(int authorId)
        {
            var query = new GetWrittenStoryOfAuthorQuery(authorId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("author_detail/top_famous")]
        public async Task<ActionResult> GetStoryFamousByAuthorId(int authorId)
        {
            var query = new GetTopFamousStoryOfAuthorQuery(authorId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("author_detail/top_purchase")]
        public async Task<ActionResult> GetStoryPurchaseByAuthorId(int authorId)
        {
            var query = new GetTopPurchaseStoryOfAuthorQuery(authorId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("author_detail/top_newest_by_chapter")]
        public async Task<ActionResult> GetStoryNewestByAuthorId(int authorId)
        {
            var query = new GetNewestStoryOfAuthorQuery(authorId);
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("author_manage")]
        public async Task<ActionResult> GetStoryOfAuthor(string? title, [FromQuery] string? sort, int page, int pageSize)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var query = new GetStoryOfAuthorQuery
            {
                AuthorId = userId,
                Title = title,
                Sort = sort,
                PageIndex = page,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        // get stories owned
        [Authorize]
        [HttpGet("my_owned")]
        [EnableQuery]
        public async Task<ActionResult> GetMyOwned(int page, int pageSize)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var query = new GetOwnedStoryQuery
            {
                UserId = userId,
                Page = page,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        // get stories follow
        [Authorize]
        [HttpGet("my_follow")]
        [EnableQuery]
        public async Task<ActionResult> GetMyFollow(int page, int pageSize)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var query = new GetFollowedStoryQuery
            {
                UserId = userId,
                PageIndex = page,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("my_read")]
        [EnableQuery]
        public async Task<ActionResult> GetMyReadHistory(int page, int pageSize)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var query = new GetReadHistoryQuery
            {
                UserId = userId,
                Page = page,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        //// GET: api/Stories : top price accend story 
        //[HttpGet("top_free")]
        //[EnableQuery]
        //public async Task<ActionResult> GetTopPriceStories(int page)
        //{
        //    var stories = await _shelvesRepository.GetTopPriceStories();
        //    return _msgService.MsgPagingReturn("Truyện miễn phí",
        //       stories.Skip(pagesize * (page - 1)).Take(pagesize), page, pagesize, stories.Count);
        //}

        // GET: api/Stories : top latest story
        //[HttpGet("top_newest")]
        //[EnableQuery]
        //public async Task<ActionResult> GetTopLatestStories(int page)
        //{
        //    var stories = await _shelvesRepository.GetTopLatestStories();
        //    return _msgService.MsgPagingReturn("Truyện mới thêm",
        //       stories.Skip(pagesize * (page - 1)).Take(pagesize), page, pagesize, stories.Count);
        //}
    }
}
