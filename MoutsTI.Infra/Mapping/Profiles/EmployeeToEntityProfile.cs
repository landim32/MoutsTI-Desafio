using AutoMapper;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Infra.Context;

namespace MoutsTI.Infra.Mapping.Profiles
{
    public class EmployeeToEntityProfile : Profile
    {
        public EmployeeToEntityProfile()
        {
            CreateMap<IEmployeeModel, Employee>()
                .ForMember(dest => dest.Birthday, opt => opt.MapFrom(src => 
                    src.Birthday.Kind == DateTimeKind.Unspecified 
                        ? DateTime.SpecifyKind(src.Birthday, DateTimeKind.Utc)
                        : src.Birthday.ToUniversalTime()))
                .ForMember(dest => dest.EmployeePhones, opt => opt.MapFrom(src => src.Phones))
                .ForMember(dest => dest.InverseManager, opt => opt.Ignore())
                .ForMember(dest => dest.Manager, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());
        }
    }
}
