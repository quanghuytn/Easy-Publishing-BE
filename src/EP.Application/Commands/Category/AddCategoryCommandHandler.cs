using EP.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Commands.Category
{
    public record AddCategoryCommand : IRequest<bool>
    {
        public string? CategoryName { get; set; }
        public string? CategoryBanner { get; set; }
        public string? CategoryDescription { get; set; }
    }
    public class AddCategoryCommandHandler: IRequestHandler<AddCategoryCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        public AddCategoryCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Category.AddCategory(request);
            await _unitOfWork.CompleteAsync();

            return result;
        }
    }
}
