using EP.Application.Common;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Shelves
{
    public record GetNewestStoryOfAuthorQuery(int authorId) : IRequest<ApiResponse<IEnumerable<TopStoryDto>>>;
    public class GetNewestStoryOfAuthorQueryHandler : IRequestHandler<GetNewestStoryOfAuthorQuery, ApiResponse<IEnumerable<TopStoryDto>>>
    {
        private readonly IShelvesRepository _shelvesRepository;
        public GetNewestStoryOfAuthorQueryHandler(IShelvesRepository shelvesRepository)
        {
            _shelvesRepository = shelvesRepository;
        }
        public async Task<ApiResponse<IEnumerable<TopStoryDto>>> Handle(GetNewestStoryOfAuthorQuery request, CancellationToken cancellationToken)
        {
            var stories = await _shelvesRepository.GetNewestStoryOfAuthor(request.authorId);

            return ApiResponse<IEnumerable<TopStoryDto>>.Success("Get newest stories of author successfully", stories);
        }
    }
}
