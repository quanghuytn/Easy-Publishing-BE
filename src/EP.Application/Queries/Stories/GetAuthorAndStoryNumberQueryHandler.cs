using EP.Application.Common;
using EP.Application.Common.DTOs.Story;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Stories
{
    public record GetAuthorAndStoryNumberQuery() : IRequest<ApiResponse<AuthorAndStoryNumberDto>>;
    public class GetAuthorAndStoryNumberQueryHandler : IRequestHandler<GetAuthorAndStoryNumberQuery, ApiResponse<AuthorAndStoryNumberDto>>
    {
        private readonly IStoryRepository _storyRepository;
        public GetAuthorAndStoryNumberQueryHandler(IStoryRepository storyRepository)
        {
            _storyRepository = storyRepository;
        }

        public async Task<ApiResponse<AuthorAndStoryNumberDto>> Handle(GetAuthorAndStoryNumberQuery request, CancellationToken cancellationToken)
        {
            var data = await _storyRepository.GetAuthorAndStoryNumber();

            return ApiResponse<AuthorAndStoryNumberDto>.Success("Số truyện và tác giả", data);
        }
    }
}
