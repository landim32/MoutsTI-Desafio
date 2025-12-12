using FluentAssertions;
using MoutsTI.Domain.Entities;

namespace MoutsTI.Tests.Domain.Entities
{
    public class EmployeeModelTests
    {
        private readonly DateTime _validBirthday = DateTime.Today.AddYears(-25);

        #region Create Tests

        [Fact]
        public void Create_WithValidParameters_ShouldCreateInstance()
        {
            // Arrange
            var firstName = "John";
            var lastName = "Doe";
            var docNumber = "123.456.789-01";
            var email = "john.doe@example.com";
            var password = "Password123!";
            var birthday = _validBirthday;
            var roleId = 1L;

            // Act
            var employee = EmployeeModel.Create(firstName, lastName, docNumber, email, password, birthday, roleId);

            // Assert
            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(0);
            employee.FirstName.Should().Be("John");
            employee.LastName.Should().Be("Doe");
            employee.DocNumber.Should().Be("123.456.789-01");
            employee.Email.Should().Be("john.doe@example.com");
            employee.Password.Should().Be("Password123!");
            employee.Birthday.Should().Be(birthday);
            employee.RoleId.Should().Be(1);
            employee.ManagerId.Should().BeNull();
        }

        [Fact]
        public void Create_WithManager_ShouldCreateInstanceWithManagerId()
        {
            // Arrange
            var managerId = 5L;

            // Act
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01", 
                "john@example.com", "pass", _validBirthday, 1L, managerId);

            // Assert
            employee.ManagerId.Should().Be(5);
            employee.HasManager().Should().BeTrue();
        }

        [Fact]
        public void Create_WithoutManager_ShouldCreateInstanceWithNullManagerId()
        {
            // Act
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Assert
            employee.ManagerId.Should().BeNull();
            employee.HasManager().Should().BeFalse();
        }

        [Fact]
        public void Create_ShouldNormalizeEmail()
        {
            // Arrange
            var email = "JOHN.DOE@EXAMPLE.COM"; // Without spaces to avoid validation error

            // Act
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                email, "pass", _validBirthday, 1L);

            // Assert
            employee.Email.Should().Be("john.doe@example.com");
        }

        [Fact]
        public void Create_ShouldNormalizeDocNumber()
        {
            // Arrange
            var docNumber = "  123.456.789-01  ";

            // Act
            var employee = EmployeeModel.Create(
                "John", "Doe", docNumber,
                "john@example.com", "pass", _validBirthday, 1L);

            // Assert
            employee.DocNumber.Should().Be("123.456.789-01");
        }

        #endregion

        #region Create Validation Tests - FirstName

        [Fact]
        public void Create_WithEmptyFirstName_ShouldThrowArgumentException()
        {
            // Act
            Action act = () => EmployeeModel.Create(
                "", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*First name cannot be empty*")
                .WithParameterName("firstName");
        }

        [Fact]
        public void Create_WithNullFirstName_ShouldThrowArgumentException()
        {
            // Act
            Action act = () => EmployeeModel.Create(
                null!, "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("firstName");
        }

        [Fact]
        public void Create_WithFirstNameLessThan2Characters_ShouldThrowArgumentException()
        {
            // Act
            Action act = () => EmployeeModel.Create(
                "J", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*First name must have at least 2 characters*")
                .WithParameterName("firstName");
        }

        [Fact]
        public void Create_WithFirstNameExceeding120Characters_ShouldThrowArgumentException()
        {
            // Arrange
            var longName = new string('A', 121);

            // Act
            Action act = () => EmployeeModel.Create(
                longName, "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*First name cannot exceed 120 characters*")
                .WithParameterName("firstName");
        }

        [Fact]
        public void Create_WithFirstNameExactly120Characters_ShouldCreateInstance()
        {
            // Arrange
            var name = new string('A', 120);

            // Act
            var employee = EmployeeModel.Create(
                name, "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Assert
            employee.FirstName.Should().HaveLength(120);
        }

        #endregion

        #region Create Validation Tests - LastName

        [Fact]
        public void Create_WithEmptyLastName_ShouldThrowArgumentException()
        {
            // Act
            Action act = () => EmployeeModel.Create(
                "John", "", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Last name cannot be empty*")
                .WithParameterName("lastName");
        }

        [Fact]
        public void Create_WithLastNameLessThan2Characters_ShouldThrowArgumentException()
        {
            // Act
            Action act = () => EmployeeModel.Create(
                "John", "D", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Last name must have at least 2 characters*")
                .WithParameterName("lastName");
        }

        [Fact]
        public void Create_WithLastNameExceeding120Characters_ShouldThrowArgumentException()
        {
            // Arrange
            var longName = new string('A', 121);

            // Act
            Action act = () => EmployeeModel.Create(
                "John", longName, "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Last name cannot exceed 120 characters*")
                .WithParameterName("lastName");
        }

        #endregion

        #region Create Validation Tests - DocNumber

        [Fact]
        public void Create_WithValidCPF_ShouldCreateInstance()
        {
            // Act
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Assert
            employee.Should().NotBeNull();
        }

        [Fact]
        public void Create_WithValidCNPJ_ShouldCreateInstance()
        {
            // Act
            var employee = EmployeeModel.Create(
                "John", "Doe", "12.345.678/0001-90",
                "john@example.com", "pass", _validBirthday, 1L);

            // Assert
            employee.Should().NotBeNull();
        }

        [Fact]
        public void Create_WithEmptyDocNumber_ShouldThrowArgumentException()
        {
            // Act
            Action act = () => EmployeeModel.Create(
                "John", "Doe", "",
                "john@example.com", "pass", _validBirthday, 1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Document number cannot be empty*")
                .WithParameterName("docNumber");
        }

        [Fact]
        public void Create_WithInvalidDocNumberLength_ShouldThrowArgumentException()
        {
            // Act
            Action act = () => EmployeeModel.Create(
                "John", "Doe", "123.456",
                "john@example.com", "pass", _validBirthday, 1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Document number must be a valid CPF (11 digits) or CNPJ (14 digits)*")
                .WithParameterName("docNumber");
        }

        [Fact]
        public void Create_WithDocNumberAllSameDigits_ShouldThrowArgumentException()
        {
            // Act
            Action act = () => EmployeeModel.Create(
                "John", "Doe", "111.111.111-11",
                "john@example.com", "pass", _validBirthday, 1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Document number cannot have all digits the same*")
                .WithParameterName("docNumber");
        }

        [Fact]
        public void Create_WithDocNumberExceeding25Characters_ShouldThrowArgumentException()
        {
            // Arrange
            var longDoc = new string('1', 30);

            // Act
            Action act = () => EmployeeModel.Create(
                "John", "Doe", longDoc,
                "john@example.com", "pass", _validBirthday, 1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Document number cannot exceed 25 characters*")
                .WithParameterName("docNumber");
        }

        #endregion

        #region Create Validation Tests - Email

        [Fact]
        public void Create_WithEmptyEmail_ShouldThrowArgumentException()
        {
            // Act
            Action act = () => EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "", "pass", _validBirthday, 1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Email cannot be empty*")
                .WithParameterName("email");
        }

        [Fact]
        public void Create_WithInvalidEmailFormat_ShouldThrowArgumentException()
        {
            // Act
            Action act = () => EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "invalid-email", "pass", _validBirthday, 1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Email format is invalid*")
                .WithParameterName("email");
        }

        [Fact]
        public void Create_WithEmailExceeding180Characters_ShouldThrowArgumentException()
        {
            // Arrange
            var longEmail = new string('a', 172) + "@test.com"; // Total 181 chars

            // Act
            Action act = () => EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                longEmail, "pass", _validBirthday, 1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Email cannot exceed 180 characters*")
                .WithParameterName("email");
        }

        [Theory]
        [InlineData("user@domain.com")]
        [InlineData("user.name@domain.com")]
        [InlineData("user+tag@domain.co.uk")]
        [InlineData("user_name@domain.com")]
        public void Create_WithValidEmailFormats_ShouldCreateInstance(string email)
        {
            // Act
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                email, "pass", _validBirthday, 1L);

            // Assert
            employee.Email.Should().Be(email.ToLowerInvariant());
        }

        #endregion

        #region Create Validation Tests - Password

        [Fact]
        public void Create_WithPasswordExceeding520Characters_ShouldThrowArgumentException()
        {
            // Arrange
            var longPassword = new string('a', 521);

            // Act
            Action act = () => EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", longPassword, _validBirthday, 1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Password cannot exceed 520 characters*")
                .WithParameterName("password");
        }

        [Fact]
        public void Create_WithEmptyPassword_ShouldCreateInstance()
        {
            // Act - Password validation is commented out in the code
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "", _validBirthday, 1L);

            // Assert
            employee.Password.Should().Be("");
        }

        #endregion

        #region Create Validation Tests - Birthday

        [Fact]
        public void Create_WithBirthdayInFuture_ShouldThrowArgumentException()
        {
            // Arrange
            var futureBirthday = DateTime.Today.AddDays(1);

            // Act
            Action act = () => EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", futureBirthday, 1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Birthday must be in the past*")
                .WithParameterName("birthday");
        }

        [Fact]
        public void Create_WithBirthdayToday_ShouldThrowArgumentException()
        {
            // Arrange
            var today = DateTime.Today;

            // Act
            Action act = () => EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", today, 1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Birthday must be in the past*")
                .WithParameterName("birthday");
        }

        [Fact]
        public void Create_WithAgeOver120_ShouldThrowArgumentException()
        {
            // Arrange
            var veryOldBirthday = DateTime.Today.AddYears(-121);

            // Act
            Action act = () => EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", veryOldBirthday, 1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Birthday indicates an age greater than 120 years*")
                .WithParameterName("birthday");
        }

        [Fact]
        public void Create_WithAgeUnder18_ShouldThrowArgumentException()
        {
            // Arrange
            var minorBirthday = DateTime.Today.AddYears(-17);

            // Act
            Action act = () => EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", minorBirthday, 1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Employee cannot be underage. Must be at least 18 years old*")
                .WithParameterName("birthday");
        }

        [Fact]
        public void Create_WithExactly18YearsOld_ShouldCreateInstance()
        {
            // Arrange
            var birthday = DateTime.Today.AddYears(-18);

            // Act
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", birthday, 1L);

            // Assert
            employee.GetAge().Should().Be(18);
            employee.IsAdult().Should().BeTrue();
            employee.IsMinor().Should().BeFalse();
        }

        #endregion

        #region Create Validation Tests - RoleId

        [Fact]
        public void Create_WithZeroRoleId_ShouldThrowArgumentException()
        {
            // Act
            Action act = () => EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 0L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Role ID must be greater than zero*")
                .WithParameterName("roleId");
        }

        [Fact]
        public void Create_WithNegativeRoleId_ShouldThrowArgumentException()
        {
            // Act
            Action act = () => EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, -1L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Role ID must be greater than zero*")
                .WithParameterName("roleId");
        }

        #endregion

        #region Load Tests

        [Fact]
        public void Load_WithValidParameters_ShouldCreateInstance()
        {
            // Act
            var employee = EmployeeModel.Load(
                1L, "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, null);

            // Assert
            employee.Should().NotBeNull();
            employee.EmployeeId.Should().Be(1);
            employee.FirstName.Should().Be("John");
            employee.LastName.Should().Be("Doe");
        }

        [Fact]
        public void Load_WithZeroEmployeeId_ShouldCreateInstance()
        {
            // Act
            var employee = EmployeeModel.Load(
                0L, "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, null);

            // Assert
            employee.EmployeeId.Should().Be(0);
        }

        [Fact]
        public void Load_WithNegativeEmployeeId_ShouldCreateInstance()
        {
            // Act
            var employee = EmployeeModel.Load(
                -1L, "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, null);

            // Assert
            employee.EmployeeId.Should().Be(-1);
        }

        [Fact]
        public void Load_ShouldValidateAllParameters()
        {
            // Act
            Action act = () => EmployeeModel.Load(
                1L, "", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, null);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("firstName");
        }

        #endregion

        #region Update Tests - FirstName

        [Fact]
        public void UpdateFirstName_WithValidName_ShouldUpdateFirstName()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Act
            employee.UpdateFirstName("Jane");

            // Assert
            employee.FirstName.Should().Be("Jane");
        }

        [Fact]
        public void UpdateFirstName_WithInvalidName_ShouldThrowArgumentException()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Act
            Action act = () => employee.UpdateFirstName("");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("firstName");
        }

        #endregion

        #region Update Tests - LastName

        [Fact]
        public void UpdateLastName_WithValidName_ShouldUpdateLastName()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Act
            employee.UpdateLastName("Smith");

            // Assert
            employee.LastName.Should().Be("Smith");
        }

        [Fact]
        public void UpdateLastName_WithInvalidName_ShouldThrowArgumentException()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Act
            Action act = () => employee.UpdateLastName("D");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("lastName");
        }

        #endregion

        #region Update Tests - Email

        [Fact]
        public void UpdateEmail_WithValidEmail_ShouldUpdateEmail()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Act
            employee.UpdateEmail("newemail@example.com");

            // Assert
            employee.Email.Should().Be("newemail@example.com");
        }

        [Fact]
        public void UpdateEmail_ShouldNormalizeEmail()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Act
            employee.UpdateEmail("NEWEMAIL@EXAMPLE.COM"); // Without extra spaces to avoid validation error

            // Assert
            employee.Email.Should().Be("newemail@example.com");
        }

        [Fact]
        public void UpdateEmail_WithInvalidEmail_ShouldThrowArgumentException()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Act
            Action act = () => employee.UpdateEmail("invalid-email");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("email");
        }

        #endregion

        #region Update Tests - Password

        [Fact]
        public void UpdatePassword_WithValidPassword_ShouldUpdatePassword()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "oldpass", _validBirthday, 1L);

            // Act
            employee.UpdatePassword("newpass");

            // Assert
            employee.Password.Should().Be("newpass");
        }

        [Fact]
        public void UpdatePassword_WithTooLongPassword_ShouldThrowArgumentException()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);
            var longPassword = new string('a', 521);

            // Act
            Action act = () => employee.UpdatePassword(longPassword);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("password");
        }

        #endregion

        #region Update Tests - Birthday

        [Fact]
        public void UpdateBirthday_WithValidBirthday_ShouldUpdateBirthday()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);
            var newBirthday = DateTime.Today.AddYears(-30);

            // Act
            employee.UpdateBirthday(newBirthday);

            // Assert
            employee.Birthday.Should().Be(newBirthday);
            employee.GetAge().Should().Be(30);
        }

        [Fact]
        public void UpdateBirthday_WithInvalidBirthday_ShouldThrowArgumentException()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);
            var futureBirthday = DateTime.Today.AddYears(1);

            // Act
            Action act = () => employee.UpdateBirthday(futureBirthday);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("birthday");
        }

        #endregion

        #region Update Tests - Role

        [Fact]
        public void UpdateRole_WithValidRoleId_ShouldUpdateRole()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Act
            employee.UpdateRole(2L);

            // Assert
            employee.RoleId.Should().Be(2);
        }

        [Fact]
        public void UpdateRole_WithInvalidRoleId_ShouldThrowArgumentException()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Act
            Action act = () => employee.UpdateRole(0L);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("roleId");
        }

        #endregion

        #region AssignManager Tests

        [Fact]
        public void AssignManager_WithValidManagerId_ShouldAssignManager()
        {
            // Arrange
            var employee = EmployeeModel.Load(
                1L, "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, null);

            // Act
            employee.AssignManager(5L);

            // Assert
            employee.ManagerId.Should().Be(5);
            employee.HasManager().Should().BeTrue();
        }

        [Fact]
        public void AssignManager_WithNull_ShouldRemoveManager()
        {
            // Arrange
            var employee = EmployeeModel.Load(
                1L, "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, 5L);

            // Act
            employee.AssignManager(null);

            // Assert
            employee.ManagerId.Should().BeNull();
            employee.HasManager().Should().BeFalse();
        }

        [Fact]
        public void AssignManager_WithSelfAsManager_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var employee = EmployeeModel.Load(
                1L, "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, null);

            // Act
            Action act = () => employee.AssignManager(1L);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*Employee cannot be their own manager*");
        }

        #endregion

        #region Phone Management Tests

        [Fact]
        public void AddPhone_WithValidPhone_ShouldAddPhone()
        {
            // Arrange
            var employee = EmployeeModel.Load(
                1L, "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, null);
            var phone = EmployeePhoneModel.Create(1L, "11987654321");

            // Act
            employee.AddPhone(phone);

            // Assert
            employee.Phones.Should().HaveCount(1);
            employee.Phones.First().Phone.Should().Be("11987654321");
        }

        [Fact]
        public void AddPhone_WithNullPhone_ShouldThrowArgumentNullException()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Act
            Action act = () => employee.AddPhone(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("phone");
        }

        [Fact]
        public void AddPhone_WithPhoneBelongingToAnotherEmployee_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var employee = EmployeeModel.Load(
                1L, "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, null);
            var phone = EmployeePhoneModel.Create(2L, "11987654321");

            // Act
            Action act = () => employee.AddPhone(phone);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*Phone does not belong to this employee*");
        }

        [Fact]
        public void AddPhone_WithDuplicatePhoneNumber_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var employee = EmployeeModel.Load(
                1L, "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, null);
            var phone1 = EmployeePhoneModel.Create(1L, "11987654321");
            var phone2 = EmployeePhoneModel.Create(1L, "11987654321");

            employee.AddPhone(phone1);

            // Act
            Action act = () => employee.AddPhone(phone2);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*This phone number is already registered for this employee*");
        }

        [Fact]
        public void AddPhone_WithMultipleDifferentPhones_ShouldAddAll()
        {
            // Arrange
            var employee = EmployeeModel.Load(
                1L, "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, null);
            var phone1 = EmployeePhoneModel.Create(1L, "11987654321");
            var phone2 = EmployeePhoneModel.Create(1L, "1133334444");

            // Act
            employee.AddPhone(phone1);
            employee.AddPhone(phone2);

            // Assert
            employee.Phones.Should().HaveCount(2);
        }

        [Fact]
        public void RemovePhone_WithValidPhone_ShouldRemovePhone()
        {
            // Arrange
            var employee = EmployeeModel.Load(
                1L, "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, null);
            var phone = EmployeePhoneModel.Create(1L, "11987654321");
            employee.AddPhone(phone);

            // Act
            employee.RemovePhone(phone);

            // Assert
            employee.Phones.Should().BeEmpty();
        }

        [Fact]
        public void RemovePhone_WithNullPhone_ShouldThrowArgumentNullException()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Act
            Action act = () => employee.RemovePhone(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("phone");
        }

        #endregion

        #region Business Logic Tests - Age

        [Fact]
        public void GetAge_ShouldCalculateCorrectAge()
        {
            // Arrange
            var birthday = DateTime.Today.AddYears(-25);
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", birthday, 1L);

            // Act
            var age = employee.GetAge();

            // Assert
            age.Should().Be(25);
        }

        [Fact]
        public void GetAge_BeforeBirthdayThisYear_ShouldReturnCorrectAge()
        {
            // Arrange
            var birthday = DateTime.Today.AddYears(-25).AddDays(1); // Birthday hasn't occurred yet this year
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", birthday, 1L);

            // Act
            var age = employee.GetAge();

            // Assert
            age.Should().Be(24); // One year less because birthday hasn't occurred yet
        }

        [Fact]
        public void IsAdult_WithAgeOver18_ShouldReturnTrue()
        {
            // Arrange
            var birthday = DateTime.Today.AddYears(-25);
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", birthday, 1L);

            // Act
            var isAdult = employee.IsAdult();

            // Assert
            isAdult.Should().BeTrue();
        }

        [Fact]
        public void IsAdult_WithExactly18Years_ShouldReturnTrue()
        {
            // Arrange
            var birthday = DateTime.Today.AddYears(-18);
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", birthday, 1L);

            // Act
            var isAdult = employee.IsAdult();

            // Assert
            isAdult.Should().BeTrue();
        }

        [Fact]
        public void IsMinor_WithAgeOver18_ShouldReturnFalse()
        {
            // Arrange
            var birthday = DateTime.Today.AddYears(-25);
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", birthday, 1L);

            // Act
            var isMinor = employee.IsMinor();

            // Assert
            isMinor.Should().BeFalse();
        }

        #endregion

        #region Business Logic Tests - GetFullName

        [Fact]
        public void GetFullName_ShouldReturnCombinedName()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Act
            var fullName = employee.GetFullName();

            // Assert
            fullName.Should().Be("John Doe");
        }

        [Fact]
        public void ToString_ShouldReturnFullName()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Act
            var result = employee.ToString();

            // Assert
            result.Should().Be("John Doe");
        }

        #endregion

        #region Equals and GetHashCode Tests

        [Fact]
        public void Equals_WithSameEmployeeId_ShouldReturnTrue()
        {
            // Arrange
            var employee1 = EmployeeModel.Load(
                1L, "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, null);
            var employee2 = EmployeeModel.Load(
                1L, "Jane", "Smith", "987.654.321-09",
                "jane@example.com", "pass", _validBirthday, 2L, null);

            // Act
            var areEqual = employee1.Equals(employee2);

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentEmployeeId_ShouldReturnFalse()
        {
            // Arrange
            var employee1 = EmployeeModel.Load(
                1L, "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, null);
            var employee2 = EmployeeModel.Load(
                2L, "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, null);

            // Act
            var areEqual = employee1.Equals(employee2);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Act
            var areEqual = employee.Equals(null);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void Equals_WithDifferentType_ShouldReturnFalse()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);
            var other = new object();

            // Act
            var areEqual = employee.Equals(other);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_WithSameEmployeeId_ShouldReturnSameHashCode()
        {
            // Arrange
            var employee1 = EmployeeModel.Load(
                1L, "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, null);
            var employee2 = EmployeeModel.Load(
                1L, "Jane", "Smith", "987.654.321-09",
                "jane@example.com", "pass", _validBirthday, 2L, null);

            // Act
            var hash1 = employee1.GetHashCode();
            var hash2 = employee2.GetHashCode();

            // Assert
            hash1.Should().Be(hash2);
        }

        [Fact]
        public void GetHashCode_WithDifferentEmployeeId_ShouldReturnDifferentHashCode()
        {
            // Arrange
            var employee1 = EmployeeModel.Load(
                1L, "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, null);
            var employee2 = EmployeeModel.Load(
                2L, "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, null);

            // Act
            var hash1 = employee1.GetHashCode();
            var hash2 = employee2.GetHashCode();

            // Assert
            hash1.Should().NotBe(hash2);
        }

        #endregion

        #region Immutability Tests

        [Fact]
        public void Employee_ShouldMaintainImmutableEmployeeId()
        {
            // Arrange
            var employee = EmployeeModel.Load(
                1L, "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, null);
            var originalId = employee.EmployeeId;

            // Act
            employee.UpdateFirstName("Jane");
            employee.UpdateLastName("Smith");
            employee.UpdateEmail("jane@example.com");
            employee.UpdateRole(2L);

            // Assert
            employee.EmployeeId.Should().Be(originalId);
        }

        [Fact]
        public void Phones_ShouldBeReadOnly()
        {
            // Arrange
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L);

            // Act
            var phones = employee.Phones;

            // Assert
            phones.Should().BeAssignableTo<IReadOnlyCollection<EmployeePhoneModel>>();
        }

        #endregion

        #region Integration-like Tests

        [Fact]
        public void Employee_CompleteLifecycle_ShouldMaintainConsistency()
        {
            // Arrange - Create
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john.doe@example.com", "password", _validBirthday, 1L);

            // Act - Update properties
            employee.UpdateFirstName("Jonathan");
            employee.UpdateLastName("Doe-Smith");
            employee.UpdateEmail("jonathan.doesmith@example.com");
            employee.UpdatePassword("newPassword");
            employee.UpdateRole(2L);
            employee.AssignManager(5L);

            // Add phones
            var phone1 = EmployeePhoneModel.Create(0, "11987654321");
            var phone2 = EmployeePhoneModel.Create(0, "1133334444");
            employee.AddPhone(phone1);
            employee.AddPhone(phone2);

            // Assert
            employee.FirstName.Should().Be("Jonathan");
            employee.LastName.Should().Be("Doe-Smith");
            employee.GetFullName().Should().Be("Jonathan Doe-Smith");
            employee.Email.Should().Be("jonathan.doesmith@example.com");
            employee.Password.Should().Be("newPassword");
            employee.RoleId.Should().Be(2);
            employee.ManagerId.Should().Be(5);
            employee.HasManager().Should().BeTrue();
            employee.Phones.Should().HaveCount(2);
        }

        [Fact]
        public void Employee_WithMultiplePhones_ShouldManageCorrectly()
        {
            // Arrange
            var employee = EmployeeModel.Load(
                1L, "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", _validBirthday, 1L, null);

            var phone1 = EmployeePhoneModel.Create(1L, "11987654321");
            var phone2 = EmployeePhoneModel.Create(1L, "1133334444");
            var phone3 = EmployeePhoneModel.Create(1L, "11988887777");

            // Act - Add phones
            employee.AddPhone(phone1);
            employee.AddPhone(phone2);
            employee.AddPhone(phone3);

            // Assert - Should have 3 phones
            employee.Phones.Should().HaveCount(3);
            employee.Phones.Should().Contain(p => p.Phone == "11987654321");
            employee.Phones.Should().Contain(p => p.Phone == "1133334444");
            employee.Phones.Should().Contain(p => p.Phone == "11988887777");
        }

        [Theory]
        [InlineData(18, true, false)]
        [InlineData(25, true, false)]
        [InlineData(65, true, false)]
        [InlineData(120, true, false)]
        public void Employee_WithDifferentAges_ShouldCalculateCorrectly(int years, bool shouldBeAdult, bool shouldBeMinor)
        {
            // Arrange
            var birthday = DateTime.Today.AddYears(-years);
            var employee = EmployeeModel.Create(
                "John", "Doe", "123.456.789-01",
                "john@example.com", "pass", birthday, 1L);

            // Act & Assert
            employee.GetAge().Should().Be(years);
            employee.IsAdult().Should().Be(shouldBeAdult);
            employee.IsMinor().Should().Be(shouldBeMinor);
        }

        #endregion
    }
}
