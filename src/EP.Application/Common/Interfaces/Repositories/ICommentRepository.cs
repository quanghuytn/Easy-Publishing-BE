using EP.Application.Common.DTOs.Comment;
using EP.Application.Common.Pagination;
using EP.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Common.Interfaces.Repositories
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<PaginatedResult<CommentDto>> GetStoryComments(int userId, int storyId, int page, int pageSize);
    }
}
