using EP.Application.Common;
using EP.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EP.Application.Commands.Stories
{
    public record AddStoryCommand : IRequest<ApiResponse<string>>
    {
        public string StoryTitle { get; set; } = null!;
        public int AuthorId { get; set; }
        public string? StoryDescription { get; set; }
        public string? StoryDescriptionMarkdown { get; set; }
        public string? StoryDescriptionHtml { get; set; }
        public string? StoryImage { get; set; }
        public List<int> CategoryIds { get; set; }
    }
    public class AddStoryCommandHandler : IRequestHandler<AddStoryCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddStoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<string>> Handle(AddStoryCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.StoryRepository.AddAsync(new Domain.Models.Story
            {
                StoryTitle = request.StoryTitle,
                AuthorId = request.AuthorId,
                StoryDescription = request.StoryDescription,
                StoryDescriptionHtml = request.StoryDescriptionHtml,
                StoryDescriptionMarkdown = request.StoryDescriptionMarkdown,
                StoryImage = request.StoryImage != null ? request.StoryImage : null,
                CreateTime = DateTime.Now,
                Status = 0,
                StoryPrice = 0,
                Categories = (await _unitOfWork.CategoryRepository.FindManyAsTrackingAsync(c => request.CategoryIds.Contains(c.CategoryId))).ToList()
            });

            var affectedRows = await _unitOfWork.CompleteAsync();
            if (affectedRows > 0)
            {
                return ApiResponse<string>.Success("Thêm truyện thành công!");
            }
            else
            {
                return ApiResponse<string>.Failure("Thêm truyện thất bại!. Vui lòng thử lại sau.");
            }
        }
    }
}
