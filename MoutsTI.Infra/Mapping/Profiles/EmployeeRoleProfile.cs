using AutoMapper;
using MoutsTI.Domain.Entities;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Dtos;
using MoutsTI.Infra.Context;

namespace MoutsTI.Infra.Mapping.Profiles
{
    public class EmployeeRoleProfile : Profile
    {
        public EmployeeRoleProfile()
        {
            // IEmployeeRoleModel -> EmployeeRole
            CreateMap<IEmployeeRoleModel, EmployeeRole>()
                .ForMember(dest => dest.Employees, opt => opt.Ignore());

            // EmployeeRole -> EmployeeRoleModel (usando factory method Load)
            CreateMap<EmployeeRole, IEmployeeRoleModel>()
                .ConstructUsing(src => EmployeeRoleModel.Load(
                    src.RoleId,
                    src.Name,
                    src.Level));

            // EmployeeRoleModel -> EmployeeRoleDto
            CreateMap<IEmployeeRoleModel, EmployeeRoleDto>();

            // EmployeeRoleDto -> EmployeeRoleModel (usando factory method Load para atualização ou Create para novo)
            CreateMap<EmployeeRoleDto, IEmployeeRoleModel>()
                .ConstructUsing(src => src.RoleId > 0
                    ? EmployeeRoleModel.Load(src.RoleId, src.Name, src.Level)
                    : EmployeeRoleModel.Create(src.Name, src.Level));
        }
    }
}
