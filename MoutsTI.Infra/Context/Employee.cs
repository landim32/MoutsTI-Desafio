namespace MoutsTI.Infra.Context;

public partial class Employee
{
    public long EmployeeId { get; set; }

    public long RoleId { get; set; }

    public long? ManagerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string DocNumber { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime Birthday { get; set; }

    public virtual ICollection<EmployeePhone> EmployeePhones { get; set; } = new List<EmployeePhone>();

    public virtual ICollection<Employee> InverseManager { get; set; } = new List<Employee>();

    public virtual Employee? Manager { get; set; }

    public virtual EmployeeRole Role { get; set; } = null!;
}
