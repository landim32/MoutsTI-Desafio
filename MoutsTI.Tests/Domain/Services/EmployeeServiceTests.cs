using AutoMapper;
using FluentAssertions;
using Moq;
using MoutsTI.Domain.Entities;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Domain.Services;
using MoutsTI.Domain.Services.Interfaces;
using MoutsTI.Dtos;
using MoutsTI.Infra.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace MoutsTI.Tests.Domain.Services
{
    public class EmployeeServiceTests
    {
        private Mock<IEmployeeRepository<IEmployeeModel>> _mockRepository;
        private Mock<IEmployeeRoleRepository<IEmployeeRoleModel>> _mockRoleRepository;
        private Mock<IMapper> _mockMapper;
        private Mock<IAuthService> _mockAuthService;
        private Mock<ILogger<EmployeeService>> _mockLogger;
        private EmployeeService _service;

        public EmployeeServiceTests()
        {
            _mockRepository = new Mock<IEmployeeRepository<IEmployeeModel>>();
            _mockRoleRepository = new Mock<IEmployeeRoleRepository<IEmployeeRoleModel>>();
            _mockMapper = new Mock<IMapper>();
            _mockAuthService = new Mock<IAuthService>();
            _mockLogger = new Mock<ILogger<EmployeeService>>();

            _service = new EmployeeService(
                _mockRepository.Object,
                _mockRoleRepository.Object,
                _mockMapper.Object,
                _mockAuthService.Object,
                _mockLogger.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithNullRepository_ShouldThrowArgumentNullException()
        {
            // Act
            Action act = () => new EmployeeService(
                null!,
                _mockRoleRepository.Object,
                _mockMapper.Object,
                _mockAuthService.Object,
                _mockLogger.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("repository");
        }

        [Fact]
        public void Constructor_WithNullRoleRepository_ShouldThrowArgumentNullException()
        {
            // Act
            Action act = () => new EmployeeService(
                _mockRepository.Object,
                null!,
                _mockMapper.Object,
                _mockAuthService.Object,
                _mockLogger.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("roleRepository");
        }

        [Fact]
        public void Constructor_WithNullMapper_ShouldThrowArgumentNullException()
        {
            // Act
            Action act = () => new EmployeeService(
                _mockRepository.Object,
                _mockRoleRepository.Object,
                null!,
                _mockAuthService.Object,
                _mockLogger.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("mapper");
        }

        [Fact]
        public void Constructor_WithNullAuthService_ShouldThrowArgumentNullException()
        {
            // Act
            Action act = () => new EmployeeService(
                _mockRepository.Object,
                _mockRoleRepository.Object,
                _mockMapper.Object,
                null!,
                _mockLogger.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("authService");
        }

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateInstance()
        {
            // Act
            var service = new EmployeeService(
                _mockRepository.Object,
                _mockRoleRepository.Object,
                _mockMapper.Object,
                _mockAuthService.Object,
                _mockLogger.Object);

            // Assert
            service.Should().NotBeNull();
        }

        #endregion

        #region Add Tests

        [Fact]
        public void Add_WithNullEmployee_ShouldThrowArgumentNullException()
        {
            // Arrange
            var currentEmployee = new EmployeeDto { EmployeeId = 1, RoleId = 6 };

            // Act
            Action act = () => _service.Add(null!, currentEmployee);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("employee");
        }

        [Fact]
        public void Add_WithNullCurrentEmployee_ShouldThrowArgumentNullException()
        {
            // Arrange
            var employee = new EmployeeDto { RoleId = 1 };

            // Act
            Action act = () => _service.Add(employee, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("currentEmployee");
        }

        [Fact]
        public void Add_WithValidEmployee_ShouldHashPasswordAndAdd()
        {
            // Arrange
            var employee = new EmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "plainPassword",
                DocNumber = "12345678901",
                Birthday = DateTime.Now.AddYears(-25),
                RoleId = 1
            };

            var currentEmployee = new EmployeeDto { EmployeeId = 1, RoleId = 6 };

            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, "Developer", 1),
                EmployeeRoleModel.Load(6, "Director", 6)
            };

            var employeeModel = EmployeeModel.Load(
                0,
                "John",
                "Doe",
                "12345678901",
                "john@example.com",
                "hashedPassword",
                DateTime.Now.AddYears(-25),
                1,
                null
            );

            _mockRoleRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            _mockAuthService
                .Setup(x => x.HashPassword("plainPassword"))
                .Returns("hashedPassword");

            _mockMapper
                .Setup(x => x.Map<IEmployeeModel>(It.IsAny<EmployeeDto>()))
                .Returns(employeeModel);

            _mockRepository
                .Setup(x => x.Add(It.IsAny<IEmployeeModel>()))
                .Returns(1);

            // Act
            var result = _service.Add(employee, currentEmployee);

            // Assert
            result.Should().Be(1);
            employee.Password.Should().Be("hashedPassword");
            _mockAuthService.Verify(x => x.HashPassword("plainPassword"), Times.Once);
            _mockRepository.Verify(x => x.Add(It.IsAny<IEmployeeModel>()), Times.Once);
        }

        [Fact]
        public void Add_WithHigherRoleLevel_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var employee = new EmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "password",
                RoleId = 6 // Director
            };

            var currentEmployee = new EmployeeDto { EmployeeId = 1, RoleId = 1 }; // Developer

            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, "Developer", 1),
                EmployeeRoleModel.Load(6, "Director", 6)
            };

            _mockRoleRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            // Act
            Action act = () => _service.Add(employee, currentEmployee);

            // Assert
            act.Should().Throw<UnauthorizedAccessException>()
                .WithMessage("*do not have permission*");
        }

        [Fact]
        public void Add_WithEmptyPassword_ShouldNotHashPassword()
        {
            // Arrange
            var employee = new EmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "",
                DocNumber = "12345678901",
                Birthday = DateTime.Now.AddYears(-25),
                RoleId = 1
            };

            var currentEmployee = new EmployeeDto { EmployeeId = 1, RoleId = 6 };

            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, "Developer", 1),
                EmployeeRoleModel.Load(6, "Director", 6)
            };

            var employeeModel = EmployeeModel.Load(
                0,
                "John",
                "Doe",
                "12345678901",
                "john@example.com",
                "",
                DateTime.Now.AddYears(-25),
                1,
                null
            );

            _mockRoleRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            _mockMapper
                .Setup(x => x.Map<IEmployeeModel>(It.IsAny<EmployeeDto>()))
                .Returns(employeeModel);

            _mockRepository
                .Setup(x => x.Add(It.IsAny<IEmployeeModel>()))
                .Returns(1);

            // Act
            _service.Add(employee, currentEmployee);

            // Assert
            _mockAuthService.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Add_WithSameRoleLevel_ShouldSucceed()
        {
            // Arrange
            var employee = new EmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "password",
                DocNumber = "12345678901",
                Birthday = DateTime.Now.AddYears(-25),
                RoleId = 3
            };

            var currentEmployee = new EmployeeDto { EmployeeId = 1, RoleId = 3 };

            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(3, "Senior", 3)
            };

            var employeeModel = EmployeeModel.Load(
                0,
                "John",
                "Doe",
                "12345678901",
                "john@example.com",
                "hashedPassword",
                DateTime.Now.AddYears(-25),
                3,
                null
            );

            _mockRoleRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            _mockAuthService
                .Setup(x => x.HashPassword("password"))
                .Returns("hashedPassword");

            _mockMapper
                .Setup(x => x.Map<IEmployeeModel>(It.IsAny<EmployeeDto>()))
                .Returns(employeeModel);

            _mockRepository
                .Setup(x => x.Add(It.IsAny<IEmployeeModel>()))
                .Returns(1);

            // Act
            var result = _service.Add(employee, currentEmployee);

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public void Add_WithNonExistentTargetRole_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var employee = new EmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "password",
                RoleId = 999 // Non-existent role
            };

            var currentEmployee = new EmployeeDto { EmployeeId = 1, RoleId = 6 };

            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(6, "Director", 6)
            };

            _mockRoleRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            // Act
            Action act = () => _service.Add(employee, currentEmployee);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*Target role with ID 999 not found*");
        }

        [Fact]
        public void Add_WithNonExistentCurrentRole_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var employee = new EmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "password",
                RoleId = 1
            };

            var currentEmployee = new EmployeeDto { EmployeeId = 1, RoleId = 999 };

            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, "Developer", 1)
            };

            _mockRoleRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            // Act
            Action act = () => _service.Add(employee, currentEmployee);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*Current employee role with ID 999 not found*");
        }

        #endregion

        #region Update Tests

        [Fact]
        public void Update_WithNullEmployee_ShouldThrowArgumentNullException()
        {
            // Arrange
            var currentEmployee = new EmployeeDto { EmployeeId = 1, RoleId = 6 };

            // Act
            Action act = () => _service.Update(null!, currentEmployee);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("employee");
        }

        [Fact]
        public void Update_WithNullCurrentEmployee_ShouldThrowArgumentNullException()
        {
            // Arrange
            var employee = new EmployeeDto { EmployeeId = 1, RoleId = 1 };

            // Act
            Action act = () => _service.Update(employee, null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("currentEmployee");
        }

        [Fact]
        public void Update_WithZeroOrNegativeEmployeeId_ShouldThrowArgumentException()
        {
            // Arrange
            var employee = new EmployeeDto { EmployeeId = 0, RoleId = 1 };
            var currentEmployee = new EmployeeDto { EmployeeId = 1, RoleId = 6 };

            // Act
            Action act = () => _service.Update(employee, currentEmployee);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("employee")
                .WithMessage("*Employee ID must be greater than zero*");
        }

        [Fact]
        public void Update_WithNonExistentEmployee_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var employee = new EmployeeDto
            {
                EmployeeId = 999,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                RoleId = 1
            };

            var currentEmployee = new EmployeeDto { EmployeeId = 1, RoleId = 6 };

            _mockRepository
                .Setup(x => x.GetById(999))
                .Returns((IEmployeeModel)null!);

            // Act
            Action act = () => _service.Update(employee, currentEmployee);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*Employee with ID 999 not found*");
        }

        [Fact]
        public void Update_WithValidEmployee_ShouldHashPasswordAndUpdate()
        {
            // Arrange
            var employee = new EmployeeDto
            {
                EmployeeId = 1,
                FirstName = "John",
                LastName = "Doe Updated",
                Email = "john@example.com",
                Password = "newPassword",
                DocNumber = "12345678901",
                Birthday = DateTime.Now.AddYears(-25),
                RoleId = 1
            };

            var currentEmployee = new EmployeeDto { EmployeeId = 2, RoleId = 6 };

            var existingEmployee = EmployeeModel.Load(
                1,
                "John",
                "Doe",
                "12345678901",
                "john@example.com",
                "oldHash",
                DateTime.Now.AddYears(-25),
                1,
                null
            );

            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, "Developer", 1),
                EmployeeRoleModel.Load(6, "Director", 6)
            };

            var updatedEmployeeModel = EmployeeModel.Load(
                1,
                "John",
                "Doe Updated",
                "12345678901",
                "john@example.com",
                "newHashedPassword",
                DateTime.Now.AddYears(-25),
                1,
                null
            );

            _mockRepository
                .Setup(x => x.GetById(1))
                .Returns(existingEmployee);

            _mockRoleRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            _mockAuthService
                .Setup(x => x.HashPassword("newPassword"))
                .Returns("newHashedPassword");

            _mockMapper
                .Setup(x => x.Map<IEmployeeModel>(It.IsAny<EmployeeDto>()))
                .Returns(updatedEmployeeModel);

            // Act
            _service.Update(employee, currentEmployee);

            // Assert
            employee.Password.Should().Be("newHashedPassword");
            _mockAuthService.Verify(x => x.HashPassword("newPassword"), Times.Once);
            _mockRepository.Verify(x => x.Update(It.IsAny<IEmployeeModel>()), Times.Once);
        }

        [Fact]
        public void Update_WithEmptyPassword_ShouldNotHashPassword()
        {
            // Arrange
            var employee = new EmployeeDto
            {
                EmployeeId = 1,
                FirstName = "John",
                LastName = "Doe Updated",
                Email = "john@example.com",
                Password = "",
                DocNumber = "12345678901",
                Birthday = DateTime.Now.AddYears(-25),
                RoleId = 1
            };

            var currentEmployee = new EmployeeDto { EmployeeId = 2, RoleId = 6 };

            var existingEmployee = EmployeeModel.Load(
                1,
                "John",
                "Doe",
                "12345678901",
                "john@example.com",
                "oldHash",
                DateTime.Now.AddYears(-25),
                1,
                null
            );

            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, "Developer", 1),
                EmployeeRoleModel.Load(6, "Director", 6)
            };

            var updatedEmployeeModel = EmployeeModel.Load(
                1,
                "John",
                "Doe Updated",
                "12345678901",
                "john@example.com",
                "",
                DateTime.Now.AddYears(-25),
                1,
                null
            );

            _mockRepository
                .Setup(x => x.GetById(1))
                .Returns(existingEmployee);

            _mockRoleRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            _mockMapper
                .Setup(x => x.Map<IEmployeeModel>(It.IsAny<EmployeeDto>()))
                .Returns(updatedEmployeeModel);

            // Act
            _service.Update(employee, currentEmployee);

            // Assert
            _mockAuthService.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
            _mockRepository.Verify(x => x.Update(It.IsAny<IEmployeeModel>()), Times.Once);
        }

        [Fact]
        public void Update_ChangingToHigherRole_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var employee = new EmployeeDto
            {
                EmployeeId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                RoleId = 6 // Trying to change to Director
            };

            var currentEmployee = new EmployeeDto { EmployeeId = 2, RoleId = 3 }; // Senior

            var existingEmployee = EmployeeModel.Load(
                1,
                "John",
                "Doe",
                "12345678901",
                "john@example.com",
                "hash",
                DateTime.Now.AddYears(-25),
                1,
                null
            );

            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, "Developer", 1),
                EmployeeRoleModel.Load(3, "Senior", 3),
                EmployeeRoleModel.Load(6, "Director", 6)
            };

            _mockRepository
                .Setup(x => x.GetById(1))
                .Returns(existingEmployee);

            _mockRoleRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            // Act
            Action act = () => _service.Update(employee, currentEmployee);

            // Assert
            act.Should().Throw<UnauthorizedAccessException>()
                .WithMessage("*do not have permission*");
        }

        [Fact]
        public void Update_WithExistingHigherRole_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var employee = new EmployeeDto
            {
                EmployeeId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                RoleId = 6 // Keeping Director role
            };

            var currentEmployee = new EmployeeDto { EmployeeId = 2, RoleId = 3 }; // Senior

            var existingEmployee = EmployeeModel.Load(
                1,
                "John",
                "Doe",
                "12345678901",
                "john@example.com",
                "hash",
                DateTime.Now.AddYears(-25),
                6,
                null
            );

            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(3, "Senior", 3),
                EmployeeRoleModel.Load(6, "Director", 6)
            };

            _mockRepository
                .Setup(x => x.GetById(1))
                .Returns(existingEmployee);

            _mockRoleRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            // Act
            Action act = () => _service.Update(employee, currentEmployee);

            // Assert
            act.Should().Throw<UnauthorizedAccessException>()
                .WithMessage("*do not have permission*");
        }

        [Fact]
        public void Update_WithLowerOrEqualRole_ShouldSucceed()
        {
            // Arrange
            var employee = new EmployeeDto
            {
                EmployeeId = 1,
                FirstName = "John",
                LastName = "Doe Updated",
                Email = "john@example.com",
                Password = "password",
                DocNumber = "12345678901",
                Birthday = DateTime.Now.AddYears(-25),
                RoleId = 2
            };

            var currentEmployee = new EmployeeDto { EmployeeId = 2, RoleId = 3 };

            var existingEmployee = EmployeeModel.Load(
                1,
                "John",
                "Doe",
                "12345678901",
                "john@example.com",
                "hash",
                DateTime.Now.AddYears(-25),
                1,
                null
            );

            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, "Junior", 1),
                EmployeeRoleModel.Load(2, "Pleno", 2),
                EmployeeRoleModel.Load(3, "Senior", 3)
            };

            var updatedEmployeeModel = EmployeeModel.Load(
                1,
                "John",
                "Doe Updated",
                "12345678901",
                "john@example.com",
                "hashedPassword",
                DateTime.Now.AddYears(-25),
                2,
                null
            );

            _mockRepository
                .Setup(x => x.GetById(1))
                .Returns(existingEmployee);

            _mockRoleRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            _mockAuthService
                .Setup(x => x.HashPassword("password"))
                .Returns("hashedPassword");

            _mockMapper
                .Setup(x => x.Map<IEmployeeModel>(It.IsAny<EmployeeDto>()))
                .Returns(updatedEmployeeModel);

            // Act
            _service.Update(employee, currentEmployee);

            // Assert
            _mockRepository.Verify(x => x.Update(It.IsAny<IEmployeeModel>()), Times.Once);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public void Delete_WithZeroEmployeeId_ShouldThrowArgumentException()
        {
            // Arrange & Act
            Action act = () => _service.Delete(0);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("employeeId")
                .WithMessage("*Employee ID must be greater than zero*");
        }

        [Fact]
        public void Delete_WithNegativeEmployeeId_ShouldThrowArgumentException()
        {
            // Arrange & Act
            Action act = () => _service.Delete(-1);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("employeeId")
                .WithMessage("*Employee ID must be greater than zero*");
        }

        [Fact]
        public void Delete_WithValidEmployeeId_ShouldCallRepositoryDelete()
        {
            // Arrange
            _mockRepository
                .Setup(x => x.Delete(1))
                .Verifiable();

            // Act
            _service.Delete(1);

            // Assert
            _mockRepository.Verify(x => x.Delete(1), Times.Once);
        }

        [Fact]
        public void Delete_WhenRepositoryThrowsException_ShouldWrapInInvalidOperationException()
        {
            // Arrange
            _mockRepository
                .Setup(x => x.Delete(1))
                .Throws(new InvalidOperationException("Database error"));

            // Act
            Action act = () => _service.Delete(1);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*Failed to delete employee*");
        }

        #endregion

        #region GetById Tests

        [Fact]
        public void GetById_WithZeroEmployeeId_ShouldThrowArgumentException()
        {
            // Arrange & Act
            Action act = () => _service.GetById(0);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("employeeId")
                .WithMessage("*Employee ID must be greater than zero*");
        }

        [Fact]
        public void GetById_WithNegativeEmployeeId_ShouldThrowArgumentException()
        {
            // Arrange & Act
            Action act = () => _service.GetById(-1);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithParameterName("employeeId")
                .WithMessage("*Employee ID must be greater than zero*");
        }

        [Fact]
        public void GetById_WithNonExistentEmployee_ShouldReturnNull()
        {
            // Arrange
            _mockRepository
                .Setup(x => x.GetById(999))
                .Returns((IEmployeeModel)null!);

            // Act
            var result = _service.GetById(999);

            // Assert
            result.Should().BeNull();
            _mockRepository.Verify(x => x.GetById(999), Times.Once);
        }

        [Fact]
        public void GetById_WithValidEmployeeId_ShouldReturnMappedDto()
        {
            // Arrange
            var employeeModel = EmployeeModel.Load(
                1,
                "John",
                "Doe",
                "12345678901",
                "john@example.com",
                "hash",
                DateTime.Now.AddYears(-25),
                1,
                null
            );

            var employeeDto = new EmployeeDto
            {
                EmployeeId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                RoleId = 1
            };

            _mockRepository
                .Setup(x => x.GetById(1))
                .Returns(employeeModel);

            _mockMapper
                .Setup(x => x.Map<EmployeeDto>(employeeModel))
                .Returns(employeeDto);

            // Act
            var result = _service.GetById(1);

            // Assert
            result.Should().NotBeNull();
            result!.EmployeeId.Should().Be(1);
            result.FirstName.Should().Be("John");
            result.LastName.Should().Be("Doe");
            result.Email.Should().Be("john@example.com");
            result.RoleId.Should().Be(1);
            _mockRepository.Verify(x => x.GetById(1), Times.Once);
            _mockMapper.Verify(x => x.Map<EmployeeDto>(employeeModel), Times.Once);
        }

        #endregion

        #region ListAll Tests

        [Fact]
        public void ListAll_WhenRepositoryReturnsEmptyList_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyList = new List<IEmployeeModel>();
            var emptyDtosList = new List<EmployeeDto>();

            _mockRepository
                .Setup(x => x.ListAll())
                .Returns(emptyList);

            _mockMapper
                .Setup(x => x.Map<IList<EmployeeDto>>(emptyList))
                .Returns(emptyDtosList);

            // Act
            var result = _service.ListAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mockRepository.Verify(x => x.ListAll(), Times.Once);
        }

        [Fact]
        public void ListAll_WhenRepositoryReturnsSingleEmployee_ShouldReturnSingleDto()
        {
            // Arrange
            var employee = EmployeeModel.Load(
                1,
                "John",
                "Doe",
                "12345678901",
                "john@example.com",
                "hash",
                DateTime.Now.AddYears(-25),
                1,
                null
            );
            var employeesList = new List<IEmployeeModel> { employee };

            var employeeDto = new EmployeeDto
            {
                EmployeeId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                RoleId = 1
            };
            var dtosList = new List<EmployeeDto> { employeeDto };

            _mockRepository
                .Setup(x => x.ListAll())
                .Returns(employeesList);

            _mockMapper
                .Setup(x => x.Map<IList<EmployeeDto>>(employeesList))
                .Returns(dtosList);

            // Act
            var result = _service.ListAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].EmployeeId.Should().Be(1);
            result[0].FirstName.Should().Be("John");
        }

        [Fact]
        public void ListAll_WhenRepositoryReturnsMultipleEmployees_ShouldReturnAllDtos()
        {
            // Arrange
            var birthday = DateTime.Now.AddYears(-25);
            var employees = new List<IEmployeeModel>
            {
                EmployeeModel.Load(1, "John", "Doe", "12345678901", "john@example.com", "hash1", birthday, 1, null),
                EmployeeModel.Load(2, "Jane", "Smith", "98765432101", "jane@example.com", "hash2", birthday, 2, null),
                EmployeeModel.Load(3, "Bob", "Johnson", "11122233301", "bob@example.com", "hash3", birthday, 3, null)
            };

            var dtos = new List<EmployeeDto>
            {
                new EmployeeDto { EmployeeId = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", RoleId = 1 },
                new EmployeeDto { EmployeeId = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", RoleId = 2 },
                new EmployeeDto { EmployeeId = 3, FirstName = "Bob", LastName = "Johnson", Email = "bob@example.com", RoleId = 3 }
            };

            _mockRepository
                .Setup(x => x.ListAll())
                .Returns(employees);

            _mockMapper
                .Setup(x => x.Map<IList<EmployeeDto>>(employees))
                .Returns(dtos);

            // Act
            var result = _service.ListAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(dtos, options => options.WithStrictOrdering());
            _mockRepository.Verify(x => x.ListAll(), Times.Once);
        }

        [Fact]
        public void ListAll_ShouldCallRepositoryOnlyOnce()
        {
            // Arrange
            var birthday = DateTime.Now.AddYears(-25);
            var employees = new List<IEmployeeModel>
            {
                EmployeeModel.Load(1, "John", "Doe", "12345678901", "john@example.com", "hash", birthday, 1, null)
            };

            var dtos = new List<EmployeeDto>
            {
                new EmployeeDto { EmployeeId = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", RoleId = 1 }
            };

            _mockRepository
                .Setup(x => x.ListAll())
                .Returns(employees);

            _mockMapper
                .Setup(x => x.Map<IList<EmployeeDto>>(employees))
                .Returns(dtos);

            // Act
            _service.ListAll();

            // Assert
            _mockRepository.Verify(x => x.ListAll(), Times.Once);
        }

        #endregion

        #region Role Hierarchy Validation Tests

        [Fact]
        public void ValidateRoleHierarchy_DirectorCanManageAllRoles()
        {
            // Arrange
            var birthday = DateTime.Now.AddYears(-25);
            var employee = new EmployeeDto
            {
                FirstName = "New",
                LastName = "Employee",
                Email = "employee@example.com",
                Password = "password",
                DocNumber = "12345678901",
                Birthday = birthday,
                RoleId = 1 // Junior
            };

            var currentEmployee = new EmployeeDto { EmployeeId = 1, RoleId = 6 }; // Director

            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, "Junior", 1),
                EmployeeRoleModel.Load(6, "Director", 6)
            };

            var employeeModel = EmployeeModel.Load(0, "New", "Employee", "12345678901", "employee@example.com", "hash", birthday, 1, null);

            _mockRoleRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            _mockAuthService
                .Setup(x => x.HashPassword("password"))
                .Returns("hash");

            _mockMapper
                .Setup(x => x.Map<IEmployeeModel>(It.IsAny<EmployeeDto>()))
                .Returns(employeeModel);

            _mockRepository
                .Setup(x => x.Add(It.IsAny<IEmployeeModel>()))
                .Returns(1);

            // Act
            var result = _service.Add(employee, currentEmployee);

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public void ValidateRoleHierarchy_JuniorCannotManageSenior()
        {
            // Arrange
            var employee = new EmployeeDto
            {
                FirstName = "New",
                LastName = "Employee",
                Email = "employee@example.com",
                Password = "password",
                RoleId = 3 // Senior
            };

            var currentEmployee = new EmployeeDto { EmployeeId = 1, RoleId = 1 }; // Junior

            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, "Junior", 1),
                EmployeeRoleModel.Load(3, "Senior", 3)
            };

            _mockRoleRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            // Act
            Action act = () => _service.Add(employee, currentEmployee);

            // Assert
            act.Should().Throw<UnauthorizedAccessException>()
                .WithMessage("*do not have permission*");
        }

        [Fact]
        public void ValidateRoleHierarchy_SeniorCanManageJuniorAndPleno()
        {
            // Arrange
            var birthday = DateTime.Now.AddYears(-25);
            var employee = new EmployeeDto
            {
                FirstName = "New",
                LastName = "Employee",
                Email = "employee@example.com",
                Password = "password",
                DocNumber = "12345678901",
                Birthday = birthday,
                RoleId = 2 // Pleno
            };

            var currentEmployee = new EmployeeDto { EmployeeId = 1, RoleId = 3 }; // Senior

            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(2, "Pleno", 2),
                EmployeeRoleModel.Load(3, "Senior", 3)
            };

            var employeeModel = EmployeeModel.Load(0, "New", "Employee", "12345678901", "employee@example.com", "hash", birthday, 2, null);

            _mockRoleRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            _mockAuthService
                .Setup(x => x.HashPassword("password"))
                .Returns("hash");

            _mockMapper
                .Setup(x => x.Map<IEmployeeModel>(It.IsAny<EmployeeDto>()))
                .Returns(employeeModel);

            _mockRepository
                .Setup(x => x.Add(It.IsAny<IEmployeeModel>()))
                .Returns(1);

            // Act
            var result = _service.Add(employee, currentEmployee);

            // Assert
            result.Should().Be(1);
        }

        #endregion
    }
}
