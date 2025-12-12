using FluentAssertions;
using MoutsTI.Domain.Entities;

namespace MoutsTI.Tests.Domain.Entities
{
    public class EmployeePhoneModelTests
    {
        #region Create Tests

        [Fact]
        public void Create_WithValidParameters_ShouldCreateInstance()
        {
            // Arrange
            var employeeId = 1L;
            var phone = "(11) 98765-4321";

            // Act
            var employeePhone = EmployeePhoneModel.Create(employeeId, phone);

            // Assert
            employeePhone.Should().NotBeNull();
            employeePhone.EmployeeId.Should().Be(1);
            employeePhone.Phone.Should().Be("(11) 98765-4321");
            employeePhone.PhoneId.Should().Be(0);
        }

        [Fact]
        public void Create_WithMobilePhone_ShouldCreateInstance()
        {
            // Arrange
            var employeeId = 1L;
            var phone = "11987654321";

            // Act
            var employeePhone = EmployeePhoneModel.Create(employeeId, phone);

            // Assert
            employeePhone.Should().NotBeNull();
            employeePhone.Phone.Should().Be("11987654321");
        }

        [Fact]
        public void Create_WithLandlinePhone_ShouldCreateInstance()
        {
            // Arrange
            var employeeId = 1L;
            var phone = "1133334444";

            // Act
            var employeePhone = EmployeePhoneModel.Create(employeeId, phone);

            // Assert
            employeePhone.Should().NotBeNull();
            employeePhone.Phone.Should().Be("1133334444");
        }

        [Fact]
        public void Create_WithInternationalPhone_ShouldCreateInstance()
        {
            // Arrange
            var employeeId = 1L;
            var phone = "+55 11 98765-4321";

            // Act
            var employeePhone = EmployeePhoneModel.Create(employeeId, phone);

            // Assert
            employeePhone.Should().NotBeNull();
            employeePhone.Phone.Should().Be("+55 11 98765-4321");
        }

        [Fact]
        public void Create_WithFormattedPhone_ShouldCreateInstance()
        {
            // Arrange
            var employeeId = 1L;
            var phone = "(11) 3333-4444";

            // Act
            var employeePhone = EmployeePhoneModel.Create(employeeId, phone);

            // Assert
            employeePhone.Should().NotBeNull();
            employeePhone.Phone.Should().Be("(11) 3333-4444");
        }

        [Fact]
        public void Create_WithEmptyPhone_ShouldThrowArgumentException()
        {
            // Arrange
            var employeeId = 1L;
            var phone = "";

            // Act
            Action act = () => EmployeePhoneModel.Create(employeeId, phone);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Phone number cannot be empty*")
                .WithParameterName("phone");
        }

        [Fact]
        public void Create_WithWhitespacePhone_ShouldThrowArgumentException()
        {
            // Arrange
            var employeeId = 1L;
            var phone = "   ";

            // Act
            Action act = () => EmployeePhoneModel.Create(employeeId, phone);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Phone number cannot be empty*")
                .WithParameterName("phone");
        }

        [Fact]
        public void Create_WithNullPhone_ShouldThrowArgumentException()
        {
            // Arrange
            var employeeId = 1L;
            string phone = null!;

            // Act
            Action act = () => EmployeePhoneModel.Create(employeeId, phone);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Phone number cannot be empty*")
                .WithParameterName("phone");
        }

        [Fact]
        public void Create_WithPhoneLessThan8Digits_ShouldThrowArgumentException()
        {
            // Arrange
            var employeeId = 1L;
            var phone = "1234567";

            // Act
            Action act = () => EmployeePhoneModel.Create(employeeId, phone);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Phone number must have at least 8 digits*")
                .WithParameterName("phone");
        }

        [Fact]
        public void Create_WithPhoneExceeding25Characters_ShouldThrowArgumentException()
        {
            // Arrange
            var employeeId = 1L;
            var phone = "+55 (11) 98765-4321 ext 12345";

            // Act
            Action act = () => EmployeePhoneModel.Create(employeeId, phone);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Phone number cannot exceed 25 characters*")
                .WithParameterName("phone");
        }

        [Fact]
        public void Create_WithPhoneExactly25Characters_ShouldCreateInstance()
        {
            // Arrange
            var employeeId = 1L;
            var phone = "+55 (11) 98765-4321 12345"; // Exactly 25 characters

            // Act
            var employeePhone = EmployeePhoneModel.Create(employeeId, phone);

            // Assert
            employeePhone.Should().NotBeNull();
            employeePhone.Phone.Should().HaveLength(25);
        }

        [Fact]
        public void Create_WithInvalidCharacters_ShouldThrowArgumentException()
        {
            // Arrange
            var employeeId = 1L;
            var phone = "(11) 98765-4321#";

            // Act
            Action act = () => EmployeePhoneModel.Create(employeeId, phone);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Phone number contains invalid characters*")
                .WithParameterName("phone");
        }

        [Fact]
        public void Create_WithLetters_ShouldThrowArgumentException()
        {
            // Arrange
            var employeeId = 1L;
            var phone = "11-9876-ABCD";

            // Act
            Action act = () => EmployeePhoneModel.Create(employeeId, phone);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Phone number contains invalid characters*")
                .WithParameterName("phone");
        }

        [Fact]
        public void Create_WithZeroEmployeeId_ShouldCreateInstance()
        {
            // Arrange
            var employeeId = 0L;
            var phone = "11987654321";

            // Act
            var employeePhone = EmployeePhoneModel.Create(employeeId, phone);

            // Assert
            employeePhone.Should().NotBeNull();
            employeePhone.EmployeeId.Should().Be(0);
        }

        [Fact]
        public void Create_WithNegativeEmployeeId_ShouldCreateInstance()
        {
            // Arrange
            var employeeId = -1L;
            var phone = "11987654321";

            // Act
            var employeePhone = EmployeePhoneModel.Create(employeeId, phone);

            // Assert
            employeePhone.Should().NotBeNull();
            employeePhone.EmployeeId.Should().Be(-1);
        }

        [Fact]
        public void Create_ShouldTrimWhitespace()
        {
            // Arrange
            var employeeId = 1L;
            var phone = "  11987654321  ";

            // Act
            var employeePhone = EmployeePhoneModel.Create(employeeId, phone);

            // Assert
            employeePhone.Phone.Should().Be("11987654321");
        }

        #endregion

        #region Load Tests

        [Fact]
        public void Load_WithValidParameters_ShouldCreateInstance()
        {
            // Arrange
            var phoneId = 1L;
            var employeeId = 1L;
            var phone = "(11) 98765-4321";

            // Act
            var employeePhone = EmployeePhoneModel.Load(phoneId, employeeId, phone);

            // Assert
            employeePhone.Should().NotBeNull();
            employeePhone.PhoneId.Should().Be(1);
            employeePhone.EmployeeId.Should().Be(1);
            employeePhone.Phone.Should().Be("(11) 98765-4321");
        }

        [Fact]
        public void Load_WithZeroPhoneId_ShouldCreateInstance()
        {
            // Arrange
            var phoneId = 0L;
            var employeeId = 1L;
            var phone = "11987654321";

            // Act
            var employeePhone = EmployeePhoneModel.Load(phoneId, employeeId, phone);

            // Assert
            employeePhone.Should().NotBeNull();
            employeePhone.PhoneId.Should().Be(0);
        }

        [Fact]
        public void Load_WithNegativePhoneId_ShouldCreateInstance()
        {
            // Arrange
            var phoneId = -1L;
            var employeeId = 1L;
            var phone = "11987654321";

            // Act
            var employeePhone = EmployeePhoneModel.Load(phoneId, employeeId, phone);

            // Assert
            employeePhone.Should().NotBeNull();
            employeePhone.PhoneId.Should().Be(-1);
        }

        [Fact]
        public void Load_WithInvalidPhone_ShouldThrowArgumentException()
        {
            // Arrange
            var phoneId = 1L;
            var employeeId = 1L;
            var phone = "123";

            // Act
            Action act = () => EmployeePhoneModel.Load(phoneId, employeeId, phone);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Phone number must have at least 8 digits*")
                .WithParameterName("phone");
        }

        [Fact]
        public void Load_WithMultiplePhones_ShouldCreateAllInstances()
        {
            // Arrange & Act
            var phones = new[]
            {
                EmployeePhoneModel.Load(1, 1, "11987654321"),
                EmployeePhoneModel.Load(2, 1, "(11) 3333-4444"),
                EmployeePhoneModel.Load(3, 1, "+55 11 98765-4321")
            };

            // Assert
            phones.Should().HaveCount(3);
            phones.Should().OnlyContain(p => p != null);
            phones.Should().OnlyContain(p => p.EmployeeId == 1);
        }

        #endregion

        #region UpdatePhone Tests

        [Fact]
        public void UpdatePhone_WithValidPhone_ShouldUpdatePhone()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "11987654321");
            var newPhone = "(11) 98765-4321";

            // Act
            employeePhone.UpdatePhone(newPhone);

            // Assert
            employeePhone.Phone.Should().Be("(11) 98765-4321");
        }

        [Fact]
        public void UpdatePhone_WithEmptyPhone_ShouldThrowArgumentException()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "11987654321");

            // Act
            Action act = () => employeePhone.UpdatePhone("");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Phone number cannot be empty*")
                .WithParameterName("phone");
        }

        [Fact]
        public void UpdatePhone_WithNullPhone_ShouldThrowArgumentException()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "11987654321");

            // Act
            Action act = () => employeePhone.UpdatePhone(null!);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Phone number cannot be empty*")
                .WithParameterName("phone");
        }

        [Fact]
        public void UpdatePhone_WithInvalidPhone_ShouldThrowArgumentException()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "11987654321");

            // Act
            Action act = () => employeePhone.UpdatePhone("123");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Phone number must have at least 8 digits*")
                .WithParameterName("phone");
        }

        [Fact]
        public void UpdatePhone_MultipleTimes_ShouldKeepLastValue()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "11987654321");

            // Act
            employeePhone.UpdatePhone("1133334444");
            employeePhone.UpdatePhone("11988887777");
            employeePhone.UpdatePhone("(11) 99999-9999");

            // Assert
            employeePhone.Phone.Should().Be("(11) 99999-9999");
        }

        [Fact]
        public void UpdatePhone_ShouldTrimWhitespace()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "11987654321");

            // Act
            employeePhone.UpdatePhone("  1133334444  ");

            // Assert
            employeePhone.Phone.Should().Be("1133334444");
        }

        #endregion

        #region GetDigitsOnly Tests

        [Fact]
        public void GetDigitsOnly_WithFormattedPhone_ShouldReturnOnlyDigits()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "(11) 98765-4321");

            // Act
            var digits = employeePhone.GetDigitsOnly();

            // Assert
            digits.Should().Be("11987654321");
        }

        [Fact]
        public void GetDigitsOnly_WithInternationalPhone_ShouldReturnOnlyDigits()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "+55 11 98765-4321");

            // Act
            var digits = employeePhone.GetDigitsOnly();

            // Assert
            digits.Should().Be("5511987654321");
        }

        [Fact]
        public void GetDigitsOnly_WithUnformattedPhone_ShouldReturnSameDigits()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "11987654321");

            // Act
            var digits = employeePhone.GetDigitsOnly();

            // Assert
            digits.Should().Be("11987654321");
        }

        [Fact]
        public void GetDigitsOnly_WithLandline_ShouldReturnOnlyDigits()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "(11) 3333-4444");

            // Act
            var digits = employeePhone.GetDigitsOnly();

            // Assert
            digits.Should().Be("1133334444");
        }

        [Fact]
        public void GetDigitsOnly_ShouldRemoveAllNonDigitCharacters()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "+55 (11) 98765-4321");

            // Act
            var digits = employeePhone.GetDigitsOnly();

            // Assert
            digits.Should().NotBeNullOrEmpty();
            digits.All(c => char.IsDigit(c)).Should().BeTrue();
        }

        #endregion

        #region IsMobilePhone Tests

        [Fact]
        public void IsMobilePhone_WithBrazilianMobile_ShouldReturnTrue()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "11987654321");

            // Act
            var isMobile = employeePhone.IsMobilePhone();

            // Assert
            isMobile.Should().BeTrue();
        }

        [Fact]
        public void IsMobilePhone_WithFormattedBrazilianMobile_ShouldReturnTrue()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "(11) 98765-4321");

            // Act
            var isMobile = employeePhone.IsMobilePhone();

            // Assert
            isMobile.Should().BeTrue();
        }

        [Fact]
        public void IsMobilePhone_WithDifferentAreaCode_ShouldReturnTrue()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "(21) 99876-5432");

            // Act
            var isMobile = employeePhone.IsMobilePhone();

            // Assert
            isMobile.Should().BeTrue();
        }

        [Fact]
        public void IsMobilePhone_WithLandline_ShouldReturnFalse()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "1133334444");

            // Act
            var isMobile = employeePhone.IsMobilePhone();

            // Assert
            isMobile.Should().BeFalse();
        }

        [Fact]
        public void IsMobilePhone_WithInternationalFormat_ShouldReturnFalse()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "+55 11 98765-4321");

            // Act
            var isMobile = employeePhone.IsMobilePhone();

            // Assert
            isMobile.Should().BeFalse(); // 13 digits with country code
        }

        [Fact]
        public void IsMobilePhone_WithLessThan11Digits_ShouldReturnFalse()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "1198765432");

            // Act
            var isMobile = employeePhone.IsMobilePhone();

            // Assert
            isMobile.Should().BeFalse();
        }

        [Fact]
        public void IsMobilePhone_WithMoreThan11Digits_ShouldReturnFalse()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "+55 11 987654321");

            // Act
            var isMobile = employeePhone.IsMobilePhone();

            // Assert
            isMobile.Should().BeFalse();
        }

        [Fact]
        public void IsMobilePhone_With11DigitsButNotStartingWith9_ShouldReturnFalse()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "11812345678");

            // Act
            var isMobile = employeePhone.IsMobilePhone();

            // Assert
            isMobile.Should().BeFalse();
        }

        [Theory]
        [InlineData("11987654321")]
        [InlineData("21987654321")]
        [InlineData("85987654321")]
        [InlineData("47987654321")]
        public void IsMobilePhone_WithValidBrazilianMobiles_ShouldReturnTrue(string phone)
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, phone);

            // Act
            var isMobile = employeePhone.IsMobilePhone();

            // Assert
            isMobile.Should().BeTrue();
        }

        #endregion

        #region IsLandline Tests

        [Fact]
        public void IsLandline_WithBrazilianLandline_ShouldReturnTrue()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "1133334444");

            // Act
            var isLandline = employeePhone.IsLandline();

            // Assert
            isLandline.Should().BeTrue();
        }

        [Fact]
        public void IsLandline_WithFormattedLandline_ShouldReturnTrue()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "(11) 3333-4444");

            // Act
            var isLandline = employeePhone.IsLandline();

            // Assert
            isLandline.Should().BeTrue();
        }

        [Fact]
        public void IsLandline_WithDifferentAreaCode_ShouldReturnTrue()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "(21) 2222-3333");

            // Act
            var isLandline = employeePhone.IsLandline();

            // Assert
            isLandline.Should().BeTrue();
        }

        [Fact]
        public void IsLandline_WithMobilePhone_ShouldReturnFalse()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "11987654321");

            // Act
            var isLandline = employeePhone.IsLandline();

            // Assert
            isLandline.Should().BeFalse();
        }

        [Fact]
        public void IsLandline_WithInternationalFormat_ShouldReturnFalse()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "+55 11 3333-4444");

            // Act
            var isLandline = employeePhone.IsLandline();

            // Assert
            isLandline.Should().BeFalse(); // 12 digits with country code
        }

        [Fact]
        public void IsLandline_WithLessThan10Digits_ShouldReturnFalse()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "113333444");

            // Act
            var isLandline = employeePhone.IsLandline();

            // Assert
            isLandline.Should().BeFalse();
        }

        [Fact]
        public void IsLandline_WithMoreThan10Digits_ShouldReturnFalse()
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, "11987654321");

            // Act
            var isLandline = employeePhone.IsLandline();

            // Assert
            isLandline.Should().BeFalse();
        }

        [Theory]
        [InlineData("1133334444")]
        [InlineData("2122223333")]
        [InlineData("8533334444")]
        [InlineData("4733334444")]
        public void IsLandline_WithValidBrazilianLandlines_ShouldReturnTrue(string phone)
        {
            // Arrange
            var employeePhone = EmployeePhoneModel.Create(1, phone);

            // Act
            var isLandline = employeePhone.IsLandline();

            // Assert
            isLandline.Should().BeTrue();
        }

        #endregion

        #region Equals Tests

        [Fact]
        public void Equals_WithSamePhoneId_ShouldReturnTrue()
        {
            // Arrange
            var phone1 = EmployeePhoneModel.Load(1, 1, "11987654321");
            var phone2 = EmployeePhoneModel.Load(1, 2, "1133334444");

            // Act
            var areEqual = phone1.Equals(phone2);

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentPhoneId_ShouldReturnFalse()
        {
            // Arrange
            var phone1 = EmployeePhoneModel.Load(1, 1, "11987654321");
            var phone2 = EmployeePhoneModel.Load(2, 1, "11987654321");

            // Act
            var areEqual = phone1.Equals(phone2);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var phone = EmployeePhoneModel.Load(1, 1, "11987654321");

            // Act
            var areEqual = phone.Equals(null);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void Equals_WithDifferentType_ShouldReturnFalse()
        {
            // Arrange
            var phone = EmployeePhoneModel.Load(1, 1, "11987654321");
            var other = new object();

            // Act
            var areEqual = phone.Equals(other);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void Equals_WithSameInstance_ShouldReturnTrue()
        {
            // Arrange
            var phone = EmployeePhoneModel.Load(1, 1, "11987654321");

            // Act
            var areEqual = phone.Equals(phone);

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void Equals_WithBothPhoneIdZero_ShouldReturnTrue()
        {
            // Arrange
            var phone1 = EmployeePhoneModel.Create(1, "11987654321");
            var phone2 = EmployeePhoneModel.Create(1, "1133334444");

            // Act
            var areEqual = phone1.Equals(phone2);

            // Assert
            areEqual.Should().BeTrue(); // Both have PhoneId = 0
        }

        #endregion

        #region GetHashCode Tests

        [Fact]
        public void GetHashCode_WithSamePhoneId_ShouldReturnSameHashCode()
        {
            // Arrange
            var phone1 = EmployeePhoneModel.Load(1, 1, "11987654321");
            var phone2 = EmployeePhoneModel.Load(1, 2, "1133334444");

            // Act
            var hash1 = phone1.GetHashCode();
            var hash2 = phone2.GetHashCode();

            // Assert
            hash1.Should().Be(hash2);
        }

        [Fact]
        public void GetHashCode_WithDifferentPhoneId_ShouldReturnDifferentHashCode()
        {
            // Arrange
            var phone1 = EmployeePhoneModel.Load(1, 1, "11987654321");
            var phone2 = EmployeePhoneModel.Load(2, 1, "11987654321");

            // Act
            var hash1 = phone1.GetHashCode();
            var hash2 = phone2.GetHashCode();

            // Assert
            hash1.Should().NotBe(hash2);
        }

        [Fact]
        public void GetHashCode_ShouldBeConsistent()
        {
            // Arrange
            var phone = EmployeePhoneModel.Load(1, 1, "11987654321");

            // Act
            var hash1 = phone.GetHashCode();
            var hash2 = phone.GetHashCode();
            var hash3 = phone.GetHashCode();

            // Assert
            hash1.Should().Be(hash2);
            hash2.Should().Be(hash3);
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_ShouldReturnPhoneNumber()
        {
            // Arrange
            var phone = EmployeePhoneModel.Create(1, "(11) 98765-4321");

            // Act
            var result = phone.ToString();

            // Assert
            result.Should().Be("(11) 98765-4321");
        }

        [Fact]
        public void ToString_WithUnformattedPhone_ShouldReturnPhoneNumber()
        {
            // Arrange
            var phone = EmployeePhoneModel.Create(1, "11987654321");

            // Act
            var result = phone.ToString();

            // Assert
            result.Should().Be("11987654321");
        }

        [Fact]
        public void ToString_WithInternationalPhone_ShouldReturnPhoneNumber()
        {
            // Arrange
            var phone = EmployeePhoneModel.Create(1, "+55 11 98765-4321");

            // Act
            var result = phone.ToString();

            // Assert
            result.Should().Be("+55 11 98765-4321");
        }

        [Fact]
        public void ToString_AfterUpdate_ShouldReturnUpdatedPhoneNumber()
        {
            // Arrange
            var phone = EmployeePhoneModel.Create(1, "11987654321");

            // Act
            phone.UpdatePhone("(11) 3333-4444");
            var result = phone.ToString();

            // Assert
            result.Should().Be("(11) 3333-4444");
        }

        #endregion

        #region Business Logic Tests

        [Fact]
        public void Phone_ShouldMaintainImmutablePhoneId()
        {
            // Arrange
            var phone = EmployeePhoneModel.Load(1, 1, "11987654321");
            var originalPhoneId = phone.PhoneId;

            // Act
            phone.UpdatePhone("1133334444");

            // Assert
            phone.PhoneId.Should().Be(originalPhoneId);
        }

        [Fact]
        public void Phone_ShouldMaintainImmutableEmployeeId()
        {
            // Arrange
            var phone = EmployeePhoneModel.Load(1, 1, "11987654321");
            var originalEmployeeId = phone.EmployeeId;

            // Act
            phone.UpdatePhone("1133334444");

            // Assert
            phone.EmployeeId.Should().Be(originalEmployeeId);
        }

        [Fact]
        public void Phone_CreatedWithCreate_ShouldHaveZeroPhoneId()
        {
            // Arrange & Act
            var phone = EmployeePhoneModel.Create(1, "11987654321");

            // Assert
            phone.PhoneId.Should().Be(0);
        }

        [Fact]
        public void Phone_LoadedWithLoad_ShouldPreservePhoneId()
        {
            // Arrange & Act
            var phone = EmployeePhoneModel.Load(42, 1, "11987654321");

            // Assert
            phone.PhoneId.Should().Be(42);
        }

        [Fact]
        public void Phone_AfterMultipleUpdates_ShouldMaintainConsistency()
        {
            // Arrange
            var phone = EmployeePhoneModel.Load(1, 1, "11987654321");

            // Act
            phone.UpdatePhone("1133334444");
            phone.UpdatePhone("11988887777");
            phone.UpdatePhone("(11) 99999-9999");

            // Assert
            phone.PhoneId.Should().Be(1);
            phone.EmployeeId.Should().Be(1);
            phone.Phone.Should().Be("(11) 99999-9999");
        }

        [Fact]
        public void Phone_MobileAndLandline_ShouldBeMutuallyExclusive()
        {
            // Arrange
            var mobilePhone = EmployeePhoneModel.Create(1, "11987654321");
            var landlinePhone = EmployeePhoneModel.Create(1, "1133334444");

            // Act & Assert
            mobilePhone.IsMobilePhone().Should().BeTrue();
            mobilePhone.IsLandline().Should().BeFalse();

            landlinePhone.IsMobilePhone().Should().BeFalse();
            landlinePhone.IsLandline().Should().BeTrue();
        }

        [Fact]
        public void Phone_WithInternationalFormat_ShouldNotBeClassifiedAsMobileOrLandline()
        {
            // Arrange
            var phone = EmployeePhoneModel.Create(1, "+55 11 98765-4321");

            // Act & Assert
            phone.IsMobilePhone().Should().BeFalse();
            phone.IsLandline().Should().BeFalse();
        }

        [Theory]
        [InlineData("(11) 98765-4321", "11987654321")]
        [InlineData("11 98765-4321", "11987654321")]
        [InlineData("+55 11 98765-4321", "5511987654321")]
        [InlineData("(11) 3333-4444", "1133334444")]
        public void GetDigitsOnly_WithVariousFormats_ShouldExtractDigitsCorrectly(string inputPhone, string expectedDigits)
        {
            // Arrange
            var phone = EmployeePhoneModel.Create(1, inputPhone);

            // Act
            var digits = phone.GetDigitsOnly();

            // Assert
            digits.Should().Be(expectedDigits);
        }

        [Fact]
        public void Phone_WithMinimumValidLength_ShouldBeCreated()
        {
            // Arrange & Act
            var phone = EmployeePhoneModel.Create(1, "33334444"); // 8 digits minimum

            // Assert
            phone.Should().NotBeNull();
            phone.GetDigitsOnly().Should().HaveLength(8);
        }

        #endregion

        #region Collection Tests

        [Fact]
        public void Phones_InList_CanBeComparedByPhoneId()
        {
            // Arrange
            var phones = new List<EmployeePhoneModel>
            {
                EmployeePhoneModel.Load(3, 1, "11988887777"),
                EmployeePhoneModel.Load(1, 1, "11987654321"),
                EmployeePhoneModel.Load(2, 1, "1133334444")
            };

            // Act
            var sortedPhones = phones.OrderBy(p => p.PhoneId).ToList();

            // Assert
            sortedPhones[0].PhoneId.Should().Be(1);
            sortedPhones[1].PhoneId.Should().Be(2);
            sortedPhones[2].PhoneId.Should().Be(3);
        }

        [Fact]
        public void Phones_InHashSet_ShouldHandleDuplicatesByPhoneId()
        {
            // Arrange
            var phone1 = EmployeePhoneModel.Load(1, 1, "11987654321");
            var phone2 = EmployeePhoneModel.Load(1, 1, "1133334444");
            var hashSet = new HashSet<EmployeePhoneModel>();

            // Act
            hashSet.Add(phone1);
            hashSet.Add(phone2);

            // Assert
            hashSet.Should().HaveCount(1); // Same PhoneId, should be treated as duplicate
        }

        [Fact]
        public void Phones_ForEmployee_CanBeGrouped()
        {
            // Arrange
            var phones = new List<EmployeePhoneModel>
            {
                EmployeePhoneModel.Load(1, 1, "11987654321"),
                EmployeePhoneModel.Load(2, 1, "1133334444"),
                EmployeePhoneModel.Load(3, 2, "21987654321"),
                EmployeePhoneModel.Load(4, 2, "2133334444")
            };

            // Act
            var groupedPhones = phones.GroupBy(p => p.EmployeeId).ToDictionary(g => g.Key, g => g.ToList());

            // Assert
            groupedPhones.Should().HaveCount(2);
            groupedPhones[1].Should().HaveCount(2);
            groupedPhones[2].Should().HaveCount(2);
        }

        [Fact]
        public void Phones_CanBeSeparatedByType()
        {
            // Arrange
            var phones = new List<EmployeePhoneModel>
            {
                EmployeePhoneModel.Load(1, 1, "11987654321"),
                EmployeePhoneModel.Load(2, 1, "1133334444"),
                EmployeePhoneModel.Load(3, 1, "11988887777"),
                EmployeePhoneModel.Load(4, 1, "1144445555")
            };

            // Act
            var mobilePhones = phones.Where(p => p.IsMobilePhone()).ToList();
            var landlinePhones = phones.Where(p => p.IsLandline()).ToList();

            // Assert
            mobilePhones.Should().HaveCount(2);
            landlinePhones.Should().HaveCount(2);
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public void Phone_With8Digits_ShouldBeValid()
        {
            // Arrange & Act
            var phone = EmployeePhoneModel.Create(1, "33334444");

            // Assert
            phone.Should().NotBeNull();
            phone.GetDigitsOnly().Should().HaveLength(8);
        }

        [Fact]
        public void Phone_With25Characters_ShouldBeValid()
        {
            // Arrange & Act
            var phone = EmployeePhoneModel.Create(1, "+55 (11) 98765-4321 12345");

            // Assert
            phone.Should().NotBeNull();
            phone.Phone.Should().HaveLength(25);
        }

        [Fact]
        public void Phone_WithOnlySpacesAndHyphens_ShouldBeValid()
        {
            // Arrange & Act
            var phone = EmployeePhoneModel.Create(1, "11 98765 4321");

            // Assert
            phone.Should().NotBeNull();
        }

        [Fact]
        public void Phone_WithParentheses_ShouldBeValid()
        {
            // Arrange & Act
            var phone = EmployeePhoneModel.Create(1, "(11)987654321");

            // Assert
            phone.Should().NotBeNull();
        }

        [Fact]
        public void Phone_WithPlusSign_ShouldBeValid()
        {
            // Arrange & Act
            var phone = EmployeePhoneModel.Create(1, "+5511987654321");

            // Assert
            phone.Should().NotBeNull();
        }

        [Theory]
        [InlineData("(11) 98765-4321")]
        [InlineData("11 98765-4321")]
        [InlineData("11-98765-4321")]
        [InlineData("+55 11 98765-4321")]
        [InlineData("+55 (11) 98765-4321")]
        [InlineData("11987654321")]
        public void Phone_WithVariousValidFormats_ShouldBeCreated(string phoneNumber)
        {
            // Arrange & Act
            var phone = EmployeePhoneModel.Create(1, phoneNumber);

            // Assert
            phone.Should().NotBeNull();
            phone.Phone.Should().Be(phoneNumber);
        }

        [Theory]
        [InlineData("123")]
        [InlineData("1234567")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("11-9876-ABCD")]
        [InlineData("(11) 98765-4321#")]
        [InlineData("11.98765.4321")]
        [InlineData("11*98765*4321")]
        public void Phone_WithInvalidFormats_ShouldThrowException(string phoneNumber)
        {
            // Arrange & Act
            Action act = () => EmployeePhoneModel.Create(1, phoneNumber);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        #endregion
    }
}
