using AutoMapper;
using MoutsTI.Domain.Entities;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Dtos;
using MoutsTI.Infra.Context;

namespace MoutsTI.Infra.Mapping.Profiles
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            // IEmployeeModel -> Employee
            CreateMap<IEmployeeModel, Employee>()
                .ForMember(dest => dest.EmployeePhones, opt => opt.MapFrom(src => src.Phones))
                .ForMember(dest => dest.InverseManager, opt => opt.Ignore())
                .ForMember(dest => dest.Manager, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            // Employee -> EmployeeModel (usando factory method Load)
            CreateMap<Employee, IEmployeeModel>()
                .ConstructUsing(src => EmployeeModel.Load(
                    src.EmployeeId,
                    src.FirstName,
                    src.LastName,
                    src.DocNumber,
                    src.Email,
                    src.Password,
                    src.Birthday,
                    src.RoleId,
                    src.ManagerId))
                .AfterMap((src, dest, context) =>
                {
                    var employeeModel = dest as EmployeeModel;
                    if (employeeModel != null)
                    {
                        // Mapeia Role
                        if (src.Role != null)
                        {
                            var role = context.Mapper.Map<EmployeeRoleModel>(src.Role);
                            typeof(EmployeeModel).GetProperty("Role")!
                                .SetValue(employeeModel, role);
                        }

                        // Mapeia Manager
                        if (src.Manager != null)
                        {
                            var manager = context.Mapper.Map<EmployeeModel>(src.Manager);
                            typeof(EmployeeModel).GetProperty("Manager")!
                                .SetValue(employeeModel, manager);
                        }

                        // Mapeia Phones
                        if (src.EmployeePhones != null && src.EmployeePhones.Any())
                        {
                            foreach (var phone in src.EmployeePhones)
                            {
                                var phoneModel = context.Mapper.Map<EmployeePhoneModel>(phone);
                                employeeModel.AddPhone(phoneModel);
                            }
                        }
                    }
                });

            // IEmployeeModel -> EmployeeDto
            CreateMap<IEmployeeModel, EmployeeDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.GetFullName()))
                .ForMember(dest => dest.Password, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
                .ForMember(dest => dest.Manager, opt => opt.MapFrom(src => src.Manager))
                .ForMember(dest => dest.Phones, opt => opt.MapFrom(src => src.Phones.Select(p => p.Phone).ToList()));

            // IEmployeeModel -> ManagerDto
            CreateMap<IEmployeeModel, ManagerDto>()
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.GetFullName()));

            // EmployeeDto -> EmployeeModel (usando factory method Load para atualização ou Create para novo)
            CreateMap<EmployeeDto, IEmployeeModel>()
                .ConstructUsing(src => src.EmployeeId > 0
                    ? EmployeeModel.Load(
                        src.EmployeeId,
                        src.FirstName,
                        src.LastName,
                        src.DocNumber,
                        src.Email,
                        src.Password,
                        src.Birthday,
                        src.RoleId,
                        src.ManagerId)
                    : EmployeeModel.Create(
                        src.FirstName,
                        src.LastName,
                        src.DocNumber,
                        src.Email,
                        src.Password,
                        src.Birthday,
                        src.RoleId,
                        src.ManagerId))
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Manager, opt => opt.Ignore())
                .ForMember(dest => dest.Phones, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    if (src.Phones != null && src.Phones.Any())
                    {
                        var employeeModel = dest as EmployeeModel;
                        if (employeeModel != null)
                        {
                            foreach (var phoneNumber in src.Phones)
                            {
                                var phone = EmployeePhoneModel.Create(src.EmployeeId, phoneNumber);
                                employeeModel.AddPhone(phone);
                            }
                        }
                    }
                });
        }
    }
}
