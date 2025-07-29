using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Models;
using app.Service;
using app.Service.Caching;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using app.DTOs.Story;

namespace app.Controllers
{
    [Route("api/v1/story")]
    [ApiController]
    public class StoriesController : ControllerBase
    {
        private readonly EasyPublishingContext _context;
        private MsgService _msgService = new MsgService();
        private readonly IRedisCacheService _cache;

        public StoriesController(EasyPublishingContext context, IRedisCacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        /// GET: api/Stories
        [HttpGet("get_all_stories")]
        public async Task<ActionResult> GetStories()
        {
            if (_context.Stories == null)
            {
                return NotFound();
            }
            var stories = await _context.Stories
                .Include(s => s.Author)
                .Include(s => s.Comments)
                .Include(s => s.ReportContents)
                .Include(s => s.StoryFollowLikes)
                .Include(s => s.Volumes)
                .Include(s => s.Chapters)
                .Include(s => s.StoryInteraction)
                .Include(s => s.StoryReads)
                .Include(s => s.Categories)
                .Select(c => new
                {
                    StoryId = c.StoryId,
                    StoryTitle = c.StoryTitle,
                    StoryImage = c.StoryImage,
                    StoryDescription = c.StoryDescriptionHtml.Substring(0, 90) + "...",
                    StoryPrice = c.StoryPrice,
                    StorySale = c.StorySale,
                    CreateTime = c.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    StoryCategories = string.Join(",", c.Categories.Select(c => c.CategoryName).ToList()),
                    StoryAuthor = c.Author.UserFullname,
                    StoryChapterNumber = c.Chapters.Count,
                    StoryChapters = c.Chapters.Where(c => c.Status > 0).Count(),
                    StoryReads = c.StoryReads.Count(),
                    Volumes = c.Volumes.Count(),
                    UserOwned = c.Users.Count(),
                    Status = c.Status,
                    UserFollow = c.StoryFollowLikes.Where(c => c.Follow == true).Count(),
                    UserLike = c.StoryFollowLikes.Where(c => c.Like == true).Count()
                })
                .ToListAsync();
            return _msgService.MsgReturn(0, "Thông tin truyện", stories);
        }

        [HttpGet("story_detail")]
        public async Task<ActionResult> GetStoryDetail(int storyId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var lastReadChapterNumber = await GetLastestChapterUserRead(storyId, userId);
            var story = await _context.Stories.Where(c => c.StoryId == storyId && c.Status > 0)
                        .Include(c => c.Author).Include(c => c.StoryInteraction)
                        .Include(c => c.Categories)
                        .Include(c => c.Users) // luot mua truyen
                        .Include(c => c.Chapters).ThenInclude(c => c.Users)
                        .Include(c => c.StoryFollowLikes)
                        .Select(c => new
                        {
                            StoryId = c.StoryId,
                            StoryTitle = c.StoryTitle,
                            StoryImage = c.StoryImage,
                            StoryDescription = c.StoryDescriptionHtml,
                            StoryPrice = c.StoryPrice,
                            StorySale = c.StorySale,
                            CreateTime = c.CreateTime,
                            StoryCategories = c.Categories.ToList(),
                            StoryAuthor = new { c.Author.UserId, c.Author.UserFullname },
                            StoryChapterNumber = c.Chapters.Count,
                            StoryChapters = c.Chapters.Where(c => c.Status > 0).Select(c => new
                            {
                                c.ChapterId,
                                c.ChapterNumber,
                                c.ChapterTitle,
                                c.ChapterPrice,
                                c.CreateTime

                            }).OrderByDescending(c => c.ChapterNumber)
                            .Take(3).ToList(),
                            UserPurchaseStory = c.Users.Count,
                            StoryInteraction = c.StoryInteraction,
                            AuthorOwned = userId == c.AuthorId ? true : false,
                            UserOwned = c.Users.Any(c => c.UserId == userId),
                            LastReadChapter = lastReadChapterNumber,
                            UserFollow = c.StoryFollowLikes.Any(c => c.UserId == userId && c.Follow == true),
                            UserLike = c.StoryFollowLikes.Any(c => c.UserId == userId && c.Like == true),
                        })
                        .FirstOrDefaultAsync();

            var story_interaction = await _context.StoryInteractions.FirstOrDefaultAsync(c => c.StoryId == storyId);
            story_interaction.View += 1;
            _context.Entry(story_interaction).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return _msgService.MsgReturn(0, "Thông tin truyện", story);
        }

        private async Task<long> GetLastestChapterUserRead(int storyId, int userId)
        {
            if (userId == 0) return 1;
            var lastReadChapter = await _context.StoryReads.Where(sr => sr.UserId == userId && sr.StoryId == storyId)
                                            .OrderByDescending(sr => sr.ReadTime)
                                            .Select(sr => sr.Chapter)
                                            .FirstOrDefaultAsync();
            var chapterNumber = lastReadChapter?.ChapterNumber ?? 1;
            return chapterNumber;
        }

        [HttpGet("story_detail/related")]
        public async Task<ActionResult> GetStoryDetailRelate(int storyId)
        {
            var story = await _context.Stories.Include(c => c.Categories).FirstOrDefaultAsync(c => c.StoryId == storyId);
            var cates = story.Categories.Select(c => c.CategoryId).ToList();
            var stories = await _context.Stories.Where(c => c.StoryId != storyId && c.Status > 0)
                .Include(c => c.Categories)
                .Include(c => c.Chapters)
                .Select(c => new
                {
                    StoryId = c.StoryId,
                    StoryTitle = c.StoryTitle,
                    StoryImage = c.StoryImage,
                    StoryPrice = c.StoryPrice,
                    StorySale = c.StorySale,
                    StoryCategories = c.Categories.Select(c => new { c.CategoryId, c.CategoryName }).ToList(),
                    StoryAuthor = new { c.Author.UserId, c.Author.UserFullname },
                    StoryCreateTime = c.CreateTime,
                    StoryChapterNumber = c.Chapters.Count,
                    StoryLatestChapter = c.Chapters.Where(c => c.Status > 0).OrderByDescending(c => c.ChapterNumber).FirstOrDefault() == null ? null :
                    new
                    {
                        c.Chapters.Where(c => c.Status > 0).OrderByDescending(c => c.ChapterNumber).FirstOrDefault().ChapterId,
                        c.Chapters.Where(c => c.Status > 0).OrderByDescending(c => c.ChapterNumber).FirstOrDefault().ChapterNumber,
                        c.Chapters.Where(c => c.Status > 0).OrderByDescending(c => c.ChapterNumber).FirstOrDefault().ChapterTitle,
                        c.Chapters.Where(c => c.Status > 0).OrderByDescending(c => c.ChapterNumber).FirstOrDefault().CreateTime
                    }
                })
                .OrderByDescending(c => c.StoryId)
                .ToListAsync();
            var verified = stories.Where(c => c.StoryCategories.Any(cat => cates.Contains(cat.CategoryId)) && c.StoryChapterNumber > 0).ToList();
            return _msgService.MsgReturn(0, "Truyện liên quan", verified.Take(3));
        }

        [Authorize]
        [HttpGet("prints")]
        public async Task<ActionResult> CreatePrint(int storyId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var data = await _context.Stories.Where(s => s.StoryId == storyId && s.AuthorId == userId)
                    .Include(c => c.Volumes).ThenInclude(c => c.Chapters)
                    .Select(c => new
                    {
                        StoryTitle = c.StoryTitle,
                        StoryImage = c.StoryImage,
                        StoryDescription = c.StoryDescription,
                        StoryDescriptionHtml = c.StoryDescriptionHtml,
                        StoryDescriptionMarkdown = c.StoryDescriptionMarkdown,
                        StoryPrice = c.StoryPrice,
                        StoryVolumes = c.Volumes.Select(s => new
                        {
                            VolumeNumber = s.VolumeNumber,
                            VolumeTitle = s.VolumeTitle,
                            VolumeChapters = s.Chapters.Select(se => new
                            {
                                se.ChapterNumber,
                                se.ChapterTitle,
                                se.ChapterContentMarkdown,
                                se.ChapterContentHtml
                            })
                        }).ToList(),
                    }).FirstOrDefaultAsync();

            if (data == null) return _msgService.MsgActionReturn(-1, "Bạn không sở hữu truyện");

            return _msgService.MsgReturn(0, "List Story", data);
        }

        [HttpGet("searchOptions")]
        public async Task<ActionResult> GetOptionFilter()
        {
            var author = await _context.Users.Where(u => u.Stories.Any())
                .Select(a => new
                {
                    AuthorId = a.UserId,
                    AuthorName = a.UserFullname,
                    AuthorImage = a.UserImage,
                }).ToListAsync();
            var cate = await _context.Categories
                .Include(c => c.Stories)
                .Select(c => new
                {
                    c.CategoryId,
                    c.CategoryName,
                    c.CategoryDescription
                })
                .ToListAsync();
            var stories = await _context.Stories.Select(s => new { s.StoryPrice, }).OrderByDescending(s => s.StoryPrice).ToListAsync();
            var to = stories.Max(c => c.StoryPrice);
            var from = stories.Min(c => c.StoryPrice);
            var status = new List<object>
                {
                    new { Name = "Hoàn thành", Value = 2 },
                    new { Name = "Chưa hoàn thành", Value = 1 }
                };

            return _msgService.MsgReturn(0, "Trường tìm kiếm", new { author, cate, to, from, status });
        }
        [HttpGet("test")]
        public async Task<ActionResult> Test()
        {
            var stories = await _context.Stories
            .Include(s => s.Author)
            .Include(s => s.Categories)
                .Include(s => s.StoryInteraction)
                .Select(s => new
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryDescription = s.StoryDescription.Substring(0, 100) + "...",
                    StoryCategories = s.Categories.Select(c => new { CategoryId = c.CategoryId.ToString(), c.CategoryName }).ToList(),
                    StoryAuthor = new { s.Author.UserId, s.Author.UserFullname },
                    StoryCreateTime = s.CreateTime,
                    StoryPrice = s.StoryPrice,
                    Status = s.Status,

                })
                .ToListAsync();
            foreach(var story in stories)
            {
                await _cache.AddStoryAsync(story.StoryId, story);
            }
            return _msgService.MsgReturn(0, "Kết quả tìm kiếm", stories);
        }

        [HttpGet("search_global")]
        public async Task<ActionResult> SearchGlobal(string? search, int? authorId, int? from, int? to, int? status, [FromQuery] List<int> cates)
        {
            if (search != null)
            {
                search = search.ToLower();
            }

            var stories = await _context.Stories
                .Where(s => s.Status > 0 && (search == null || s.StoryTitle.ToLower().Contains(search))
                && (!authorId.HasValue || s.AuthorId == authorId)
                && (!status.HasValue || s.Status == status) && (!from.HasValue || s.StoryPrice >= from) && (!to.HasValue || s.StoryPrice <= to))
                .Include(s => s.Author)
                .Include(s => s.Categories)
                .Include(s => s.StoryInteraction)
                .Select(s => new
                {
                    StoryId = s.StoryId,
                    StoryTitle = s.StoryTitle,
                    StoryImage = s.StoryImage,
                    StoryDescription = s.StoryDescription,
                    StoryCategories = s.Categories.ToList(),
                    StoryAuthor = new { s.Author.UserId, s.Author.UserFullname },
                    StoryCreateTime = s.CreateTime,
                    StoryPrice = s.StoryPrice,
                    Status = s.Status,
                    StoryInteraction = new
                    {
                        s.StoryInteraction.Like,
                        s.StoryInteraction.Follow,
                        s.StoryInteraction.View,
                        s.StoryInteraction.Read,
                    },
                }).OrderByDescending(s => s.StoryInteraction.Read)
                .ToListAsync();

            stories = cates == null || cates.Count() == 0 ? stories :
                stories.Where(c => cates.All(categoryId => c.StoryCategories.Any(sc => sc.CategoryId == categoryId))).ToList();
            return _msgService.MsgReturn(0, "Kết quả tìm kiếm", stories);
        }

        [Authorize]
        [HttpGet("story_information")]
        public async Task<ActionResult> GetStoryInfor(int storyId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var story = await _context.Stories.Where(s => s.StoryId == storyId && s.AuthorId == userId)
                .Select(s => new
                {
                    storyId = s.StoryId,
                    storyTitle = s.StoryTitle,
                    storyDescription = s.StoryDescription,
                    storyDescriptionMarkdown = s.StoryDescriptionMarkdown,
                    StoryDescriptionHtml = s.StoryDescriptionHtml,
                    storyCategories = s.Categories.ToList(),
                    storyImage = s.StoryImage,
                    storyPrice = s.StoryPrice,
                    storySale = s.StorySale,
                    status = s.Status,
                    reviewed = s.Chapters.Any(c => c.Status == 1)
                }).FirstOrDefaultAsync();
            if (story == null)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Bạn không thể truy cập trang này"
                });
            }
            return _msgService.MsgReturn(0, "Story Detail", story);
        }

        [Authorize]
        [HttpPut("upload_image")]
        public IActionResult GetImage([FromForm] GetStoryImageDto data)
        {
            string fileUploaded = "";
            try
            {
                if (data.image.Length > 0)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Assets/images/story");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    if(data.previousImage != null)
                    {
                        string oldFilePath = Path.Combine(path, data.previousImage);

                        // Check if the previous image file exists and delete it
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    var ext = Path.GetExtension(data.image.FileName);
                    var name = Path.GetFileNameWithoutExtension(data.image.FileName);
                    var fileName = name + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ext;
                    string filePath = Path.Combine(path, fileName);
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        data.image.CopyTo(stream);
                    }
                    fileUploaded = fileName;
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
            return new JsonResult(new
            {
                EC = 0,
                EM = "Upload ảnh thành công",
                DT = new
                {
                    fileUploaded = fileUploaded
                }
            });
        }

        [HttpPost("save_story")]
        public async Task<ActionResult> SaveStory(AddStoryDto addStoryForm)
        {
            try
            {
                _context.Stories.Add(new Story
                {
                    StoryTitle = addStoryForm.StoryTitle,
                    AuthorId = addStoryForm.AuthorId,
                    StoryDescription = addStoryForm.StoryDescription,
                    StoryDescriptionHtml = addStoryForm.StoryDescriptionHtml,
                    StoryDescriptionMarkdown = addStoryForm.StoryDescriptionMarkdown,
                    StoryImage = addStoryForm.StoryImage != null ? addStoryForm.StoryImage : null,
                    CreateTime = DateTime.Now,
                    Status = 0,
                    StoryPrice = 0,
                    Categories = await _context.Categories.Where(c => addStoryForm.CategoryIds.Contains(c.CategoryId)).ToListAsync()
                });
                _context.SaveChanges();
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
                EM = "Lưu truyện thành công!"
            });
        }

        [Authorize]
        [HttpPut("update_story")]
        public async Task<ActionResult> EditStory(UpdateStoryDto story)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var currentStory = _context.Stories.Include(s => s.Categories).FirstOrDefault(s => s.StoryId == story.StoryId && s.AuthorId == userId);
            if (currentStory == null)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Bạn không có quyền truy cập trang này"
                });
            }
            try
            {
                if (currentStory != null)
                {
                    currentStory.StoryDescription = story.StoryDescription;
                    currentStory.StoryTitle = story.StoryTitle;
                    currentStory.StoryDescriptionHtml = story.StoryDescriptionHtml;
                    currentStory.StoryDescriptionMarkdown = story.StoryDescriptionMarkdown;
                    currentStory.UpdateTime = DateTime.Now;
                    currentStory.Status = story.Status;
                    currentStory.StoryPrice = story.StoryPrice;
                    currentStory.StorySale = story.StorySale;
                    if (story.StoryImage != null)
                    {
                        currentStory.StoryImage = story.StoryImage;
                    }
                    var existingCategories = currentStory.Categories.Select(c => c.CategoryId).ToList();

                    var categoriesToAdd = story.CategoryIds.Except(existingCategories).ToList();
                    var categoriesToRemove = existingCategories.Except(story.CategoryIds).ToList();

                    foreach (var categoryId in categoriesToAdd)
                    {
                        var category = await _context.Categories.FindAsync(categoryId);
                        if (category != null)
                        {
                            currentStory.Categories.Add(category);
                        }
                    }

                    // Remove existing categories from the story
                    foreach (var categoryId in categoriesToRemove)
                    {
                        var categoryToRemove = currentStory.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
                        if (categoryToRemove != null)
                        {
                            currentStory.Categories.Remove(categoryToRemove);
                        }
                    }

                }
                _context.Entry<Story>(currentStory).State = EntityState.Modified;
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
                EM = "Cập nhật truyện thành công!"
            });
        }

        [HttpGet("getAuthorAndStoryNumber")]
        public async Task<ActionResult> getAuthorAndStoryNumber()
        {
            try
            {
                var authorNumber = await _context.Stories.Select(s => s.AuthorId).Distinct().CountAsync();
                var storyNumber = await _context.Stories.CountAsync();
                return new JsonResult(new
                {
                    EC = 0,
                    EM = "Số truyện và tác giá",
                    DT = new { authorNumber = authorNumber, storyNumber = storyNumber }
                });
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
        [HttpPut("delete_story")]
        public async Task<ActionResult> DeleteStory(int storyId)
        {
            int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            var currentStory = _context.Stories.FirstOrDefault(s => s.StoryId == storyId && (s.AuthorId == userId || user.RoleId == 1));

            if (currentStory == null)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Bạn không có quyền dùng chức năng này"
                });
            }

            if(currentStory.Status == -1)
            {
                return new JsonResult(new
                {
                    EC = -1,
                    EM = "Truyện này đã bị khóa"
                });
            }
            try
            {
                currentStory.Status = -1;
                _context.Entry<Story>(currentStory).State = EntityState.Modified;
                _context.SaveChanges();
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
                EM = "Xóa truyện thành công"
            });
        }

        [HttpPut("update_storyimage")]
        public IActionResult ChangeAvatar([FromForm] StoryImageDto data)
        {
            string fileUploaded = "";
            try
            {
                int userId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                var story = _context.Stories.Include(s => s.Categories).FirstOrDefault(s => s.StoryId == data.storyId && s.AuthorId == userId);
                if (story == null)
                {
                    return new JsonResult(new
                    {
                        EC = 1,
                        EM = "Không thể truy cập truyện này"
                    });
                }
                if (data.image.Length > 0)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Assets/images/story");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    var ext = Path.GetExtension(data.image.FileName);
                    var name = Path.GetFileNameWithoutExtension(data.image.FileName);
                    var fileName = name + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ext;
                    string filePath = Path.Combine(path, fileName);
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        data.image.CopyTo(stream);
                    }
                    story.StoryImage = fileName;
                    fileUploaded = story.StoryImage;
                    _context.SaveChanges();
                }
                else
                {
                    return new JsonResult(new
                    {
                        EC = 2,
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
            return new JsonResult(new
            {
                EC = 0,
                EM = "Cập nhật ảnh truyện thành công",
                DT = new
                {
                    fileUploaded = fileUploaded
                }
            });
        }
    }
}
