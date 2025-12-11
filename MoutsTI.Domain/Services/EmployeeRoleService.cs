using AutoMapper;
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

        public EmployeeRoleService(IEmployeeRoleRepository<IEmployeeRoleModel> repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IList<EmployeeRoleDto> ListAll()
        {
            var employeeRoles = _repository.ListAll();

            // Usa AutoMapper para converter lista de Models para lista de DTOs
            return _mapper.Map<IList<EmployeeRoleDto>>(employeeRoles);
        }
    }
}
