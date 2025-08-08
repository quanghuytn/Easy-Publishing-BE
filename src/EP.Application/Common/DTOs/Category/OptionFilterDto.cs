using EP.Application.Common.DTOs.Common;

namespace EP.Application.Common.DTOs.Category
{
    public class OptionFilterDto
    {
        public decimal To { get; set; }
        public decimal From { get; set; }
        public required IEnumerable<CategoryDto> Categories { get; set; }
        public required List<StatusDto> Status { get; set; }
    }
}
