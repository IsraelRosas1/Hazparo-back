using AutoMapper;
using Hazparo.Application.Professional.Dtos;
using Hazparo.Domain.Entities;

namespace Hazparo.Application.Professional.Dtos;

public sealed class ProfessionalProfileMappingProfile : Profile
{
    public ProfessionalProfileMappingProfile()
    {
        CreateMap<ProfessionalProfile, ProfessionalProfileDetailsDto>()
            .ForMember(
                destination => destination.Verification,
                options => options.MapFrom(
                    source => source.Verification.ToString()))

            .ForMember(
                destination => destination.IsVerificationComplete,
                options => options.MapFrom(
                    source =>
                        source.Verification ==
                        VerificationStatus.Verified))

            .ForMember(
                destination => destination.City,
                options => options.MapFrom(
                    source => source.Location == null
                        ? null
                        : source.Location.City))

            .ForMember(
                destination => destination.Latitude,
                options => options.MapFrom(
                    source => source.Location == null
                        ? null
                        : (decimal?)source.Location.Latitude))

            .ForMember(
                destination => destination.Longitude,
                options => options.MapFrom(
                    source => source.Location == null
                        ? null
                        : (decimal?)source.Location.Longitude))

            .ForMember(
                destination => destination.CategoryIds,
                options => options.MapFrom(
                    source => source.Services
                        .Where(service => service.IsActive)
                        .Select(service => service.CategoryId)))

            .ForMember(
                destination => destination.DisplayName,
                options => options.Ignore())

            .ForMember(
                destination => destination.PhoneNumber,
                options => options.Ignore());
                
        CreateMap<ProfessionalProfile,ProfessionalVerificationStatusDto>()
            .ForMember(
                destination => destination.Status,
                options => options.MapFrom(
                    source => source.Verification.ToString()))
            .ForMember(
                destination => destination.IsSubmitted,
                options => options.MapFrom(
                    source =>
                        source.Verification !=
                        VerificationStatus.NotSubmitted))
            .ForMember(
                destination => destination.IsPending,
                options => options.MapFrom(
                    source =>
                        source.Verification ==
                        VerificationStatus.Pending))
            .ForMember(
                destination => destination.IsVerificationComplete,
                options => options.MapFrom(
                    source =>
                        source.Verification ==
                        VerificationStatus.Verified))
            .ForMember(
                destination => destination.IsRejected,
                options => options.MapFrom(
                    source =>
                        source.Verification ==
                        VerificationStatus.Rejected));
    
    }
}