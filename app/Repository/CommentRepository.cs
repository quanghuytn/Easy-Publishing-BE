using app.DTOs.Comment;
using app.Interface;
using app.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace app.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly EasyPublishingContext _context;

        public CommentRepository(EasyPublishingContext context)
        {
            _context = context;
        }

        public async Task AddComment(int userId, SendCommentDto newComment)
        {
            try
            {
                Comment cmt = new Comment()
                {
                    UserId = userId,
                    StoryId = newComment.StoryId,
                    ChapterId = newComment.ChapterId,
                    CommentContent = newComment.CommentContent,
                    CommentDate = DateTime.Now,
                };
                await _context.Comments.AddAsync(cmt);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteComment(int commentId)
        {
            try
            {
                var comment = await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);
                if (comment == null)
                {
                    return false;
                }
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }

        public async Task<List<CommentDto>> GetChapterComments(int userId, int chapterId)
        {
            var comments = await _context.Comments.Where(c => c.StoryId == chapterId)
               .Include(c => c.User)
               .Select(c => new CommentDto
               {
                   UserComment = new UserComment{ UserId = c.User.UserId, UserFullname = c.User.UserFullname, UserImage = c.User.UserImage },
                   CommentId = c.CommentId,
                   CommentContent = c.CommentContent,
                   CommentDate = c.CommentDate,
                   CommentWriter = userId == c.UserId ? true : false
               })
               .OrderByDescending(c => c.CommentId)
               .ToListAsync();
            return comments;
        }

        public async Task<List<CommentDto>> GetStoryComments(int userId, int storyId)
        {
            var comments = await _context.Comments.Where(c => c.StoryId == storyId)
                .Include(c => c.User)
                .Select(c => new CommentDto
                {
                    UserComment = new UserComment { UserId = c.User.UserId, UserFullname = c.User.UserFullname, UserImage = c.User.UserImage },
                    CommentId = c.CommentId,
                    CommentContent = c.CommentContent,
                    CommentDate = c.CommentDate,
                    CommentWriter = userId == c.UserId ? true : false
                })
                .OrderByDescending(c => c.CommentId)
                .ToListAsync();
            return comments;
        }

        public async Task<bool> UpdateComment(int userId, int commentId, string? commentContent)
        {
            Comment comment = await _context.Comments.FirstOrDefaultAsync(c => c.UserId == userId && c.CommentId == commentId);
            if (comment == null) return false;
            try
            {
                if (String.IsNullOrEmpty(commentContent)) _context.Comments.Remove(comment);
                else
                {
                    comment.CommentContent = commentContent;
                }
                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }
    }
}
