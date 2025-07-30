using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EP.Application.Commands.Categories
{
    public record AddCategoryCommand : IRequest<int>
    {
        public string? CategoryName { get; set; }
        public string? CategoryBanner { get; set; }
        public string? CategoryDescription { get; set; }
    }
    public class AddCategoryCommandHandler : IRequestHandler<AddCategoryCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<int> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.CategoryName))
            {
                throw new ArgumentException("Tên thể loại không được để trống!", nameof(request.CategoryName));
            }

            var category = new Category
            {
                CategoryName = request.CategoryName,
                CategoryBanner = request.CategoryBanner,
                CategoryDescription = request.CategoryDescription
            };

            await _unitOfWork.CategoryRepository.AddAsync(category);
            return await _unitOfWork.CompleteAsync();
        }
    }
}
