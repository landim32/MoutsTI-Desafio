using AutoMapper;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Dtos;

namespace MoutsTI.Infra.Mapping.Profiles
{
    public class EmployeeToDtoProfile : Profile
    {
        public EmployeeToDtoProfile()
        {
            CreateMap<IEmployeeModel, EmployeeDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.GetFullName()))
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.Manager, opt => opt.MapFrom(src => src.Manager))
                .ForMember(dest => dest.Phones, opt => opt.MapFrom(src => src.Phones.Select(p => p.Phone).ToList()));

            CreateMap<IEmployeeModel, ManagerDto>()
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.GetFullName()));
        }
    }
}
