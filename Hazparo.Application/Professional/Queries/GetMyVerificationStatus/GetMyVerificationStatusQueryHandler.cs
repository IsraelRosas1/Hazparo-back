using AutoMapper;
using Hazparo.Application.Professional.Dtos;
using Hazparo.Application.User;
using Hazparo.Domain.Repositories;
using MediatR;

namespace Hazparo.Application.Professional.Queries
    .GetMyVerificationStatus;

public sealed class GetMyVerificationStatusQueryHandler(IUserContext userContext,
    IProfessionalProfileRepository professionalProfileRepository,
    IMapper mapper)
    : IRequestHandler<GetMyVerificationStatusQuery,ProfessionalVerificationStatusDto?>
{
    public async Task<ProfessionalVerificationStatusDto?> Handle(GetMyVerificationStatusQuery request,CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (currentUser is null ||
            !Guid.TryParse(
                currentUser.Id,
                out var professionalId))
        {
            return null;
        }

        var profile =await professionalProfileRepository.GetByUserIdAsync(professionalId,cancellationToken);

        if (profile is null)
        {
            return null;
        }

        return mapper.Map<ProfessionalVerificationStatusDto>(profile);
    }
}