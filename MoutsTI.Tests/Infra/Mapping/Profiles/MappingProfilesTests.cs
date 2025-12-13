using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MoutsTI.Domain.Entities;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Dtos;
using MoutsTI.Infra.Context;
using MoutsTI.Infra.Mapping.Profiles;

namespace MoutsTI.Tests.Infra.Mapping.Profiles
{
    public class MappingProfilesTests
    {
        private readonly IMapper _mapper;
        private readonly DateTime _validBirthday = DateTime.Today.AddYears(-25);

        public MappingProfilesTests()
        {
            // Configuração do LoggerFactory para testes
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Warning); // Apenas warnings e errors durante os testes
            });

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DtoToEmployeeProfile>();
                cfg.AddProfile<EmployeeToDtoProfile>();
                cfg.AddProfile<EmployeeToEntityProfile>();
                cfg.AddProfile<EntityToEmployeeProfile>();
                cfg.AddProfile<EmployeeRoleProfile>();
                cfg.AddProfile<EmployeePhoneProfile>();
            }, loggerFactory);

            config.AssertConfigurationIsValid();
            _mapper = config.CreateMapper();
        }

        #region DtoToEmployeeProfile Tests

        [Fact]
        public void DtoToEmployeeProfile_WithNewEmployee_ShouldMapCorrectly()
        {
            // Arrange
            var dto = new EmployeeDto
            {
                EmployeeId = 0,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "123.456.789-01",
                Email = "john@example.com",
                Password = "password123",
                Birthday = _validBirthday,
                RoleId = 1L
            };

            // Act
            var result = _mapper.Map<IEmployeeModel>(dto);

            // Assert
            result.Should().NotBeNull();
            result.EmployeeId.Should().Be(0);
            result.FirstName.Should().Be("John");
            result.LastName.Should().Be("Doe");
            result.DocNumber.Should().Be("123.456.789-01");
            result.Email.Should().Be("john@example.com");
            result.Password.Should().Be("password123");
            result.Birthday.Should().Be(_validBirthday);
            result.RoleId.Should().Be(1L);
        }

        [Fact]
        public void DtoToEmployeeProfile_WithExistingEmployee_ShouldMapCorrectly()
        {
            // Arrange
            var dto = new EmployeeDto
            {
                EmployeeId = 5L,
                FirstName = "Jane",
                LastName = "Smith",
                DocNumber = "987.654.321-09",
                Email = "jane@example.com",
                Password = "securepass",
                Birthday = _validBirthday,
                RoleId = 2L,
                ManagerId = 1L
            };

            // Act
            var result = _mapper.Map<IEmployeeModel>(dto);

            // Assert
            result.Should().NotBeNull();
            result.EmployeeId.Should().Be(5L);
            result.FirstName.Should().Be("Jane");
            result.LastName.Should().Be("Smith");
            result.ManagerId.Should().Be(1L);
        }

        [Fact]
        public void DtoToEmployeeProfile_WithPhones_ShouldMapPhones()
        {
            // Arrange
            var dto = new EmployeeDto
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "123.456.789-01",
                Email = "john@example.com",
                Password = "password",
                Birthday = _validBirthday,
                RoleId = 1L,
                Phones = new List<string> { "11987654321", "1133334444" }
            };

            // Act
            var result = _mapper.Map<IEmployeeModel>(dto);

            // Assert
            result.Should().NotBeNull();
            result.Phones.Should().HaveCount(2);
            result.Phones.Should().Contain(p => p.Phone == "11987654321");
            result.Phones.Should().Contain(p => p.Phone == "1133334444");
        }

        [Fact]
        public void DtoToEmployeeProfile_WithEmptyPhones_ShouldNotAddPhones()
        {
            // Arrange
            var dto = new EmployeeDto
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "123.456.789-01",
                Email = "john@example.com",
                Password = "password",
                Birthday = _validBirthday,
                RoleId = 1L,
                Phones = new List<string>()
            };

            // Act
            var result = _mapper.Map<IEmployeeModel>(dto);

            // Assert
            result.Should().NotBeNull();
            result.Phones.Should().BeEmpty();
        }

        #endregion

        #region EmployeeToDtoProfile Tests

        [Fact]
        public void EmployeeToDtoProfile_ShouldMapModelToDto()
        {
            // Arrange
            var model = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "123.456.789-01",
                Email = "john@example.com",
                Password = "hashedPassword",
                Birthday = _validBirthday,
                RoleId = 1L
            });

            // Act
            var result = _mapper.Map<EmployeeDto>(model);

            // Assert
            result.Should().NotBeNull();
            result.EmployeeId.Should().Be(1L);
            result.FirstName.Should().Be("John");
            result.LastName.Should().Be("Doe");
            result.FullName.Should().Be("John Doe");
            result.Password.Should().BeNullOrEmpty(); // Password should be ignored
        }

        [Fact]
        public void EmployeeToDtoProfile_ShouldMapFullName()
        {
            // Arrange
            var model = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
            {
                EmployeeId = 1L,
                FirstName = "Jane",
                LastName = "Smith",
                DocNumber = "123.456.789-01",
                Email = "jane@example.com",
                Password = "password",
                Birthday = _validBirthday,
                RoleId = 1L
            });

            // Act
            var result = _mapper.Map<EmployeeDto>(model);

            // Assert
            result.FullName.Should().Be("Jane Smith");
        }

        #endregion

        #region EmployeeRoleProfile Tests

        [Fact]
        public void EmployeeRoleProfile_ShouldMapModelToDto()
        {
            // Arrange
            var model = EmployeeRoleModel.Load(1L, "Developer", 2);

            // Act
            var result = _mapper.Map<EmployeeRoleDto>(model);

            // Assert
            result.Should().NotBeNull();
            result.RoleId.Should().Be(1L);
            result.Name.Should().Be("Developer");
            result.Level.Should().Be(2);
        }

        [Fact]
        public void EmployeeRoleProfile_ShouldMapDtoToModelForNew()
        {
            // Arrange
            var dto = new EmployeeRoleDto
            {
                RoleId = 0,
                Name = "Analyst",
                Level = 1
            };

            // Act
            var result = _mapper.Map<IEmployeeRoleModel>(dto);

            // Assert
            result.Should().NotBeNull();
            result.RoleId.Should().Be(0);
            result.Name.Should().Be("Analyst");
            result.Level.Should().Be(1);
        }

        [Fact]
        public void EmployeeRoleProfile_ShouldMapDtoToModelForExisting()
        {
            // Arrange
            var dto = new EmployeeRoleDto
            {
                RoleId = 5L,
                Name = "Senior Developer",
                Level = 4
            };

            // Act
            var result = _mapper.Map<IEmployeeRoleModel>(dto);

            // Assert
            result.Should().NotBeNull();
            result.RoleId.Should().Be(5L);
            result.Name.Should().Be("Senior Developer");
            result.Level.Should().Be(4);
        }

        [Fact]
        public void EmployeeRoleProfile_EntityToModel_ShouldMapCorrectly()
        {
            // Arrange
            var entity = new EmployeeRole
            {
                RoleId = 1L,
                Name = "Developer",
                Level = 2
            };

            // Act
            var result = _mapper.Map<IEmployeeRoleModel>(entity);

            // Assert
            result.Should().NotBeNull();
            result.RoleId.Should().Be(1L);
            result.Name.Should().Be("Developer");
            result.Level.Should().Be(2);
        }

        #endregion

        #region EmployeePhoneProfile Tests

        [Fact]
        public void EmployeePhoneProfile_ModelToEntity_ShouldMapCorrectly()
        {
            // Arrange
            var model = EmployeePhoneModel.Load(5L, 20L, "1133334444");

            // Act
            var result = _mapper.Map<EmployeePhone>(model);

            // Assert
            result.Should().NotBeNull();
            result.PhoneId.Should().Be(5L);
            result.EmployeeId.Should().Be(20L);
            result.Phone.Should().Be("1133334444");
        }

        [Fact]
        public void EmployeePhoneProfile_EntityToModel_ShouldMapCorrectly()
        {
            // Arrange
            var entity = new EmployeePhone
            {
                PhoneId = 5L,
                EmployeeId = 20L,
                Phone = "1133334444"
            };

            // Act
            var result = _mapper.Map<IEmployeePhoneModel>(entity);

            // Assert
            result.Should().NotBeNull();
            result.PhoneId.Should().Be(5L);
            result.EmployeeId.Should().Be(20L);
            result.Phone.Should().Be("1133334444");
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void MappingLogic_EntityToModel_ShouldWork()
        {
            // Arrange
            var entity = new Employee
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "123.456.789-01",
                Email = "john@example.com",
                Password = "password",
                Birthday = DateTime.SpecifyKind(_validBirthday, DateTimeKind.Utc),
                RoleId = 1L,
                ManagerId = 5L
            };

            // Act
            var result = _mapper.Map<IEmployeeModel>(entity);

            // Assert
            result.Should().NotBeNull();
            result.EmployeeId.Should().Be(1L);
            result.FirstName.Should().Be("John");
            result.LastName.Should().Be("Doe");
            result.Email.Should().Be("john@example.com");
        }

        [Fact]
        public void MappingLogic_ModelToEntity_ShouldWork()
        {
            // Arrange
            var model = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "123.456.789-01",
                Email = "john@example.com",
                Password = "password",
                Birthday = _validBirthday,
                RoleId = 1L
            });

            // Act
            var result = _mapper.Map<Employee>(model);

            // Assert
            result.Should().NotBeNull();
            result.EmployeeId.Should().Be(1L);
            result.FirstName.Should().Be("John");
            result.LastName.Should().Be("Doe");
            result.Email.Should().Be("john@example.com");
        }

        [Fact]
        public void MappingLogic_ModelToDto_ShouldIgnorePassword()
        {
            // Arrange
            var employee = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "123.456.789-01",
                Email = "john@example.com",
                Password = "secretPassword123",
                Birthday = _validBirthday,
                RoleId = 1L
            });

            // Act
            var result = _mapper.Map<EmployeeDto>(employee);

            // Assert
            result.Should().NotBeNull();
            result.Password.Should().BeNullOrEmpty(); // Password should be ignored in DTO
            result.FirstName.Should().Be("John");
            result.Email.Should().Be("john@example.com");
        }

        [Fact]
        public void MappingLogic_RoundTrip_DtoToModelToDto_ShouldPreserveData()
        {
            // Arrange
            var originalDto = new EmployeeDto
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "123.456.789-01",
                Email = "john@example.com",
                Password = "password",
                Birthday = _validBirthday,
                RoleId = 1L,
                Phones = new List<string> { "11987654321" }
            };

            // Act
            var model = _mapper.Map<IEmployeeModel>(originalDto);
            var resultDto = _mapper.Map<EmployeeDto>(model);

            // Assert
            resultDto.Should().NotBeNull();
            resultDto.EmployeeId.Should().Be(originalDto.EmployeeId);
            resultDto.FirstName.Should().Be(originalDto.FirstName);
            resultDto.LastName.Should().Be(originalDto.LastName);
            resultDto.Email.Should().Be(originalDto.Email);
            resultDto.Phones.Should().ContainSingle();
            resultDto.Phones.First().Should().Be("11987654321");
        }

        #endregion

        #region Configuration Tests

        [Fact]
        public void AutoMapper_Configuration_ShouldBeValid()
        {
            // Arrange & Act
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Warning);
            });

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DtoToEmployeeProfile>();
                cfg.AddProfile<EmployeeToDtoProfile>();
                cfg.AddProfile<EmployeeToEntityProfile>();
                cfg.AddProfile<EntityToEmployeeProfile>();
                cfg.AddProfile<EmployeeRoleProfile>();
                cfg.AddProfile<EmployeePhoneProfile>();
            }, loggerFactory);

            // Assert
            var action = () => config.AssertConfigurationIsValid();
            action.Should().NotThrow();
        }

        #endregion
    }
}
