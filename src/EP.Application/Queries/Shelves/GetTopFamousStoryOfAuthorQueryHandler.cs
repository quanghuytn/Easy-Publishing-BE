using EP.Application.Common;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EP.Application.Queries.Shelves
{
    public record GetTopFamousStoryOfAuthorQuery(int authorId) : IRequest<ApiResponse<IEnumerable<TopStoryDto>>>;
    public class GetTopFamousStoryOfAuthorQueryHandler : IRequestHandler<GetTopFamousStoryOfAuthorQuery, ApiResponse<IEnumerable<TopStoryDto>>>
    {
        private readonly IShelvesRepository _shelvesRepository;
        public GetTopFamousStoryOfAuthorQueryHandler(IShelvesRepository shelvesRepository)
        {
            _shelvesRepository = shelvesRepository;
        }
        public async Task<ApiResponse<IEnumerable<TopStoryDto>>> Handle(GetTopFamousStoryOfAuthorQuery request, CancellationToken cancellationToken)
        {
            var stories = await _shelvesRepository.GetTopFamousStoryOfAuthor(request.authorId);

            return ApiResponse<IEnumerable<TopStoryDto>>.Success("Get top famous stories of author successfully", stories);
        }
    }
}
