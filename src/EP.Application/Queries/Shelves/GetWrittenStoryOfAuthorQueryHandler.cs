using EP.Application.Common;
using EP.Application.Common.DTOs.Shelves;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Shelves
{
    public record GetWrittenStoryOfAuthorQuery(int authorId) : IRequest<ApiResponse<IEnumerable<TopStoryDto>>>;
    public class GetWrittenStoryOfAuthorQueryHandler : IRequestHandler<GetWrittenStoryOfAuthorQuery, ApiResponse<IEnumerable<TopStoryDto>>>
    {
        private readonly IShelvesRepository _shelvesRepository;
        public GetWrittenStoryOfAuthorQueryHandler(IShelvesRepository shelvesRepository)
        {
            _shelvesRepository = shelvesRepository;
        }
        public async Task<ApiResponse<IEnumerable<TopStoryDto>>> Handle(GetWrittenStoryOfAuthorQuery request, CancellationToken cancellationToken)
        {
            var stories = await _shelvesRepository.GetWrittenStoryOfAuthor(request.authorId);

            return ApiResponse<IEnumerable<TopStoryDto>>.Success("Get written stories of author successfully", stories);
        }
    }
}
