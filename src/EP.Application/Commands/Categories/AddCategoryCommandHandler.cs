using EP.Application.Common.Interfaces;
using EP.Domain.Models;
using FluentValidation;
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

    public class AddCategoryCommandValidator : AbstractValidator<AddCategoryCommand>
    {
        public AddCategoryCommandValidator()
        {
            RuleFor(command => command.CategoryName)
                .NotEmpty().WithMessage("CategoryName is required.")
                .MaximumLength(100).WithMessage("CategoryName must not exceed 100 characters.");

            RuleFor(command => command.CategoryDescription)
                .MaximumLength(1000).WithMessage("CategoryDescription must not exceed 1000 characters.")
                .When(command => command.CategoryDescription != null);
        }
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
