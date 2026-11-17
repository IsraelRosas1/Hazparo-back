using Hazparo.Application.Professional.Dtos;
using MediatR;

namespace Hazparo.Application.Professional.Queries.GetMyVerificationStatus;

public class GetMyVerificationStatusQuery: IRequest<ProfessionalVerificationStatusDto?>
{
}