namespace MoutsTI.Infra.Interfaces.Repositories
{
    public interface IEmployeeRoleRepository<out TModel>
    {
        IEnumerable<TModel> ListAll();
    }
}
