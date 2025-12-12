using AutoMapper;
using Microsoft.Extensions.Logging;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Domain.Services.Interfaces;
using MoutsTI.Dtos;
using MoutsTI.Infra.Interfaces.Repositories;

namespace MoutsTI.Domain.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository<IEmployeeModel> _repository;
        private readonly IEmployeeRoleRepository<IEmployeeRoleModel> _roleRepository;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(
            IEmployeeRepository<IEmployeeModel> repository, 
            IEmployeeRoleRepository<IEmployeeRoleModel> roleRepository,
            IMapper mapper, 
            IAuthService authService,
            ILogger<EmployeeService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public long Add(EmployeeDto employee, EmployeeDto currentEmployee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            if (currentEmployee == null)
                throw new ArgumentNullException(nameof(currentEmployee));

            _logger.LogInformation("Adding new employee. Email: {Email}, RoleId: {RoleId}, RequestedBy: {CurrentEmployeeId}", 
                employee.Email, employee.RoleId, currentEmployee.EmployeeId);

            try
            {
                // REGRA DE NEGÓCIO: Validar hierarquia de roles
                ValidateRoleHierarchy(employee.RoleId, currentEmployee);

                // Hash da senha antes de mapear
                if (!string.IsNullOrEmpty(employee.Password))
                {
                    _logger.LogDebug("Hashing password for new employee");
                    employee.Password = _authService.HashPassword(employee.Password);
                }

                // Usa AutoMapper para converter DTO para Model (com validações)
                var employeeModel = _mapper.Map<IEmployeeModel>(employee);

                // Persiste no repositório
                var employeeId = _repository.Add(employeeModel);

                _logger.LogInformation("Employee added successfully. EmployeeId: {EmployeeId}, Email: {Email}", 
                    employeeId, employee.Email);

                return employeeId;
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to add employee with RoleId: {RoleId} by employee: {CurrentEmployeeId}", 
                    employee.RoleId, currentEmployee.EmployeeId);
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while adding employee. Email: {Email}", employee.Email);
                throw new InvalidOperationException($"Validation error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding employee. Email: {Email}", employee.Email);
                throw;
            }
        }

        public void Delete(long employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("Employee ID must be greater than zero.", nameof(employeeId));

            _logger.LogInformation("Deleting employee: {EmployeeId}", employeeId);

            try
            {
                _repository.Delete(employeeId);
                _logger.LogInformation("Employee deleted successfully: {EmployeeId}", employeeId);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Failed to delete employee: {EmployeeId}", employeeId);
                throw new InvalidOperationException($"Failed to delete employee: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee: {EmployeeId}", employeeId);
                throw;
            }
        }

        public EmployeeDto? GetById(long employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("Employee ID must be greater than zero.", nameof(employeeId));

            _logger.LogDebug("Getting employee by ID: {EmployeeId}", employeeId);

            try
            {
                var employeeModel = _repository.GetById(employeeId);

                if (employeeModel == null)
                {
                    _logger.LogWarning("Employee not found: {EmployeeId}", employeeId);
                    return null;
                }

                // Usa AutoMapper para converter Model para DTO
                var employeeDto = _mapper.Map<EmployeeDto>(employeeModel);

                _logger.LogDebug("Employee retrieved successfully: {EmployeeId}", employeeId);
                return employeeDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee by ID: {EmployeeId}", employeeId);
                throw;
            }
        }

        public IList<EmployeeDto> ListAll()
        {
            _logger.LogDebug("Listing all employees");

            try
            {
                var employees = _repository.ListAll();
                var employeeDtos = _mapper.Map<IList<EmployeeDto>>(employees);

                _logger.LogInformation("Retrieved {Count} employees", employeeDtos.Count);
                return employeeDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing all employees");
                throw;
            }
        }

        public void Update(EmployeeDto employee, EmployeeDto currentEmployee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            if (currentEmployee == null)
                throw new ArgumentNullException(nameof(currentEmployee));

            if (employee.EmployeeId <= 0)
                throw new ArgumentException("Employee ID must be greater than zero.", nameof(employee));

            _logger.LogInformation("Updating employee: {EmployeeId}, RequestedBy: {CurrentEmployeeId}", 
                employee.EmployeeId, currentEmployee.EmployeeId);

            try
            {
                // Busca o funcionário existente
                var existingEmployee = _repository.GetById(employee.EmployeeId);

                if (existingEmployee == null)
                {
                    _logger.LogWarning("Employee not found for update: {EmployeeId}", employee.EmployeeId);
                    throw new InvalidOperationException($"Employee with ID {employee.EmployeeId} not found.");
                }

                // REGRA DE NEGÓCIO: Validar hierarquia de roles
                // Valida tanto a role existente quanto a nova role
                ValidateRoleHierarchy(existingEmployee.RoleId, currentEmployee);
                ValidateRoleHierarchy(employee.RoleId, currentEmployee);

                // Hash da senha se foi alterada
                if (!string.IsNullOrEmpty(employee.Password))
                {
                    _logger.LogDebug("Hashing new password for employee: {EmployeeId}", employee.EmployeeId);
                    employee.Password = _authService.HashPassword(employee.Password);
                }

                // Usa AutoMapper para converter DTO para Model (com validações)
                var employeeModel = _mapper.Map<IEmployeeModel>(employee);

                // Persiste as alterações
                _repository.Update(employeeModel);

                _logger.LogInformation("Employee updated successfully: {EmployeeId}", employee.EmployeeId);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to update employee: {EmployeeId} by employee: {CurrentEmployeeId}", 
                    employee.EmployeeId, currentEmployee.EmployeeId);
                throw;
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while updating employee: {EmployeeId}", employee.EmployeeId);
                throw new InvalidOperationException($"Validation error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee: {EmployeeId}", employee.EmployeeId);
                throw;
            }
        }

        /// <summary>
        /// Valida se o usuário atual tem permissão para gerenciar um funcionário com a role especificada.
        /// REGRA DE NEGÓCIO: Um funcionário não pode incluir ou alterar outro funcionário com role de nível superior.
        /// </summary>
        /// <param name="targetRoleId">ID da role do funcionário a ser gerenciado</param>
        /// <param name="currentEmployee">Funcionário que está realizando a operação</param>
        /// <exception cref="UnauthorizedAccessException">Quando o usuário não tem permissão</exception>
        private void ValidateRoleHierarchy(long targetRoleId, EmployeeDto currentEmployee)
        {
            _logger.LogDebug("Validating role hierarchy. TargetRoleId: {TargetRoleId}, CurrentEmployeeRoleId: {CurrentRoleId}", 
                targetRoleId, currentEmployee.RoleId);

            try
            {
                // Busca todas as roles
                var roles = _roleRepository.ListAll().ToList();

                // Busca a role do funcionário atual
                var currentRole = roles.FirstOrDefault(r => r.RoleId == currentEmployee.RoleId);
                if (currentRole == null)
                {
                    _logger.LogError("Current employee role not found: {RoleId}", currentEmployee.RoleId);
                    throw new InvalidOperationException($"Current employee role with ID {currentEmployee.RoleId} not found.");
                }

                // Busca a role do funcionário alvo
                var targetRole = roles.FirstOrDefault(r => r.RoleId == targetRoleId);
                if (targetRole == null)
                {
                    _logger.LogError("Target role not found: {RoleId}", targetRoleId);
                    throw new InvalidOperationException($"Target role with ID {targetRoleId} not found.");
                }

                // Valida hierarquia: o nível da role alvo não pode ser maior que a role atual
                if (targetRole.Level > currentRole.Level)
                {
                    _logger.LogWarning("Role hierarchy violation. CurrentRole: {CurrentRole} (Level {CurrentLevel}), TargetRole: {TargetRole} (Level {TargetLevel})", 
                        currentRole.Name, currentRole.Level, targetRole.Name, targetRole.Level);

                    throw new UnauthorizedAccessException(
                        $"You do not have permission to manage an employee with role '{targetRole.Name}' (Level {targetRole.Level}). " +
                        $"Your role '{currentRole.Name}' (Level {currentRole.Level}) does not allow managing higher-level positions.");
                }

                _logger.LogDebug("Role hierarchy validation passed");
            }
            catch (Exception ex) when (ex is not UnauthorizedAccessException)
            {
                _logger.LogError(ex, "Error validating role hierarchy");
                throw;
            }
        }
    }
}
