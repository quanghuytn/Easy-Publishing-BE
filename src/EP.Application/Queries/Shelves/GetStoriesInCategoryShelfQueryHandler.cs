using EP.Application.Common;
using EP.Application.Common.DTOs.Category;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Queries.Shelves
{
    public record GetStoriesInCategoryShelfQuery() : IRequest<ApiResponse<IEnumerable<CategoryWithStoryDto>>>;
    public class GetStoriesInCategoryShelfQueryHandler : IRequestHandler<GetStoriesInCategoryShelfQuery, ApiResponse<IEnumerable<CategoryWithStoryDto>>>
    {
        private readonly IShelvesRepository _shelvesRepository;
        public GetStoriesInCategoryShelfQueryHandler(IShelvesRepository shelvesRepository)
        {
            _shelvesRepository = shelvesRepository;
        }
        public async Task<ApiResponse<IEnumerable<CategoryWithStoryDto>>> Handle(GetStoriesInCategoryShelfQuery request, CancellationToken cancellationToken)
        {
            var categories = await _shelvesRepository.GetStoriesInCategoryShelf();

            return ApiResponse<IEnumerable<CategoryWithStoryDto>>.Success("Truyện theo thể loại", categories);
        }
    }
}
