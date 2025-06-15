using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Models;
using NuGet.Common;
using app.Service;
using System.Drawing.Printing;
using app.Service.Caching;
using Microsoft.AspNetCore.Authorization;
using app.Interface;
using app.DTOs.Category;

namespace app.Controllers
{
    [Route("api/v1/category")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private MsgService _msgService = new MsgService();
        private readonly ICategoryRepository _categoryRepo;
        public CategoriesController(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult> GetCategories()
        {
            var cate = await _categoryRepo.GetAllCategories();
            return _msgService.MsgReturn(0, "Các thể loại truyện", cate);
        }

        // GET: api/Categories
        [HttpGet("cate_shelves_detail")]
        public async Task<ActionResult> GetCategoryById(int cateId)
        {
            var category = await _categoryRepo.GetCategoryById(cateId);
            if (category == null) return _msgService.MsgActionReturn(-1, "Không có loại đó");
            return _msgService.MsgReturn(0, "Chi tiết thể loại", category);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _categoryRepo.GetCategoryById(id); ;
            if (category == null)
            {
                return NotFound();
            }
            return category;
        }

        // GET: api/filter
        [HttpGet("options")]
        public async Task<ActionResult> GetOptionFilter()
        {
            var optionFilter = await _categoryRepo.GetOptionFilter();
            var status = new List<object>
                {
                    new { Name = "Hoàn thành", Value = 2 },
                    new { Name = "Chưa hoàn thành", Value = 1 }
                };

            return _msgService.MsgReturn(0, "Trường tìm kiếm", new { cate = optionFilter.Categories, to = optionFilter.To, from = optionFilter.From, status });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Không tìm thấy thể loại!"
                });
            }
            if (category.CategoryName == "" || category.CategoryName == null)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Tên thể loại không được để trống!"
                });
            }
            try
            {
                await _categoryRepo.UpdateCategory(category);
            }
            catch
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Hệ thống xảy ra lỗi!"
                });
            }

            return new JsonResult(new
            {
                EC = 0,
                EM = "Cập nhật thể loại thành công!"
            });
        }

        [HttpPost("addCategory")]
        public async Task<ActionResult> addCategory(addCategoryDto category)
        {
            if (category.CategoryName == "" || category.CategoryName == null )
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Tên thể loại không được để trống"
                });
            }
            try
            {
                bool result = await _categoryRepo.AddCategory(category);
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

    }
}
