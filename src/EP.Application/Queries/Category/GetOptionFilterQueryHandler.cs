using EP.Application.Common;
using EP.Application.Common.DTOs.Category;
using EP.Application.Common.DTOs.Common;
using EP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace EP.Application.Queries.Category
{
    public record GetOptionFilterQuery : IRequest<ApiResponse<OptionFilterDto>>;

    public class GetOptionFilterQueryHandler : IRequestHandler<GetOptionFilterQuery, ApiResponse<OptionFilterDto>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStoryRepository _storyRepository;
        public GetOptionFilterQueryHandler(ICategoryRepository categoryRepository, IStoryRepository storyRepository)
        {
            _categoryRepository = categoryRepository;
            _storyRepository = storyRepository;
        }
        public async Task<ApiResponse<OptionFilterDto>> Handle(GetOptionFilterQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.SelectAsync(c => new CategoryDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategoryDescription = c.CategoryDescription
            });

            var prices = await _storyRepository.SelectAsync(s => s.StoryPrice);
            decimal from = 0, to = 0;
            if (prices.Any())
            {
                from = prices.Min();
                to = prices.Max();
            }

            var status = new List<StatusDto>
                {
                    new("Hoàn thành", 2),
                    new("Chưa hoàn thành", 1)
                };

            return ApiResponse<OptionFilterDto>.Success("Trường tìm kiếm", 
                new OptionFilterDto
                    {
                        Categories = categories,
                        From = from,
                        To = to,
                        Status = status
                    });
        }
    }
}
