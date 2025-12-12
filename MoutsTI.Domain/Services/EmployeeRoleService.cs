using AutoMapper;
using Microsoft.Extensions.Logging;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Domain.Services.Interfaces;
using MoutsTI.Dtos;
using MoutsTI.Infra.Interfaces.Repositories;

namespace MoutsTI.Domain.Services
{
    public class EmployeeRoleService : IEmployeeRoleService
    {
        private readonly IEmployeeRoleRepository<IEmployeeRoleModel> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeRoleService> _logger;

        public EmployeeRoleService(
            IEmployeeRoleRepository<IEmployeeRoleModel> repository, 
            IMapper mapper,
            ILogger<EmployeeRoleService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IList<EmployeeRoleDto> ListAll()
        {
            _logger.LogDebug("Listing all employee roles");

            try
            {
                var employeeRoles = _repository.ListAll();

                // Usa AutoMapper para converter lista de Models para lista de DTOs
                var employeeRoleDtos = _mapper.Map<IList<EmployeeRoleDto>>(employeeRoles);

                _logger.LogInformation("Retrieved {Count} employee roles", employeeRoleDtos.Count);
                return employeeRoleDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing all employee roles");
                throw;
            }
        }
    }
}
