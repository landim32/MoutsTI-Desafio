namespace MoutsTI.Domain.Entities.Interfaces
{
    public interface IEmployeePhoneModel
    {
        long PhoneId { get; }
        long EmployeeId { get; }
        string Phone { get; }

        void UpdatePhone(string phone);
        string GetDigitsOnly();
        bool IsMobilePhone();
        bool IsLandline();
    }
}
