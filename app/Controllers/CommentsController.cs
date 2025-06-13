using app.DTOs;
using app.Models;
using app.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace app.Controllers
{
    [Authorize]
    [Route("api/v1/comments")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly EasyPublishingContext _context;
        private MsgService _msgService = new MsgService();
        //private int pageSize = 10;
        public CommentsController(EasyPublishingContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("story_detail")]
        public async Task<ActionResult> GetStoryComments(int storyId, int page, int pageSize)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var comments = await _context.Comments.Where(c => c.StoryId == storyId)
                .Include(c => c.User)
                .Select(c => new
                {
                    UserComment = new { c.User.UserId, c.User.UserFullname, c.User.UserImage },
                    CommentId = c.CommentId,
                    CommentContent = c.CommentContent,
                    CommentDate = c.CommentDate,
                    CommentWriter = userId == c.UserId ? true : false
                })
                .OrderByDescending(c => c.CommentId)
                .ToListAsync();
            pageSize = pageSize == null ? 10 : pageSize;
            return _msgService.MsgPagingReturn("Bình luận của truyện",
                comments.Skip(pageSize * (page - 1)).Take(pageSize), page, pageSize, comments.Count);
        }

        [AllowAnonymous]
        [HttpGet("chapter_content")]
        public async Task<ActionResult> GetChapterComments(int chapterId, int page, int pageSize)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var comments = await _context.Comments.Where(c => c.StoryId == chapterId)
                .Include(c => c.User)
                .Select(c => new
                {
                    UserComment = new { c.User.UserId, c.User.UserFullname, c.User.UserImage },
                    CommentId = c.CommentId,
                    CommentContent = c.CommentContent,
                    CommentDate = c.CommentDate,
                    CommentWriter = userId == c.UserId ? true : false
                })
                .OrderByDescending(c => c.CommentId)
                .ToListAsync();
            pageSize = pageSize == null ? 10 : pageSize;
            return _msgService.MsgPagingReturn("Bình luận của chương",
                comments.Skip(pageSize * (page - 1)).Take(pageSize), page, pageSize, comments.Count);
        }


        [HttpPost("send")]
        public async Task<ActionResult> SendComment(CommentDTO commentDTO)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!ModelState.IsValid) return _msgService.MsgActionReturn(-1, "Thiếu điều kiện");
            try
            {
                Comment cmt = new Comment()
                {
                    UserId = userId,
                    StoryId = commentDTO.StoryId,
                    ChapterId = commentDTO.ChapterId,
                    CommentContent = commentDTO.CommentContent,
                    CommentDate = DateTime.Now,
                };
                _context.Comments.Add(cmt);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Hệ thống xảy ra lỗi!"
                });
            }
            return _msgService.MsgActionReturn(0, "Bình luận thành công");
        }

        public class CommentUpdateModel
        {
            public string CommentContent { get; set; }
        }

        [HttpPost("edit")]
        public async Task<ActionResult> EditComment(int commentId, [FromBody] CommentUpdateModel cmtUpdate)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            Comment cmt = await _context.Comments.FirstOrDefaultAsync(c => c.UserId == userId && c.CommentId == commentId);
            if (cmt == null) return _msgService.MsgActionReturn(-1, "Comment không tồn tại");

            try
            {
                if (String.IsNullOrEmpty(cmtUpdate.CommentContent)) _context.Comments.Remove(cmt);
                else
                {
                    cmt.CommentContent = cmtUpdate.CommentContent;
                    _context.Entry(cmt).State = EntityState.Modified;
                }

                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Hệ thống xảy ra lỗi!"
                });
            }
            return _msgService.MsgActionReturn(0, "Bình luận thành công");
        }

        [HttpDelete("delete_comment")]
        public async Task<ActionResult> DeleteComment(int commentId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if(user.RoleId != 1)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Bạn không có quyền thực hiện chức năng này!"
                });
            }

            try
            {
                var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);
                if(comment == null)
                {
                    return new JsonResult(new
                    {
                        EC = -1,
                        EM = "Bình luận không tồn tại"
                    });
                }
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

            }
            catch (Exception)
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
                EM = "Xóa bình luận thành công!"
            });
        }
    }
}
