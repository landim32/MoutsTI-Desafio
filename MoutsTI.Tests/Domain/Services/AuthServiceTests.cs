using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using MoutsTI.Domain.Entities;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Domain.Services;
using MoutsTI.Infra.Interfaces.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MoutsTI.Tests.Domain.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IEmployeeRepository<IEmployeeModel>> _mockEmployeeRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<AuthService>> _mockLogger;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockEmployeeRepository = new Mock<IEmployeeRepository<IEmployeeModel>>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<AuthService>>();

            // Configuração padrão do IConfiguration
            var mockJwtSection = new Mock<IConfigurationSection>();
            mockJwtSection.Setup(x => x["SecretKey"]).Returns("test-secret-key-with-at-least-32-characters-for-testing");
            mockJwtSection.Setup(x => x["Issuer"]).Returns("MoutsTI.API");
            mockJwtSection.Setup(x => x["Audience"]).Returns("MoutsTI.Client");
            mockJwtSection.Setup(x => x["ExpirationMinutes"]).Returns("60");

            _mockConfiguration.Setup(x => x.GetSection("JwtSettings")).Returns(mockJwtSection.Object);

            _authService = new AuthService(_mockEmployeeRepository.Object, _mockConfiguration.Object, _mockLogger.Object);
        }

        #region AuthenticateAsync Tests

        [Fact]
        public async Task AuthenticateAsync_WithValidCredentials_ShouldReturnEmployee()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password123";
            var hashedPassword = _authService.HashPassword(password);

            var employee = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "12345678901",
                Email = email,
                Password = hashedPassword,
                Birthday = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                RoleId = 1L
            });

            _mockEmployeeRepository
                .Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(employee);

            // Act
            var result = await _authService.AuthenticateAsync(email, password);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(employee);
            result!.Email.Should().Be(email);
            _mockEmployeeRepository.Verify(x => x.GetByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task AuthenticateAsync_WithInvalidEmail_ShouldReturnNull()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var password = "password123";

            _mockEmployeeRepository
                .Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync((IEmployeeModel?)null);

            // Act
            var result = await _authService.AuthenticateAsync(email, password);

            // Assert
            result.Should().BeNull();
            _mockEmployeeRepository.Verify(x => x.GetByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task AuthenticateAsync_WithInvalidPassword_ShouldReturnNull()
        {
            // Arrange
            var email = "test@example.com";
            var correctPassword = "correctPassword";
            var wrongPassword = "wrongPassword";
            var hashedPassword = _authService.HashPassword(correctPassword);

            var employee = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "12345678901",
                Email = email,
                Password = hashedPassword,
                Birthday = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                RoleId = 1L
            });

            _mockEmployeeRepository
                .Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(employee);

            // Act
            var result = await _authService.AuthenticateAsync(email, wrongPassword);

            // Assert
            result.Should().BeNull();
            _mockEmployeeRepository.Verify(x => x.GetByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task AuthenticateAsync_WithEmptyEmail_ShouldReturnNull()
        {
            // Arrange
            var email = string.Empty;
            var password = "password123";

            _mockEmployeeRepository
                .Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync((IEmployeeModel?)null);

            // Act
            var result = await _authService.AuthenticateAsync(email, password);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task AuthenticateAsync_WithEmptyPassword_ShouldReturnNull()
        {
            // Arrange
            var email = "test@example.com";
            var password = string.Empty;
            var hashedPassword = _authService.HashPassword("correctPassword");

            var employee = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "12345678901",
                Email = email,
                Password = hashedPassword,
                Birthday = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                RoleId = 1L
            });

            _mockEmployeeRepository
                .Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(employee);

            // Act
            var result = await _authService.AuthenticateAsync(email, password);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region GenerateJwtToken Tests

        [Fact]
        public void GenerateJwtToken_WithValidEmployee_ShouldReturnValidToken()
        {
            // Arrange
            var employee = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "12345678901",
                Email = "john.doe@example.com",
                Password = "hashedPassword",
                Birthday = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                RoleId = 2L
            });

            // Act
            var token = _authService.GenerateJwtToken(employee);

            // Assert
            token.Should().NotBeNullOrEmpty();
            
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            jwtToken.Should().NotBeNull();
            jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == "1");
            jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == "john.doe@example.com");
            jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.GivenName && c.Value == "John");
            jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.FamilyName && c.Value == "Doe");
            jwtToken.Claims.Should().Contain(c => c.Type == "RoleId" && c.Value == "2");
            jwtToken.Issuer.Should().Be("MoutsTI.API");
            jwtToken.Audiences.Should().Contain("MoutsTI.Client");
        }

        [Fact]
        public void GenerateJwtToken_ShouldHaveExpirationTime()
        {
            // Arrange
            var employee = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "12345678901",
                Email = "john.doe@example.com",
                Password = "hashedPassword",
                Birthday = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                RoleId = 2L
            });

            // Act
            var token = _authService.GenerateJwtToken(employee);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            jwtToken.ValidTo.Should().BeAfter(DateTime.UtcNow);
            jwtToken.ValidTo.Should().BeBefore(DateTime.UtcNow.AddMinutes(61));
        }

        [Fact]
        public void GenerateJwtToken_ShouldIncludeUniqueJti()
        {
            // Arrange
            var employee = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "12345678901",
                Email = "john.doe@example.com",
                Password = "hashedPassword",
                Birthday = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                RoleId = 2L
            });

            // Act
            var token1 = _authService.GenerateJwtToken(employee);
            var token2 = _authService.GenerateJwtToken(employee);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken1 = handler.ReadJwtToken(token1);
            var jwtToken2 = handler.ReadJwtToken(token2);

            var jti1 = jwtToken1.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
            var jti2 = jwtToken2.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;

            jti1.Should().NotBe(jti2);
        }

        [Fact]
        public void GenerateJwtToken_WithMissingSecretKey_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var employee = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters { EmployeeId = 1L, FirstName = "John", LastName = "Doe", DocNumber = "123.456.789-01", Email = "john.doe@example.com", Password = "hashedPassword", Birthday = DateTime.SpecifyKind(DateTime.Today.AddYears(-30), DateTimeKind.Utc), RoleId = 1L });

            var mockJwtSection = new Mock<IConfigurationSection>();
            mockJwtSection.Setup(x => x["SecretKey"]).Returns((string)null!);
            mockJwtSection.Setup(x => x["Issuer"]).Returns("MoutsTI.API");
            mockJwtSection.Setup(x => x["Audience"]).Returns("MoutsTI.Client");
            mockJwtSection.Setup(x => x["ExpirationMinutes"]).Returns("60");

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("JwtSettings")).Returns(mockJwtSection.Object);

            var mockLogger = new Mock<ILogger<AuthService>>();
            var authService = new AuthService(_mockEmployeeRepository.Object, mockConfig.Object, mockLogger.Object);

            // Act
            Action act = () => authService.GenerateJwtToken(employee);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*JWT SecretKey not configured*");
        }

        #endregion

        #region HashPassword Tests

        [Fact]
        public void HashPassword_ShouldReturnBase64String()
        {
            // Arrange
            var password = "mySecurePassword123";

            // Act
            var hashedPassword = _authService.HashPassword(password);

            // Assert
            hashedPassword.Should().NotBeNullOrEmpty();
            hashedPassword.Should().NotBe(password);
            
            // Verify it's a valid Base64 string
            var isBase64 = TryParseBase64(hashedPassword);
            isBase64.Should().BeTrue();
        }

        [Fact]
        public void HashPassword_WithSamePassword_ShouldReturnSameHash()
        {
            // Arrange
            var password = "myPassword";

            // Act
            var hash1 = _authService.HashPassword(password);
            var hash2 = _authService.HashPassword(password);

            // Assert
            hash1.Should().Be(hash2);
        }

        [Fact]
        public void HashPassword_WithDifferentPasswords_ShouldReturnDifferentHashes()
        {
            // Arrange
            var password1 = "password1";
            var password2 = "password2";

            // Act
            var hash1 = _authService.HashPassword(password1);
            var hash2 = _authService.HashPassword(password2);

            // Assert
            hash1.Should().NotBe(hash2);
        }

        [Fact]
        public void HashPassword_WithEmptyString_ShouldReturnHash()
        {
            // Arrange
            var password = string.Empty;

            // Act
            var hashedPassword = _authService.HashPassword(password);

            // Assert
            hashedPassword.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData("a")]
        [InlineData("short")]
        [InlineData("ThisIsAVeryLongPasswordWithLotsOfCharacters123456789!@#$%^&*()")]
        [InlineData("password with spaces")]
        [InlineData("pàssword-wõm-spëçiàl-çhârs")]
        public void HashPassword_WithVariousPasswords_ShouldReturnValidHash(string password)
        {
            // Act
            var hashedPassword = _authService.HashPassword(password);

            // Assert
            hashedPassword.Should().NotBeNullOrEmpty();
            hashedPassword.Should().NotBe(password);
        }

        #endregion

        #region Integration Tests

        [Fact]
        public async Task FullAuthenticationFlow_WithValidCredentials_ShouldSucceed()
        {
            // Arrange
            var email = "integration@test.com";
            var password = "integrationPassword";
            var hashedPassword = _authService.HashPassword(password);

            var employee = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
            {
                EmployeeId = 10L,
                FirstName = "Integration",
                LastName = "Test",
                DocNumber = "98765432109",
                Email = email,
                Password = hashedPassword,
                Birthday = new DateTime(1985, 5, 15, 0, 0, 0, DateTimeKind.Utc),
                RoleId = 3L,
                ManagerId = 5L
            });

            _mockEmployeeRepository
                .Setup(x => x.GetByEmailAsync(email))
                .ReturnsAsync(employee);

            // Act - Authenticate
            var authenticatedEmployee = await _authService.AuthenticateAsync(email, password);

            // Assert - Authentication
            authenticatedEmployee.Should().NotBeNull();
            authenticatedEmployee!.EmployeeId.Should().Be(10L);

            // Act - Generate Token
            var token = _authService.GenerateJwtToken(authenticatedEmployee);

            // Assert - Token
            token.Should().NotBeNullOrEmpty();

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == "10");
            jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == email);
            jwtToken.Claims.Should().Contain(c => c.Type == "RoleId" && c.Value == "3");
        }

        #endregion

        #region Helper Methods

        private static bool TryParseBase64(string base64String)
        {
            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}

