using Hazparo.Application.ServiceCategories.Dtos;
using MediatR;

namespace Hazparo.Application.ServiceCategories.Queries.GetServiceCategories;

public sealed class GetServiceCategoriesQuery
    : IRequest<List<ServiceCategoryGroupDto>>
{
}