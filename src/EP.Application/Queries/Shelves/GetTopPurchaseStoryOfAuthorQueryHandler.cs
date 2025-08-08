using EP.Application.Common;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Shelves
{
    public record GetTopPurchaseStoryOfAuthorQuery(int authorId) : IRequest<ApiResponse<IEnumerable<TopStoryDto>>>;
    public class GetTopPurchaseStoryOfAuthorQueryHandler : IRequestHandler<GetTopPurchaseStoryOfAuthorQuery, ApiResponse<IEnumerable<TopStoryDto>>>
    {
        private readonly IShelvesRepository _shelvesRepository;
        public GetTopPurchaseStoryOfAuthorQueryHandler(IShelvesRepository shelvesRepository)
        {
            _shelvesRepository = shelvesRepository;
        }
        public async Task<ApiResponse<IEnumerable<TopStoryDto>>> Handle(GetTopPurchaseStoryOfAuthorQuery request, CancellationToken cancellationToken)
        {
            var stories = await _shelvesRepository.GetTopPurchaseStoryOfAuthor(request.authorId);

            return ApiResponse<IEnumerable<TopStoryDto>>.Success("Get top purchase stories of author successfully", stories);
        }
    }
}
