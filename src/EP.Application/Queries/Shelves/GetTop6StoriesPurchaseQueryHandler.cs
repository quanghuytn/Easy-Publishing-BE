using EP.Application.Common;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Shelves
{
    public class GetTop6StoriesPurchaseQuery() : IRequest<ApiResponse<IEnumerable<TopStoryDto>>>;
    public class GetTop6StoriesPurchaseQueryHandler : IRequestHandler<GetTop6StoriesPurchaseQuery, ApiResponse<IEnumerable<TopStoryDto>>>
    {
        private readonly IShelvesRepository _shelvesRepository;
        public GetTop6StoriesPurchaseQueryHandler(IShelvesRepository shelvesRepository)
        {
            _shelvesRepository = shelvesRepository;
        }
        public async Task<ApiResponse<IEnumerable<TopStoryDto>>> Handle(GetTop6StoriesPurchaseQuery request, CancellationToken cancellationToken)
        {
            var stories = await _shelvesRepository.GetTop6StoriesPurchase();

            return ApiResponse<IEnumerable<TopStoryDto>>.Success("Get top 6 stories purchase successfully.", stories);
        }
    }
}
