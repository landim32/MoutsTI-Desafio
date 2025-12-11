namespace MoutsTI.Infra.Interfaces.Repositories
{
    public interface IEmployeeRoleRepository<TModel>
    {
        IEnumerable<TModel> ListAll();
    }
}
