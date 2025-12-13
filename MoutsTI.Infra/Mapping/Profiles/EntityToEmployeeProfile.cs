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
                .ConstructUsing((src, context) => CreateEmployeeModel(src, context))
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Manager, opt => opt.Ignore())
                .ForMember(dest => dest.Phones, opt => opt.Ignore())
                .AfterMap((src, dest, context) =>
                {
                    MapEmployeePhones(src, dest, context);
                });
        }

        private static EmployeeModel CreateEmployeeModel(Employee src, ResolutionContext context)
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
                ManagerId = src.ManagerId,
                Role = src.Role != null ? context.Mapper.Map<EmployeeRoleModel>(src.Role) : null,
                Manager = src.Manager != null ? CreateEmployeeModel(src.Manager, context) : null
            };

            return EmployeeModel.Load(parameters);
        }

        private static void MapEmployeePhones(Employee src, IEmployeeModel dest, ResolutionContext context)
        {
            if (dest is not EmployeeModel employeeModel)
            {
                return;
            }

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
