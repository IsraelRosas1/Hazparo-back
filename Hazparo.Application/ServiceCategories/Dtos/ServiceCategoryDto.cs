namespace Hazparo.Application.ServiceCategories.Dtos;

public sealed class ServiceCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? IconKey { get; set; }
}