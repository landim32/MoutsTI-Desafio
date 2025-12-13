using AutoMapper;
using MoutsTI.Domain.Entities;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Infra.Context;

namespace MoutsTI.Infra.Mapping.Profiles
{
    public class EntityToEmployeeProfile : Profile
    {
        public EntityToEmployeeProfile()
        {
            CreateMap<Employee, IEmployeeModel>()
                .ConstructUsing(src => CreateEmployeeModel(src))
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Manager, opt => opt.Ignore())
                .ForMember(dest => dest.Phones, opt => opt.Ignore())
                .AfterMap((src, dest, context) =>
                {
                    MapEmployeeRelations(src, dest, context);
                });
        }

        private static EmployeeModel CreateEmployeeModel(Employee src)
        {
            var parameters = new EmployeeModel.EmployeeModelParameters
            {
                EmployeeId = src.EmployeeId,
                FirstName = src.FirstName,
                LastName = src.LastName,
                DocNumber = src.DocNumber,
                Email = src.Email,
                Password = src.Password,
                Birthday = src.Birthday,
                RoleId = src.RoleId,
                ManagerId = src.ManagerId
            };

            return EmployeeModel.Load(parameters);
        }

        private static void MapEmployeeRelations(Employee src, IEmployeeModel dest, ResolutionContext context)
        {
            if (dest is not EmployeeModel employeeModel)
            {
                return;
            }

            MapRole(src, employeeModel, context);
            MapManager(src, employeeModel, context);
            MapPhones(src, employeeModel, context);
        }

        private static void MapRole(Employee src, EmployeeModel employeeModel, ResolutionContext context)
        {
            if (src.Role == null)
            {
                return;
            }

            var role = context.Mapper.Map<IEmployeeRoleModel>(src.Role);
            typeof(EmployeeModel).GetProperty("Role")!
                .SetValue(employeeModel, role);
        }

        private static void MapManager(Employee src, EmployeeModel employeeModel, ResolutionContext context)
        {
            if (src.Manager == null)
            {
                return;
            }

            var manager = context.Mapper.Map<IEmployeeModel>(src.Manager);
            typeof(EmployeeModel).GetProperty("Manager")!
                .SetValue(employeeModel, manager);
        }

        private static void MapPhones(Employee src, EmployeeModel employeeModel, ResolutionContext context)
        {
            if (src.EmployeePhones == null || src.EmployeePhones.Count == 0)
            {
                return;
            }

            foreach (var phone in src.EmployeePhones)
            {
                var phoneModel = context.Mapper.Map<EmployeePhoneModel>(phone);
                employeeModel.AddPhone(phoneModel);
            }
        }
    }
}
