using Hazparo.Application.ServiceCategories.Dtos;
using Hazparo.Domain.Repositories;
using MediatR;

namespace Hazparo.Application.ServiceCategories.Queries.GetServiceCategories;

public sealed class GetServiceCategoriesQueryHandler(
    IServiceCategoryRepository serviceCategoryRepository)
    : IRequestHandler<GetServiceCategoriesQuery,List<ServiceCategoryGroupDto>>
{
    public async Task<List<ServiceCategoryGroupDto>> Handle(GetServiceCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await serviceCategoryRepository.GetActiveAsync(
            cancellationToken);

        return categories.GroupBy(category => category.GroupName).Select(group => new ServiceCategoryGroupDto
            {
                GroupName = group.Key,
                Categories = group.Select(category =>
                    new ServiceCategoryDto
                    {
                        Id = category.Id,
                        Name = category.Name,
                        Slug = category.Slug,
                        IconKey = category.IconKey
                    }).ToList()
            }).ToList();
    }
}