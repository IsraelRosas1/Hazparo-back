using AutoMapper;
using Hazparo.Application.Professional.Commands.BecomeProfessional;
using Hazparo.Application.Professional.Dtos;
using Hazparo.Domain.Entities;

namespace Hazparo.Application.Professional.Mappings;

public sealed class ProfessionalLocationMappingProfile
    : Profile
{
    public ProfessionalLocationMappingProfile()
    {
        CreateMap<ProfessionalLocationDto,UpdateProfessionalLocationCommand>();

        CreateMap<UpdateProfessionalLocationCommand,ProfessionalLocation>()
            .ForMember(
                destination => destination.ProfessionalId,
                options => options.Ignore())
            .ForMember(
                destination => destination.Location,
                options => options.Ignore())
            .ForMember(
                destination => destination.UpdatedAt,
                options => options.Ignore())
            .ForMember(
                destination => destination.Professional,
                options => options.Ignore());

        CreateMap<BecomeProfessionalCommand,ProfessionalLocation>()
            .ForMember(
                destination => destination.ProfessionalId,
                options => options.Ignore())
            .ForMember(
                destination => destination.Location,
                options => options.Ignore())
            .ForMember(
                destination => destination.UpdatedAt,
                options => options.Ignore())
            .ForMember(
                destination => destination.Professional,
                options => options.Ignore());

        CreateMap<ProfessionalLocation,ProfessionalLocationDto>();
    }
}