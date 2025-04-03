﻿using app.Models;
using app.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;

namespace app.Controllers
{
    [Route("api/v1/authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly EasyPublishingContext _context;
        private MsgService _msgService = new MsgService();
        private int pageSize = 10;
        public AuthorsController(EasyPublishingContext context)
        {
            _context = context;
        }

        [HttpGet("story_detail")]
        public async Task<ActionResult> GetStoryRelateAuthor(int storyId)
        {
            var story = await _context.Stories.FirstOrDefaultAsync(c => c.StoryId == storyId);
            var author = await _context.Users.Where(c => c.UserId == story.AuthorId)
                .Include(c => c.Stories).ThenInclude(c=>c.StoryInteraction)
                .Select(c => new
                {
                    AuthorId = c.UserId,
                    AuthorName = c.UserFullname,
                    AuthorImage = c.UserImage,
                    AuthorStories = c.Stories.Count,
                    Like = c.Stories.Select(c=>c.StoryInteraction.Like).Sum(),
                    Read = c.Stories.Select(c=>c.StoryInteraction.Read).Sum(),
                    AuthorNewestStory = c.Stories.Where(c => c.AuthorId == story.AuthorId).OrderByDescending(c => c.StoryId)
                    .Select(s => new { s.StoryId, s.StoryTitle, s.StoryImage, s.StoryDescription, s.CreateTime })
                    .FirstOrDefault()
                })
                .ToListAsync();
            return _msgService.MsgReturn(0, "Tác giả liên quan", author.FirstOrDefault());
        }

        [HttpGet("author_detail")]
        public async Task<ActionResult> GetAuthor(int authorId)
        {
            var author = await _context.Users.Where(c => c.UserId == authorId)
                .Include(c => c.Stories)
                .Select(c => new
                {
                    AuthorId = c.UserId,
                    AuthorName = c.UserFullname,
                    AuthorImage = c.UserImage,
                    AuthorEmail = c.Email,
                    AuthorDescriptionHtml = c.DescriptionHtml,
                    AuthorDescriptionMarkdown = c.DescriptionMarkdown,
                    AuthorStories = c.Stories.Count,
                })
                .ToListAsync();
            return _msgService.MsgReturn(0, "Thông tin tác giả", author.FirstOrDefault());
        }
    }
}
