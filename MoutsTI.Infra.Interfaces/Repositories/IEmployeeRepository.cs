namespace MoutsTI.Infra.Interfaces.Repositories
{
    public interface IEmployeeRepository<TModel>
    {
        IEnumerable<TModel> ListAll();
        TModel? GetById(long employeeId);
        long Add(TModel employee);
        void Update(TModel employee);
        void Delete(long employeeId);
        Task<TModel?> GetByEmailAsync(string email);
    }
}
