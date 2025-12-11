namespace MoutsTI.Domain.Entities.Interfaces
{
    public interface IEmployeeRoleModel
    {
        long RoleId { get; }
        string Name { get; }
        int Level { get; }

        void UpdateName(string name);
        void UpdateLevel(int level);
        bool CanBeDeleted();
    }
}
