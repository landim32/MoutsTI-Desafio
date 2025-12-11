using Microsoft.EntityFrameworkCore;

namespace MoutsTI.Infra.Context;

public partial class MoutsTIContext : DbContext
{
    public MoutsTIContext()
    {
    }

    public MoutsTIContext(DbContextOptions<MoutsTIContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeePhone> EmployeePhones { get; set; }

    public virtual DbSet<EmployeeRole> EmployeeRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("employees_pkey");

            entity.ToTable("employees");

            entity.HasIndex(e => e.DocNumber, "employees_doc_number_key").IsUnique();

            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.DocNumber)
                .HasMaxLength(25)
                .HasColumnName("doc_number");
            entity.Property(e => e.Email)
                .HasMaxLength(180)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(120)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(120)
                .HasColumnName("last_name");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.Password)
                .HasMaxLength(520)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Manager).WithMany(p => p.InverseManager)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("employees_manager_id_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employees_role_id_fkey");
        });

        modelBuilder.Entity<EmployeePhone>(entity =>
        {
            entity.HasKey(e => e.PhoneId).HasName("employee_phones_pkey");

            entity.ToTable("employee_phones");

            entity.Property(e => e.PhoneId).HasColumnName("phone_id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Phone)
                .HasMaxLength(25)
                .HasColumnName("phone");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeePhones)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("employee_phones_employee_id_fkey");
        });

        modelBuilder.Entity<EmployeeRole>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("employee_roles_pkey");

            entity.ToTable("employee_roles");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Level)
                .HasDefaultValue(1)
                .HasColumnName("level");
            entity.Property(e => e.Name)
                .HasMaxLength(80)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
