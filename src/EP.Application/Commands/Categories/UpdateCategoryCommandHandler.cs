using EP.Application.Common.Interfaces;
using FluentValidation;
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

    public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
    {
        public UpdateCategoryCommandValidator()
        {
            RuleFor(command => command.CategoryId)
                .GreaterThan(0).WithMessage("CategoryId không hợp lệ!");

            RuleFor(command => command.CategoryName)
                .NotEmpty().WithMessage("CategoryName is required.")
                .MaximumLength(100).WithMessage("CategoryName must not exceed 100 characters.");

            RuleFor(command => command.CategoryDescription)
                .NotEmpty().WithMessage("Miêu tả không được để trống!.")
                .MaximumLength(1000).WithMessage("CategoryDescription must not exceed 1000 characters.");
        }
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
