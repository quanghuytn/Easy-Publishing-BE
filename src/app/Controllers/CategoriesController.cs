using app.DTOs.Category;
using app.Interface;
using app.Models;
using app.Service;
using EP.Application.Commands.Categories;
using EP.Application.Queries.Category;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers
{
    [Route("api/v1/category")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult> GetCategories()
        {
            var query = new GetAllCategoryQuery { };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("cate_shelves_detail")]
        public async Task<ActionResult> GetCategoryById(int cateId)
        {
            var query = new GetCategoryByIdQuery { CategoryId = cateId };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        [HttpGet("options")]
        public async Task<ActionResult> GetOptionFilter()
        {
            var command = new GetOptionFilterQuery { };
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, UpdateCategoryCommand command)
        {
            if (id != command.CategoryId)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Không tìm thấy thể loại!"
                });
            }


            var affectedRows = await _mediator.Send(command);
            
            if( affectedRows < 1)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Cập nhật thể loại thất bại! Vui lòng thử lại sau"
                });
            }

            return new JsonResult(new
            {
                EC = 0,
                EM = "Cập nhật thể loại thành công!"
            });
        }

        [HttpPost("addCategory")]
        public async Task<ActionResult> addCategory(AddCategoryCommand command)
        {

            int affectedRows = await _mediator.Send(command);

            if (affectedRows < 1)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Thêm thể loại thất bại! Vui lòng thử lại sau"
                });
            }

            return new JsonResult(new
            {
                EC = 0,
                EM = "Thêm thể loại thành công"
            });
        }

        //[HttpGet("{id}")]
        //public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        //{
        //    var category = await _categoryRepo.GetCategoryById(id); ;
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }
        //    return category;
        //}

    }
}
