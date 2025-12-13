using AutoMapper;
using MoutsTI.Domain.Entities;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Dtos;

namespace MoutsTI.Infra.Mapping.Profiles
{
    public class DtoToEmployeeProfile : Profile
    {
        public DtoToEmployeeProfile()
        {
            CreateMap<EmployeeDto, IEmployeeModel>()
                .ConstructUsing(src => CreateEmployeeModel(src))
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.Manager, opt => opt.Ignore())
                .ForMember(dest => dest.Phones, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    MapPhonesToEmployee(src, dest);
                });
        }

        private static EmployeeModel CreateEmployeeModel(EmployeeDto src)
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

            return src.EmployeeId > 0
                ? EmployeeModel.Load(parameters)
                : EmployeeModel.Create(parameters);
        }

        private static void MapPhonesToEmployee(EmployeeDto src, IEmployeeModel dest)
        {
            if (src.Phones == null || src.Phones.Count == 0)
            {
                return;
            }

            if (dest is not EmployeeModel employeeModel)
            {
                return;
            }

            foreach (var phoneNumber in src.Phones)
            {
                var phone = EmployeePhoneModel.Create(src.EmployeeId, phoneNumber);
                employeeModel.AddPhone(phone);
            }
        }
    }
}
