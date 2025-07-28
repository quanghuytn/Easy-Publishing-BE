namespace app.DTOs.Category
{
    public class OptionFilterDto
    {
        public decimal To { get; set; }
        public decimal From { get; set; }
        public required List<CategoryDto> Categories { get; set; }
    }
}
