using EP.Application.Common;
using EP.Application.Common.DTOs.Author;
using EP.Application.Common.Interfaces;
using MediatR;

namespace EP.Application.Queries.Author
{
    public record GetStoryRelateAuthorQuery(int storyId) : IRequest<ApiResponse<StoryRelateAuthorDto>>;
    public class GetStoryRelateAuthorQueryHandler : IRequestHandler<GetStoryRelateAuthorQuery, ApiResponse<StoryRelateAuthorDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetStoryRelateAuthorQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<StoryRelateAuthorDto>> Handle(GetStoryRelateAuthorQuery request, CancellationToken cancellationToken)
        {
            var story = await _unitOfWork.StoryRepository.GetByIdAsync(request.storyId);
            if (story == null)
            {
                throw new ArgumentException("Hệ thống xảy ra lỗi. Vui lòng thử lại sau!");
            }

            var author = await _unitOfWork.AuthorRepository.GetStoryRelateAuthor(story.AuthorId);
            if (author == null)
            {
                return ApiResponse<StoryRelateAuthorDto>.Failure("Tác giả không tồn tại");
            }

            return ApiResponse<StoryRelateAuthorDto>.Success(author, "Thông tin tác giả liên quan");
        }
    }
}
