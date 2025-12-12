using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Infra.Context;
using MoutsTI.Infra.Interfaces.Repositories;

namespace MoutsTI.Infra.Repositories
{
    public class EmployeeRepository : IEmployeeRepository<IEmployeeModel>
    {
        private readonly MoutsTIContext _context;
        private readonly IMapper _mapper;

        public EmployeeRepository(MoutsTIContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public long Add(IEmployeeModel employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            var entity = _mapper.Map<Employee>(employee);

            _context.Employees.Add(entity);
            _context.SaveChanges();

            return entity.EmployeeId;
        }

        public void Delete(long employeeId)
        {
            var entity = _context.Employees.Find(employeeId);

            if (entity == null)
                throw new InvalidOperationException($"Employee with ID {employeeId} not found.");

            _context.Employees.Remove(entity);
            _context.SaveChanges();
        }

        public IEmployeeModel? GetById(long employeeId)
        {
            var entity = _context.Employees
                .Include(e => e.Role)
                .Include(e => e.Manager)
                .Include(e => e.EmployeePhones)
                .FirstOrDefault(e => e.EmployeeId == employeeId);

            if (entity == null)
                return null;

            return _mapper.Map<IEmployeeModel>(entity);
        }

        public IEnumerable<IEmployeeModel> ListAll()
        {
            var entities = _context.Employees
                .Include(e => e.Role)
                .Include(e => e.Manager)
                .Include(e => e.EmployeePhones)
                .ToList();

            return _mapper.Map<IEnumerable<IEmployeeModel>>(entities);
        }

        public void Update(IEmployeeModel employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            var existingEntity = _context.Employees
                .Include(e => e.EmployeePhones)
                .FirstOrDefault(e => e.EmployeeId == employee.EmployeeId);

            if (existingEntity == null)
                throw new InvalidOperationException($"Employee with ID {employee.EmployeeId} not found.");

            // Armazena a senha anterior antes do mapeamento
            var previousPassword = existingEntity.Password;

            _mapper.Map(employee, existingEntity);

            // Se a nova senha estiver vazia, mantém a senha anterior
            if (string.IsNullOrWhiteSpace(employee.Password))
            {
                existingEntity.Password = previousPassword;
            }

            _context.Employees.Update(existingEntity);
            _context.SaveChanges();
        }

        public async Task<IEmployeeModel?> GetByEmailAsync(string email)
        {
            var employeeEntity = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == email);

            if (employeeEntity == null)
                return null;

            return _mapper.Map<IEmployeeModel>(employeeEntity);
        }
    }
}
