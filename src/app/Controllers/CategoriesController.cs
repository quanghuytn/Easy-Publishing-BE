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
        private MsgService _msgService = new MsgService();
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
            var optionFilter = await _mediator.Send(command);
            var status = new List<object>
                {
                    new { Name = "Hoàn thành", Value = 2 },
                    new { Name = "Chưa hoàn thành", Value = 1 }
                };

            return _msgService.MsgReturn(0, "Trường tìm kiếm", new { categories = optionFilter.Categories, to = optionFilter.To, from = optionFilter.From, status });
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
            if (command.CategoryName == "" || command.CategoryName == null)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Tên thể loại không được để trống!"
                });
            }

            await _mediator.Send(command);

            return new JsonResult(new
            {
                EC = 0,
                EM = "Cập nhật thể loại thành công!"
            });
        }

        [HttpPost("addCategory")]
        public async Task<ActionResult> addCategory(AddCategoryCommand command)
        {
            if (command.CategoryName == "" || command.CategoryName == null )
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Tên thể loại không được để trống"
                });
            }
            try
            {
                bool result = await _mediator.Send(command);

                if (result)
                {
                    return new JsonResult(new
                    {
                        EC = 0,
                        EM = "Thêm thể loại thành công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        EC = -1,
                        EM = "Thể loại đã tồn tại"
                    });
                }
            }
            catch
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Hệ thống xảy ra lỗi!"
                });
            }
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
