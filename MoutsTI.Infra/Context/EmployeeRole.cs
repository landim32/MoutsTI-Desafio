namespace MoutsTI.Infra.Context;

public partial class EmployeeRole
{
    public long RoleId { get; set; }

    public string Name { get; set; } = null!;

    public int Level { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
