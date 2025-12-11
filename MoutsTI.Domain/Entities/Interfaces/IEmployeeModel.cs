namespace MoutsTI.Domain.Entities.Interfaces
{
    public interface IEmployeeModel
    {
        // Properties
        long EmployeeId { get; }
        string FirstName { get; }
        string LastName { get; }
        string DocNumber { get; }
        string Email { get; }
        string Password { get; }
        DateTime Birthday { get; }
        long RoleId { get; }
        long? ManagerId { get; }

        // Navigation properties
        IEmployeeRoleModel Role { get; }
        IEmployeeModel Manager { get; }
        IReadOnlyCollection<IEmployeePhoneModel> Phones { get; }

        // Update methods
        void UpdateFirstName(string firstName);
        void UpdateLastName(string lastName);
        void UpdateEmail(string email);
        void UpdatePassword(string password);
        void UpdateBirthday(DateTime birthday);
        void UpdateRole(long roleId);
        void AssignManager(long? managerId);

        // Phone management
        void AddPhone(IEmployeePhoneModel phone);
        void RemovePhone(IEmployeePhoneModel phone);

        // Business methods
        string GetFullName();
        int GetAge();
        bool IsAdult();
        bool IsMinor();
        bool HasManager();
    }
}
