using EP.Application.Common;
using EP.Application.Common.DTOs.Author;
using EP.Application.Common.DTOs.Category;
using EP.Application.Common.DTOs.Common;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Common
{
    public record GetSearchGlobalOptionQuery : IRequest<ApiResponse<SearchGlobalFilterDto>>;
    public class GetSearchGlobalOptionQueryHandler : IRequestHandler<GetSearchGlobalOptionQuery, ApiResponse<SearchGlobalFilterDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStoryRepository _storyRepository;
        public GetSearchGlobalOptionQueryHandler(IUserRepository userRepository, ICategoryRepository categoryRepository, IStoryRepository storyRepository)
        {
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _storyRepository = storyRepository;
        }
        public async Task<ApiResponse<SearchGlobalFilterDto>> Handle(GetSearchGlobalOptionQuery request, CancellationToken cancellationToken)
        {
            var author = await _userRepository
                .SelectWithConditionAsync(u => u.Stories.Any(), u => new AuthorCardDto
                {
                    AuthorId = u.UserId,
                    AuthorName = u.UserFullname,
                    AuthorImage = u.UserImage,
                });
            var categories = await _categoryRepository
                .SelectAsync(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryDescription = c.CategoryDescription
                });

            var from = await _storyRepository.MinAsync(s => s.StoryPrice);
            var to = await _storyRepository.MaxAsync(s => s.StoryPrice);

            var status = new List<StatusDto>
            {
                new StatusDto("Hoàn thành", 2),
                new StatusDto("Chưa hoàn thành", 1)
            };

            return ApiResponse<SearchGlobalFilterDto>.Success("Lấy thông tin tìm kiếm toàn cục thành công", new SearchGlobalFilterDto
            {
                Author = author,
                From = from,
                To = to,
                Categories = categories,
                Status = status
            });
        }
    }
}
