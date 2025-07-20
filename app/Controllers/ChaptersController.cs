using app.DTOs.Chapter;
using app.Interface;
using app.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace app.Controllers
{
    [Route("api/v1/chapters")]
    [ApiController]
    public class ChaptersController : ControllerBase
    {
        private readonly IChapterRepository _chapterRepo;
        private MsgService _msgService = new MsgService();
        private int pagesize = 10;
        public ChaptersController(IChapterRepository chapterRepo)
        {
            _chapterRepo = chapterRepo;
        }

        [HttpPost("add_volume")]
        public async Task<ActionResult> AddVolume(AddVolumeDto volume)
        {
            if (volume.VolumeTitle.IsNullOrEmpty())
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Thêm tập thất bại!"
                });
            }
            try
            {
                var result = await _chapterRepo.AddVolume(volume);
                if (result)
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
                        EM = "Tập gần nhất phải có ít nhất hai chương"
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

        [HttpPut("update_volume")]
        public async Task<ActionResult> UpdateVolume(VolumeDto volume)
        {
            if (volume.VolumeTitle.IsNullOrEmpty())
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Cập nhật thất bại!"
                });
            }
            try
            {
                var result = await _chapterRepo.UpdateVolume(volume);
                if (result)
                {
                    return new JsonResult(new
                    {
                        EC = 0,
                        EM = "Cập nhật thành công"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        EC = -1,
                        EM = "Tập không tồn tại"
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

        [HttpGet("volume_list")]
        public async Task<ActionResult> GetVolumeName(int storyId)
        {
            var volumes = await _chapterRepo.GetVolumesByStory(storyId);
            return _msgService.MsgReturn(0, "Danh sách tập", volumes);
        }

        [HttpGet("story_volume")]
        public async Task<ActionResult> GetVolume(int storyId)
        {
            var volumes = await _chapterRepo.GetVolumes(storyId);
            return _msgService.MsgReturn(0, "Danh sách tập cụ thể", volumes);
        }

        [HttpPost("add_chapter")]
        public async Task<ActionResult> AddChapter(AddChapterDto chapter)
        {
            if (chapter.ChapterContentHtml.IsNullOrEmpty() || chapter.ChapterContentMarkdown.IsNullOrEmpty())
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Không được để trống nội dung!"
                });
            }
            try
            {
                await _chapterRepo.AddChapter(chapter);
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
                EM = "Thêm chương mới thành công"
            });
        }

        [HttpGet("story_detail")]
        public async Task<ActionResult> GetStoryChapters(int storyId, int page, int pageSize)
        {
            var chapters = await _chapterRepo.GetStoryChapters(storyId);
            pageSize = pageSize == null || pageSize == 0 ? pagesize : pageSize;
            return _msgService.MsgPagingReturn("Danh sách chương",
                chapters.Skip(pageSize * (page - 1)).Take(pageSize), page, pageSize, chapters.Count);
        }

        [Authorize]
        [HttpGet("chapter_information")]
        public async Task<ActionResult> GetChapterInfor(int chapterId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var chapter = await _chapterRepo.GetChapterInfor(chapterId);
            if (chapter == null)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Chương không tồn tại"
                });
            }
            var checkResult = await _chapterRepo.CheckReadPermission(userId, chapter.StoryId, chapterId);
            if (!checkResult)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Bạn không được quyền vào trang này"
                });
            }
            return _msgService.MsgReturn(0, "Thông tin chương", chapter);
        }

        [HttpPut("update_chapter")]
        public async Task<ActionResult> EditChapter(UpdateChapterDto chapter)
        {
            if (chapter.ChapterContentHtml.IsNullOrEmpty() || chapter.ChapterContentMarkdown.IsNullOrEmpty())
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Không được để trống nội dung chương!"
                });
            }
            try
            {
                var result = await _chapterRepo.UpdateChapter(chapter);
                if (result)
                {
                    return new JsonResult(new
                    {
                        EC = 0,
                        EM = "Cập nhật thành công!"
                    });
                }
                else
                {
                    return new JsonResult(new
                    {
                        EC = -1,
                        EM = "Cập nhật thất bại!"
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

        [HttpPut("delete_chapter")]
        public async Task<ActionResult> DeleteChapter(int chapterId)
        {
            try
            {
                var result = await _chapterRepo.DeleteChapter(chapterId);
                if (!result)
                {
                    return new JsonResult(new
                    {
                        EC = -1,
                        EM = "Chương không tồn tại"
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
                EM = "Xóa chương thành công!"
            });
        }

        [HttpGet("chapter_content/{storyId}/{chapterNumber}")]
        public async Task<ActionResult> GetChapterContent(long chapterNumber, int storyId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var chapter = await _chapterRepo.GetChapterContent(userId, chapterNumber, storyId);

            if (chapter == null) {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Chương không tồn tại"
                });
            }
            return _msgService.MsgReturn(0, "Nội dung chương", chapter);

        }

        
    }
}
