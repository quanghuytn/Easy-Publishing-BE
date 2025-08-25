using EP.Application.Common.DTOs.Author;
using EP.Application.Common.DTOs.Category;

namespace EP.Application.Common.DTOs.Common
{
    public class SearchGlobalFilterDto
    {
        public IEnumerable<AuthorCardDto> Author { get; set; }
        public decimal From { get; set; }
        public decimal To { get; set; }
        public required IEnumerable<CategoryDto> Categories { get; set; }
        public required List<StatusDto> Status { get; set; }
    }
}
