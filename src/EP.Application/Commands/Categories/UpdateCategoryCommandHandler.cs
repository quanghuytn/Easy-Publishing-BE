using EP.Application.Common.Interfaces;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EP.Application.Commands.Categories
{
    public record UpdateCategoryCommand : IRequest<int>
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryBanner { get; set; }
        public string? CategoryDescription { get; set; }
    }
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<int> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.CategoryName))
            {
                throw new ArgumentException("Tên thể loại không được để trống!", nameof(request.CategoryName));
            }
            var category = await _unitOfWork.CategoryRepository.GetByIdAsync(request.CategoryId);

            if (category == null)
            {
                throw new Exception("Category not found!");
            }

            category.CategoryName = request.CategoryName;
            category.CategoryBanner = request.CategoryBanner;   
            category.CategoryDescription = request.CategoryDescription;
            await _unitOfWork.CategoryRepository.UpdateAsync(category);

            return await _unitOfWork.CompleteAsync();
        }
    }
}
