using EP.Application.Common;
using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EP.Application.Commands.Stories
{
    public record EditStoryCommand : IRequest<ApiResponse<string>>
    {
        public int StoryId { get; set; }
        public int UserId { get; set; }
        public string StoryTitle { get; set; } = null!;
        public decimal StoryPrice { get; set; }
        public decimal? StorySale { get; set; }
        public string? StoryImage { get; set; }
        public string? StoryDescription { get; set; }
        public string? StoryDescriptionMarkdown { get; set; }
        public string? StoryDescriptionHtml { get; set; }
        public int Status { get; set; }
        public List<int> CategoryIds { get; set; }
    }
    public class EditStoryCommandHandler : IRequestHandler<EditStoryCommand, ApiResponse<string>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public EditStoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<string>> Handle(EditStoryCommand request, CancellationToken cancellationToken)
        {
            var currentStory = await _unitOfWork.StoryRepository.GetStoryWithCategory(request.StoryId, request.UserId);
            if (currentStory == null)
            {
                return ApiResponse<string>.Failure("Truyện không tồn tại hoặc bạn không có quyền chỉnh sửa.");
            }

            currentStory.StoryDescription = request.StoryDescription;
            currentStory.StoryTitle = request.StoryTitle;
            currentStory.StoryDescriptionHtml = request.StoryDescriptionHtml;
            currentStory.StoryDescriptionMarkdown = request.StoryDescriptionMarkdown;
            currentStory.UpdateTime = DateTime.Now;
            currentStory.Status = request.Status;
            currentStory.StoryPrice = request.StoryPrice;
            currentStory.StorySale = request.StorySale;

            if (request.StoryImage != null)
            {
                currentStory.StoryImage = request.StoryImage;
            }

            var existingCategories = currentStory.Categories.Select(c => c.CategoryId).ToList();
            var newCategoryIds = request.CategoryIds ?? new List<int>();

            var categoriesToAdd = newCategoryIds.Except(existingCategories).ToList();
            var categoriesToRemove = existingCategories.Except(newCategoryIds).ToList();

            foreach (var categoryId in categoriesToAdd)
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
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

            await _unitOfWork.StoryRepository.UpdateAsync(currentStory);
            var affectedRows = await _unitOfWork.CompleteAsync();
            if (affectedRows > 0)
            {
                return ApiResponse<string>.Success("Cập nhật truyện thành công.");
            }
            else
            {
                return ApiResponse<string>.Failure("Cập nhật thất bại.");
            }
        }
    }
}
