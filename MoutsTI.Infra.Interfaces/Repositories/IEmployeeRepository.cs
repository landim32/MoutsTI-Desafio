namespace MoutsTI.Infra.Interfaces.Repositories
{
    public interface IEmployeeRepository<TModel>
    {
        IEnumerable<TModel> ListAll();
        TModel? GetById(long resumeId);
        long Add(TModel resume);
        void Update(TModel resume);
        void Delete(long resumeId);
        Task<TModel?> GetByEmailAsync(string email);
    }
}
