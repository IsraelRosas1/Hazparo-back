using Hazparo.Application.Professional.Dtos;
using MediatR;

namespace Hazparo.Application.Professional.Queries
    .GetMyProfessionalProfile;

public sealed class GetMyProfessionalProfileQuery: IRequest<ProfessionalProfileDetailsDto?>
{
}