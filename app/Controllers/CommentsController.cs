using app.DTOs.Comment;
using app.Interface;
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
        private MsgService _msgService = new MsgService();
        private readonly ICommentRepository _commentRepo;
        public CommentsController(ICommentRepository commentRepository)
        {
            _commentRepo = commentRepository;
        }

        [HttpPost("send")]
        public async Task<ActionResult> SendComment(SendCommentDto newComment)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!ModelState.IsValid) return _msgService.MsgActionReturn(-1, "Thiếu điều kiện");
            try
            {
                await _commentRepo.AddComment(userId, newComment);
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

        [HttpPost("edit")]
        public async Task<ActionResult> EditComment(int commentId, [FromBody] CommentUpdateDto commentUpdate)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                var result = await _commentRepo.UpdateComment(userId, commentId, commentUpdate.CommentContent);
                if (!result)
                {
                    return _msgService.MsgActionReturn(-1, "Comment không tồn tại");
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
            return _msgService.MsgActionReturn(0, "Bình luận thành công");
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("delete_comment")]
        public async Task<ActionResult> DeleteComment(int commentId)
        {
            try
            {
                var result = await _commentRepo.DeleteComment(commentId); 
                if (!result)
                {
                    return new JsonResult(new
                    {
                        EC = -1,
                        EM = "Bình luận không tồn tại"
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

            return new JsonResult(new
            {
                EC = 0,
                EM = "Xóa bình luận thành công!"
            });
        }

        [AllowAnonymous]
        [HttpGet("story_detail")]
        public async Task<ActionResult> GetStoryComments(int storyId, int page, int pageSize)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var comments = await _commentRepo.GetStoryComments(userId, storyId);
            pageSize = pageSize == null ? 10 : pageSize;
            return _msgService.MsgPagingReturn("Bình luận của truyện",
                comments.Skip(pageSize * (page - 1)).Take(pageSize), page, pageSize, comments.Count);
        }

        [AllowAnonymous]
        [HttpGet("chapter_content")]
        public async Task<ActionResult> GetChapterComments(int chapterId, int page, int pageSize)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var comments = await _commentRepo.GetChapterComments(userId, chapterId);
            pageSize = pageSize == null ? 10 : pageSize;
            return _msgService.MsgPagingReturn("Bình luận của chương",
                comments.Skip(pageSize * (page - 1)).Take(pageSize), page, pageSize, comments.Count);
        }

    }
}
