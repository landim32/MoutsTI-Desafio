using MoutsTI.Dtos;

namespace MoutsTI.Domain.Services.Interfaces
{
    public interface IEmployeeRoleService
    {
        IList<EmployeeRoleDto> ListAll();
    }
}
