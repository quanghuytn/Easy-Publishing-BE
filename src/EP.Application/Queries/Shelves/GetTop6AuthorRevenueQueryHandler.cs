using EP.Application.Common;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Shelves
{
    public record GetTop6AuthorRevenueQuery() : IRequest<ApiResponse<IEnumerable<TopAuthorRevenueDto>>>;
    public class GetTop6AuthorRevenueQueryHandler : IRequestHandler<GetTop6AuthorRevenueQuery, ApiResponse<IEnumerable<TopAuthorRevenueDto>>>
    {
        private readonly IShelvesRepository _shelvesRepository;
        public GetTop6AuthorRevenueQueryHandler(IShelvesRepository shelvesRepository)
        {
            _shelvesRepository = shelvesRepository;
        }
        public async Task<ApiResponse<IEnumerable<TopAuthorRevenueDto>>> Handle(GetTop6AuthorRevenueQuery request, CancellationToken cancellationToken)
        {
            var authors = await _shelvesRepository.GetTop6AuthorRevenue();

            return ApiResponse<IEnumerable<TopAuthorRevenueDto>>.Success("Top 6 tác giả kiếm được nhiều tiền nhất", authors);
        }
    }
}
