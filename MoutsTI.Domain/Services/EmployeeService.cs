using AutoMapper;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Domain.Services.Interfaces;
using MoutsTI.Dtos;
using MoutsTI.Infra.Interfaces.Repositories;

namespace MoutsTI.Domain.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository<IEmployeeModel> _repository;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public EmployeeService(IEmployeeRepository<IEmployeeModel> repository, IMapper mapper, IAuthService authService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public long Add(EmployeeDto employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            try
            {
                // Hash da senha antes de mapear
                if (!string.IsNullOrEmpty(employee.Password))
                {
                    employee.Password = _authService.HashPassword(employee.Password);
                }

                // Usa AutoMapper para converter DTO para Model (com validações)
                var employeeModel = _mapper.Map<IEmployeeModel>(employee);

                // Persiste no repositório
                return _repository.Add(employeeModel);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException($"Validation error: {ex.Message}", ex);
            }
        }

        public void Delete(long employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("Employee ID must be greater than zero.", nameof(employeeId));

            try
            {
                _repository.Delete(employeeId);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"Failed to delete employee: {ex.Message}", ex);
            }
        }

        public EmployeeDto? GetById(long employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("Employee ID must be greater than zero.", nameof(employeeId));

            var employeeModel = _repository.GetById(employeeId);

            if (employeeModel == null)
                return null;

            // Usa AutoMapper para converter Model para DTO
            return _mapper.Map<EmployeeDto>(employeeModel);
        }

        public IList<EmployeeDto> ListAll()
        {
            var employees = _repository.ListAll();

            // Usa AutoMapper para converter lista de Models para lista de DTOs
            return _mapper.Map<IList<EmployeeDto>>(employees);
        }

        public void Update(EmployeeDto employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            if (employee.EmployeeId <= 0)
                throw new ArgumentException("Employee ID must be greater than zero.", nameof(employee));

            try
            {
                // Busca o funcionário existente
                var existingEmployee = _repository.GetById(employee.EmployeeId);

                if (existingEmployee == null)
                    throw new InvalidOperationException($"Employee with ID {employee.EmployeeId} not found.");

                // Hash da senha se foi alterada
                if (!string.IsNullOrEmpty(employee.Password))
                {
                    employee.Password = _authService.HashPassword(employee.Password);
                }

                // Usa AutoMapper para converter DTO para Model (com validações)
                var employeeModel = _mapper.Map<IEmployeeModel>(employee);

                // Persiste as alterações
                _repository.Update(employeeModel);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException($"Validation error: {ex.Message}", ex);
            }
        }
    }
}
