using Azure;
using EP.Application.Common.DTOs.Author;
using EP.Application.Common.DTOs.Comment;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces.Repositories;
using EP.Application.Common.Pagination;
using EP.Domain.Models;
using EP.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Infrastructure.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(Context context) : base(context)
        {
        }

        public async Task<PaginatedResult<CommentDto>> GetStoryComments(int userId, int storyId, int page, int pageSize)
        {
            var baseQuery = _dbSet
                .AsNoTracking()
                .Where(c => c.StoryId == storyId);

            int totalCount = await baseQuery.CountAsync();

            var comments = await baseQuery
                .Include(c => c.User)
                .OrderByDescending(c => c.CommentId)
                .Skip(pageSize * (page - 1))
                .Take(pageSize)
                .Select(c => new CommentDto
                {
                    UserComment = new UserComment { UserId = c.User.UserId, UserFullname = c.User.UserFullname, UserImage = c.User.UserImage },
                    CommentId = c.CommentId,
                    CommentContent = c.CommentContent,
                    CommentDate = c.CommentDate,
                    CommentWriter = userId == c.UserId ? true : false
                })
                .ToListAsync();

            return new PaginatedResult<CommentDto>(pageIndex: page, pageSize: pageSize, totalCount: totalCount, data: comments);
        }
    }
}
