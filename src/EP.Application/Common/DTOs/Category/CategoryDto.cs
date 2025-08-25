namespace EP.Application.Common.DTOs.Category;

public partial class CategoryDto
{
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? CategoryDescription { get; set; }
    public string? CategoryBanner { get; set; }
    public int StoriesNumber { get; set; }
}
