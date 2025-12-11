using AutoMapper;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Infra.Context;
using MoutsTI.Infra.Interfaces.Repositories;

namespace MoutsTI.Infra.Repositories
{
    public class EmployeeRoleRepository : IEmployeeRoleRepository<IEmployeeRoleModel>
    {
        private readonly MoutsTIContext _context;
        private readonly IMapper _mapper;

        public EmployeeRoleRepository(MoutsTIContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IEnumerable<IEmployeeRoleModel> ListAll()
        {
            var entities = _context.EmployeeRoles.ToList();

            return _mapper.Map<IEnumerable<IEmployeeRoleModel>>(entities);
        }
    }
}
