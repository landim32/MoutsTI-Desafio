using MoutsTI.Dtos;

namespace MoutsTI.Domain.Services.Interfaces
{
    public interface IEmployeeService
    {
        IList<EmployeeDto> ListAll();
        EmployeeDto? GetById(long employeeId);
        long Add(EmployeeDto employee, EmployeeDto currentEmployee);
        void Update(EmployeeDto employee, EmployeeDto currentEmployee);
        void Delete(long employeeId);
    }
}
