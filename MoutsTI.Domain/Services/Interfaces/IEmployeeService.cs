using MoutsTI.Dtos;

namespace MoutsTI.Domain.Services.Interfaces
{
    public interface IEmployeeService
    {
        IList<EmployeeDto> ListAll();
        EmployeeDto? GetById(long employeeId);
        long Add(EmployeeDto employee);
        void Update(EmployeeDto employee);
        void Delete(long employeeId);
    }
}
