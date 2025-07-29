using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using MediatR;

namespace EP.Application.Commands.Categories
{
    public record AddCategoryCommand : IRequest<bool>
    {
        public string? CategoryName { get; set; }
        public string? CategoryBanner { get; set; }
        public string? CategoryDescription { get; set; }
    }
    public class AddCategoryCommandHandler : IRequestHandler<AddCategoryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new Category
            {
                CategoryName = request.CategoryName,
                CategoryBanner = request.CategoryBanner,
                CategoryDescription = request.CategoryDescription
            };

            await _unitOfWork.CategoryRepository.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
