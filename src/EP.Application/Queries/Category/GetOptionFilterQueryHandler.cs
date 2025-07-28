using EP.Application.Common.DTOs.Category;
using EP.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Queries.Category
{
    public record GetOptionFilterQuery : IRequest<OptionFilterDto>;

    public class GetOptionFilterQueryHandler : IRequestHandler<GetOptionFilterQuery, OptionFilterDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        public GetOptionFilterQueryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<OptionFilterDto> Handle(GetOptionFilterQuery request, CancellationToken cancellationToken)
        {
            return await _categoryRepository.GetOptionFilter();
        }
    }
}
