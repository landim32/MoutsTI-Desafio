namespace MoutsTI.Infra.Context;

public partial class EmployeePhone
{
    public long PhoneId { get; set; }

    public long EmployeeId { get; set; }

    public string Phone { get; set; } = null!;

    public virtual Employee Employee { get; set; } = null!;
}
