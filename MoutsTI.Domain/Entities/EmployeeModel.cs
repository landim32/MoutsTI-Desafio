using MoutsTI.Domain.Entities.Interfaces;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

namespace MoutsTI.Domain.Entities
{
    public partial class EmployeeModel : IEmployeeModel
    {
        private readonly List<EmployeePhoneModel> _phones;

        public long EmployeeId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string DocNumber { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public DateTime Birthday { get; private set; }
        public long RoleId { get; private set; }
        public long? ManagerId { get; private set; }

        // Navigation properties
        IEmployeeRoleModel? IEmployeeModel.Role => Role;
        IEmployeeModel? IEmployeeModel.Manager => Manager;
        IReadOnlyCollection<IEmployeePhoneModel> IEmployeeModel.Phones => Phones;

        public EmployeeRoleModel? Role { get; }
        public EmployeeModel? Manager { get; }
        public IReadOnlyCollection<EmployeePhoneModel> Phones => _phones.AsReadOnly();

        // Factory method para criar um novo funcionário
        public static EmployeeModel Create(EmployeeModelParameters parameters)
        {
            return new EmployeeModel(parameters);
        }

        // Factory method para reconstruir um funcionário existente (da base de dados)
        public static EmployeeModel Load(EmployeeModelParameters parameters)
        {
            ValidateFirstName(parameters.FirstName);
            ValidateLastName(parameters.LastName);
            ValidateDocNumber(parameters.DocNumber);
            ValidateEmail(parameters.Email);
            ValidatePassword(parameters.Password);
            ValidateBirthday(parameters.Birthday);
            ValidateRoleId(parameters.RoleId);

            return new EmployeeModel(parameters);
        }

        // Métodos de negócio para atualizar propriedades
        public void UpdateFirstName(string firstName)
        {
            ValidateFirstName(firstName);
            FirstName = firstName;
        }

        public void UpdateLastName(string lastName)
        {
            ValidateLastName(lastName);
            LastName = lastName;
        }

        public void UpdateEmail(string email)
        {
            ValidateEmail(email);
            Email = email.ToLowerInvariant().Trim();
        }

        public void UpdatePassword(string password)
        {
            ValidatePassword(password);
            Password = password;
        }

        public void UpdateBirthday(DateTime birthday)
        {
            ValidateBirthday(birthday);
            Birthday = birthday;
        }

        public void UpdateRole(long roleId)
        {
            ValidateRoleId(roleId);
            RoleId = roleId;
        }

        public void AssignManager(long? managerId)
        {
            if (managerId.HasValue && managerId.Value == EmployeeId)
                throw new InvalidOperationException("Employee cannot be their own manager.");

            ManagerId = managerId;
        }

        // Gestão de telefones
        public void AddPhone(EmployeePhoneModel phone)
        {
            if (phone == null)
                throw new ArgumentNullException(nameof(phone));

            if (phone.EmployeeId != EmployeeId && EmployeeId != 0)
                throw new InvalidOperationException("Phone does not belong to this employee.");

            if (_phones.Any(p => p.Phone == phone.Phone))
                throw new InvalidOperationException("This phone number is already registered for this employee.");

            _phones.Add(phone);
        }

        void IEmployeeModel.AddPhone(IEmployeePhoneModel phone)
        {
            if (phone is EmployeePhoneModel employeePhone)
            {
                AddPhone(employeePhone);
            }
            else
            {
                throw new ArgumentException("Phone must be of type EmployeePhoneModel", nameof(phone));
            }
        }

        public void RemovePhone(EmployeePhoneModel phone)
        {
            if (phone == null)
                throw new ArgumentNullException(nameof(phone));

            _phones.Remove(phone);
        }

        void IEmployeeModel.RemovePhone(IEmployeePhoneModel phone)
        {
            if (phone is EmployeePhoneModel employeePhone)
            {
                RemovePhone(employeePhone);
            }
            else
            {
                throw new ArgumentException("Phone must be of type EmployeePhoneModel", nameof(phone));
            }
        }

        // Métodos de negócio
        public string GetFullName() => $"{FirstName} {LastName}";

        public int GetAge()
        {
            var today = DateTime.Today;
            var age = today.Year - Birthday.Year;

            if (Birthday.Date > today.AddYears(-age))
                age--;

            return age;
        }

        public bool IsAdult() => GetAge() >= 18;

        public bool IsMinor() => GetAge() < 18;

        public bool HasManager() => ManagerId.HasValue;

        // Validações de regras de negócio
        private static void ValidateFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("First name cannot be empty.", nameof(firstName));

            if (firstName.Length > 120)
                throw new ArgumentException("First name cannot exceed 120 characters.", nameof(firstName));

            if (firstName.Length < 2)
                throw new ArgumentException("First name must have at least 2 characters.", nameof(firstName));
        }

        private static void ValidateLastName(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Last name cannot be empty.", nameof(lastName));

            if (lastName.Length > 120)
                throw new ArgumentException("Last name cannot exceed 120 characters.", nameof(lastName));

            if (lastName.Length < 2)
                throw new ArgumentException("Last name must have at least 2 characters.", nameof(lastName));
        }

        private static void ValidateDocNumber(string docNumber)
        {
            if (string.IsNullOrWhiteSpace(docNumber))
                throw new ArgumentException("Document number cannot be empty.", nameof(docNumber));

            var normalizedDoc = NormalizeDocNumber(docNumber);

            if (normalizedDoc.Length > 25)
                throw new ArgumentException("Document number cannot exceed 25 characters.", nameof(docNumber));

            // Validação básica de CPF (11 dígitos) ou CNPJ (14 dígitos)
            var digitsOnly = Regex.Replace(normalizedDoc, @"[^\d]", string.Empty);
            if (digitsOnly.Length != 11 && digitsOnly.Length != 14)
                throw new ArgumentException("Document number must be a valid CPF (11 digits) or CNPJ (14 digits).", nameof(docNumber));

            if (digitsOnly.All(c => c == digitsOnly[0]))
                throw new ArgumentException("Document number cannot have all digits the same.", nameof(docNumber));
        }

        private static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.", nameof(email));

            if (email.Length > 180)
                throw new ArgumentException("Email cannot exceed 180 characters.", nameof(email));

            // Validação básica de formato de email
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("Email format is invalid.", nameof(email));
        }

        private static void ValidatePassword(string password)
        {
            if (password?.Length > 520)
                throw new ArgumentException("Password cannot exceed 520 characters.", nameof(password));
        }

        private static void ValidateBirthday(DateTime birthday)
        {
            var today = DateTime.Today;

            if (birthday >= today)
                throw new ArgumentException("Birthday must be in the past.", nameof(birthday));

            var age = today.Year - birthday.Year;
            if (birthday.Date > today.AddYears(-age))
                age--;

            if (age > 120)
                throw new ArgumentException("Birthday indicates an age greater than 120 years.", nameof(birthday));

            // REGRA DE NEGÓCIO: Funcionário não pode ser menor de idade
            if (age < 18)
                throw new ArgumentException("Employee cannot be underage. Must be at least 18 years old.", nameof(birthday));
        }

        private static void ValidateRoleId(long roleId)
        {
            if (roleId <= 0)
                throw new ArgumentException("Role ID must be greater than zero.", nameof(roleId));
        }

        private static string NormalizeDocNumber(string docNumber)
        {
            if (string.IsNullOrWhiteSpace(docNumber))
                return docNumber;

            // Remove espaços e mantém apenas dígitos, pontos, hífens e barras
            return Regex.Replace(docNumber.Trim(), @"[^\d.\-/]", string.Empty);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not EmployeeModel other)
                return false;

            return EmployeeId == other.EmployeeId;
        }

        public override int GetHashCode()
        {
            return EmployeeId.GetHashCode();
        }

        public override string ToString()
        {
            return GetFullName();
        }

        // Substitua o construtor privado de 8 parâmetros por um que aceita um objeto de parâmetros, reduzindo o número de parâmetros diretos.
        // Isso resolve o S107, pois o construtor agora recebe apenas um parâmetro.
        private EmployeeModel(EmployeeModelParameters parameters)
        {
            ValidateFirstName(parameters.FirstName);
            ValidateLastName(parameters.LastName);
            ValidateDocNumber(parameters.DocNumber);
            ValidateEmail(parameters.Email);
            ValidatePassword(parameters.Password);
            ValidateBirthday(parameters.Birthday);
            ValidateRoleId(parameters.RoleId);

            EmployeeId = parameters.EmployeeId;
            FirstName = parameters.FirstName;
            LastName = parameters.LastName;
            DocNumber = NormalizeDocNumber(parameters.DocNumber);
            Email = parameters.Email.ToLowerInvariant().Trim();
            Password = parameters.Password;
            Birthday = parameters.Birthday;
            RoleId = parameters.RoleId;
            ManagerId = parameters.ManagerId;
            Role = parameters.Role;
            Manager = parameters.Manager;
            _phones = new List<EmployeePhoneModel>();
        }

        // Classe interna para os parâmetros do construtor
        public sealed class EmployeeModelParameters
        {
            public long EmployeeId { get; set; }
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            public string DocNumber { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public DateTime Birthday { get; set; }
            public long RoleId { get; set; }
            public long? ManagerId { get; set; }
            public EmployeeRoleModel? Role { get; set; }
            public EmployeeModel? Manager { get; set; }
        }
    }
}
