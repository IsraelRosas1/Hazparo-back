using AutoMapper;
using Hazparo.Application.Professional.Dtos;
using Hazparo.Application.User;
using Hazparo.Domain.Repositories;
using MediatR;

public sealed class GetMyProfessionalLocationQueryHandler(IUserContext userContext,
    IProfessionalProfileRepository professionalProfileRepository,
    IMapper mapper): IRequestHandler<GetMyProfessionalLocationQuery,ProfessionalLocationDto?>
{
    public async Task<ProfessionalLocationDto?> Handle(GetMyProfessionalLocationQuery request,CancellationToken cancellationToken)
    {
        var currentUser = userContext.GetCurrentUser();

        if (currentUser is null ||
            !Guid.TryParse(
                currentUser.Id,
                out var professionalId))
        {
            return null;
        }

        var profile =
            await professionalProfileRepository.GetByUserIdAsync(
                professionalId,
                cancellationToken);

        if (profile?.Location is null)
        {
            return null;
        }

        return mapper.Map<ProfessionalLocationDto>(profile.Location);
    }
}