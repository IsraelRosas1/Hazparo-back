using AutoMapper;
using Hazparo.Application.ServiceRequests.Commands.CreateServiceRequest;
using Hazparo.Application.ServiceRequests.Commands.UpdateServiceRequest;
using Hazparo.Application.ServiceRequests.Dtos;
using Hazparo.Domain.Entities;

namespace Hazparo.Application.ServiceRequests.Mappings;

public sealed class ServiceRequestProfile : Profile
{
    public ServiceRequestProfile()
    {
        CreateMap<CreateServiceRequestCommand, ServiceRequest>()
            .ForMember(
                destination => destination.Id,
                options => options.Ignore())
            .ForMember(
                destination => destination.ClientId,
                options => options.Ignore())
            .ForMember(
                destination => destination.Status,
                options => options.Ignore())
            .ForMember(
                destination => destination.Location,
                options => options.Ignore())
            .ForMember(
                destination => destination.CreatedAt,
                options => options.Ignore())
            .ForMember(
                destination => destination.UpdatedAt,
                options => options.Ignore())
            .ForMember(
                destination => destination.Photos,
                options => options.Ignore())
            .ForMember(
                destination => destination.Client,
                options => options.Ignore())
            .ForMember(
                destination => destination.Category,
                options => options.Ignore())
            .ForMember(
                destination => destination.AssignedProfessional,
                options => options.Ignore())
            .ForMember(
                destination => destination.CancelledByUser,
                options => options.Ignore())
            .ForMember(
                destination => destination.Quotes,
                options => options.Ignore())
            .ForMember(
                destination => destination.StatusHistory,
                options => options.Ignore());

        CreateMap<UpdateServiceRequestCommand, ServiceRequest>()
            .ForMember(
                destination => destination.Id,
                options => options.Ignore())
            .ForMember(
                destination => destination.ClientId,
                options => options.Ignore())
            .ForMember(
                destination => destination.Status,
                options => options.Ignore())
            .ForMember(
                destination => destination.Location,
                options => options.Ignore())
            .ForMember(
                destination => destination.CreatedAt,
                options => options.Ignore())
            .ForMember(
                destination => destination.UpdatedAt,
                options => options.Ignore())
            .ForMember(
                destination => destination.AssignedProfessionalId,
                options => options.Ignore())
            .ForMember(
                destination => destination.AgreedPriceMxn,
                options => options.Ignore())
            .ForMember(
                destination => destination.Photos,
                options => options.Ignore())
            .ForMember(
                destination => destination.Client,
                options => options.Ignore())
            .ForMember(
                destination => destination.Category,
                options => options.Ignore())
            .ForMember(
                destination => destination.AssignedProfessional,
                options => options.Ignore())
            .ForMember(
                destination => destination.CancelledByUser,
                options => options.Ignore())
            .ForMember(
                destination => destination.Quotes,
                options => options.Ignore())
            .ForMember(
                destination => destination.StatusHistory,
                options => options.Ignore());

        CreateMap<ServiceRequestPhoto, ServiceRequestPhotoDto>();

        CreateMap<ServiceRequest, ServiceRequestDetailsDto>()
            .ForMember(
                destination => destination.CategoryName,
                options => options.MapFrom(
                    source => source.Category.Name));
    }
}