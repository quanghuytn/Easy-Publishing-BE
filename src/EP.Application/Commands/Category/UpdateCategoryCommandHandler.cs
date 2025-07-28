using EP.Application.Common.Interfaces;
using MediatR;

namespace EP.Application.Commands.Category
{
    public record UpdateCategoryCommand : IRequest<Unit>
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryBanner { get; set; }
        public string? CategoryDescription { get; set; }
    }
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _unitOfWork.Category.GetByIdAsync(request.CategoryId);

            if (category == null)
            {
                throw new Exception("Category not found!");
            }

            category.CategoryName = request.CategoryName;
            category.CategoryBanner = request.CategoryBanner;   
            category.CategoryDescription = request.CategoryDescription;
            await _unitOfWork.Category.UpdateCategory(category);
            await _unitOfWork.CompleteAsync();

            return Unit.Value;
        }
    }
}
