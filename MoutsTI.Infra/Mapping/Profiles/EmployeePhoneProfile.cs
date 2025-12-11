using AutoMapper;
using MoutsTI.Domain.Entities;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Infra.Context;

namespace MoutsTI.Infra.Mapping.Profiles
{
    public class EmployeePhoneProfile : Profile
    {
        public EmployeePhoneProfile()
        {
            // IEmployeePhoneModel -> EmployeePhone
            CreateMap<IEmployeePhoneModel, EmployeePhone>()
                .ForMember(dest => dest.Employee, opt => opt.Ignore());

            // EmployeePhone -> EmployeePhoneModel (usando factory method Load)
            CreateMap<EmployeePhone, EmployeePhoneModel>()
                .ConstructUsing(src => EmployeePhoneModel.Load(
                    src.PhoneId,
                    src.EmployeeId,
                    src.Phone));
        }
    }
}
