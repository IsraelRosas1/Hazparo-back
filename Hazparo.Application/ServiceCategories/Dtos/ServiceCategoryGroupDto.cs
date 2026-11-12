namespace Hazparo.Application.ServiceCategories.Dtos;

public sealed class ServiceCategoryGroupDto
{
    public string GroupName { get; set; } = string.Empty;

    public List<ServiceCategoryDto> Categories { get; set; } = [];
}