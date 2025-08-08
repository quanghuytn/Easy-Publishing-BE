using EP.Application.Common;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Shelves
{
    public record GetTop6StoriesSaleQuery() : IRequest<ApiResponse<IEnumerable<TopSaleDto>>>;
    public class GetTop6StoriesSaleQueryHandler : IRequestHandler<GetTop6StoriesSaleQuery, ApiResponse<IEnumerable<TopSaleDto>>>
    {
        private readonly IShelvesRepository _shelvesRepository;
        public GetTop6StoriesSaleQueryHandler(IShelvesRepository shelvesRepository)
        {
            _shelvesRepository = shelvesRepository;
        }
        public async Task<ApiResponse<IEnumerable<TopSaleDto>>> Handle(GetTop6StoriesSaleQuery request, CancellationToken cancellationToken)
        {
            var topStories = await _shelvesRepository.GetTop6StoriesSale();

            return ApiResponse<IEnumerable<TopSaleDto>>.Success("Top 6 truyện doanh thu cao nhất", topStories);
        }
    }
}
