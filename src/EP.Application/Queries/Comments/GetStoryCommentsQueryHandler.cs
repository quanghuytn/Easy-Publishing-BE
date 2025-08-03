using EP.Application.Common;
using EP.Application.Common.DTOs.Comment;
using EP.Application.Common.Interfaces;
using EP.Application.Common.Pagination;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Queries.Comments
{
    public record GetStoryCommentsQuery : IRequest<ApiResponse<PaginatedResult<CommentDto>>>
    {
        public int StoryId { get; set; }
        public int UserId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = 10;
    }
    public class GetStoryCommentsQueryValidator : AbstractValidator<GetStoryCommentsQuery>
    {
        public GetStoryCommentsQueryValidator()
        {
            RuleFor(query => query.StoryId)
                .GreaterThan(0).WithMessage("StoryId không hợp lệ.");

            RuleFor(query => query.PageIndex)
                .GreaterThanOrEqualTo(0).WithMessage("PageIndex must be greater than or equal to 0.");

            RuleFor(query => query.PageSize)
                .GreaterThan(0).WithMessage("PageSize must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("PageSize must not exceed 100.");
        }
    }
    public class GetStoryCommentsQueryHandler : IRequestHandler<GetStoryCommentsQuery, ApiResponse<PaginatedResult<CommentDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetStoryCommentsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<ApiResponse<PaginatedResult<CommentDto>>> Handle(GetStoryCommentsQuery request, CancellationToken cancellationToken)
        {
            var data = await _unitOfWork.CommentRepository.GetStoryComments(request.UserId, request.StoryId, request.PageIndex, request.PageSize);

            return ApiResponse<PaginatedResult<CommentDto>>.Success("Bình luận của truyện", data);
        }
    }
}
