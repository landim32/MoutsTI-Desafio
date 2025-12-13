using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using MoutsTI.Domain.Entities;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Infra.Context;
using MoutsTI.Infra.Repositories;

namespace MoutsTI.Tests.Infra.Repositories
{
    public class EmployeeRepositoryTests
    {
        private readonly Mock<MoutsTIContext> _mockContext;
        private readonly Mock<IMapper> _mockMapper;
        private readonly EmployeeRepository _repository;
        private readonly DateTime _validBirthday = DateTime.Today.AddYears(-25);

        public EmployeeRepositoryTests()
        {
            _mockContext = new Mock<MoutsTIContext>();
            _mockMapper = new Mock<IMapper>();
            _repository = new EmployeeRepository(_mockContext.Object, _mockMapper.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
        {
            // Arrange & Act
            Action act = () => { var unused = new EmployeeRepository(null!, _mockMapper.Object); };

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("context");
        }

        [Fact]
        public void Constructor_WithNullMapper_ShouldThrowArgumentNullException()
        {
            // Arrange & Act
            Action act = () => { var unused = new EmployeeRepository(_mockContext.Object, null!); };

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("mapper");
        }

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateInstance()
        {
            // Arrange & Act
            var repository = new EmployeeRepository(_mockContext.Object, _mockMapper.Object);

            // Assert
            repository.Should().NotBeNull();
        }

        #endregion

        #region Add Tests

        [Fact]
        public void Add_WithNullEmployee_ShouldThrowArgumentNullException()
        {
            // Arrange & Act
            Action act = () => _repository.Add(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("employee");
        }

        [Fact]
        public void Add_WithValidEmployee_ShouldAddAndReturnId()
        {
            // Arrange
            var employeeModel = EmployeeModel.Create(new EmployeeModel.EmployeeModelParameters
            {
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "123.456.789-01",
                Email = "john@example.com",
                Password = "password",
                Birthday = _validBirthday,
                RoleId = 1L
            });

            var entity = new Employee
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "123.456.789-01",
                Email = "john@example.com",
                Password = "password",
                Birthday = _validBirthday,
                RoleId = 1L
            };

            var mockDbSet = CreateMockDbSet(new List<Employee>());

            _mockContext.Setup(c => c.Employees).Returns(mockDbSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            _mockMapper.Setup(m => m.Map<Employee>(It.IsAny<IEmployeeModel>()))
                .Returns(entity);

            // Act
            var result = _repository.Add(employeeModel);

            // Assert
            result.Should().Be(1L);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
            mockDbSet.Verify(m => m.Add(It.IsAny<Employee>()), Times.Once);
        }

        [Fact]
        public void Add_ShouldMapEmployeeModelToEntity()
        {
            // Arrange
            var employeeModel = EmployeeModel.Create(new EmployeeModel.EmployeeModelParameters
            {
                FirstName = "Jane",
                LastName = "Smith",
                DocNumber = "987.654.321-09",
                Email = "jane@example.com",
                Password = "password",
                Birthday = _validBirthday,
                RoleId = 2L
            });

            var entity = new Employee { EmployeeId = 1L };
            var mockDbSet = CreateMockDbSet(new List<Employee>());

            _mockContext.Setup(c => c.Employees).Returns(mockDbSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            _mockMapper.Setup(m => m.Map<Employee>(It.IsAny<IEmployeeModel>()))
                .Returns(entity);

            // Act
            _repository.Add(employeeModel);

            // Assert
            _mockMapper.Verify(m => m.Map<Employee>(employeeModel), Times.Once);
        }

        [Fact]
        public void Add_WithPhones_ShouldAddEmployeeWithPhones()
        {
            // Arrange
            var employeeModel = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
            {
                EmployeeId = 0,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "123.456.789-01",
                Email = "john@example.com",
                Password = "password",
                Birthday = _validBirthday,
                RoleId = 1L
            });

            var phone1 = EmployeePhoneModel.Create(0, "11987654321");
            var phone2 = EmployeePhoneModel.Create(0, "1133334444");
            employeeModel.AddPhone(phone1);
            employeeModel.AddPhone(phone2);

            var entity = new Employee
            {
                EmployeeId = 1L,
                EmployeePhones = new List<EmployeePhone>
                {
                    new EmployeePhone { PhoneId = 1, Phone = "11987654321" },
                    new EmployeePhone { PhoneId = 2, Phone = "1133334444" }
                }
            };

            var mockDbSet = CreateMockDbSet(new List<Employee>());

            _mockContext.Setup(c => c.Employees).Returns(mockDbSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            _mockMapper.Setup(m => m.Map<Employee>(It.IsAny<IEmployeeModel>()))
                .Returns(entity);

            // Act
            var result = _repository.Add(employeeModel);

            // Assert
            result.Should().Be(1L);
            _mockMapper.Verify(m => m.Map<Employee>(It.Is<IEmployeeModel>(e => e.Phones.Count() == 2)), Times.Once);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public void Delete_WithNonExistentEmployee_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var mockDbSet = CreateMockDbSet(new List<Employee>());
            mockDbSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns((Employee?)null);

            _mockContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            // Act
            Action act = () => _repository.Delete(999L);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*Employee with ID 999 not found*");
        }

        [Fact]
        public void Delete_WithExistingEmployee_ShouldRemoveEmployee()
        {
            // Arrange
            var entity = new Employee
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                DocNumber = "123.456.789-01",
                Password = "password",
                Birthday = _validBirthday,
                RoleId = 1L
            };

            var mockDbSet = CreateMockDbSet(new List<Employee> { entity });
            mockDbSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns(entity);

            _mockContext.Setup(c => c.Employees).Returns(mockDbSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            _repository.Delete(1L);

            // Assert
            mockDbSet.Verify(m => m.Remove(entity), Times.Once);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Delete_ShouldCallFindWithCorrectId()
        {
            // Arrange
            var entity = new Employee { EmployeeId = 5L };
            var mockDbSet = CreateMockDbSet(new List<Employee> { entity });
            mockDbSet.Setup(m => m.Find(It.IsAny<object[]>())).Returns(entity);

            _mockContext.Setup(c => c.Employees).Returns(mockDbSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            // Act
            _repository.Delete(5L);

            // Assert
            mockDbSet.Verify(m => m.Find(5L), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public void GetById_WithNonExistentEmployee_ShouldReturnNull()
        {
            // Arrange
            var mockDbSet = CreateMockDbSet(new List<Employee>());

            _mockContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            // Act
            var result = _repository.GetById(999L);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetById_WithExistingEmployee_ShouldReturnModel()
        {
            // Arrange
            var entity = new Employee
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                DocNumber = "123.456.789-01",
                Password = "password",
                Birthday = _validBirthday,
                RoleId = 1L
            };

            var employeeModel = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
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

            var mockDbSet = CreateMockDbSet(new List<Employee> { entity });

            _mockContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            _mockMapper.Setup(m => m.Map<IEmployeeModel>(It.IsAny<Employee>()))
                .Returns(employeeModel);

            // Act
            var result = _repository.GetById(1L);

            // Assert
            result.Should().NotBeNull();
            result!.EmployeeId.Should().Be(1L);
            result.FirstName.Should().Be("John");
            result.LastName.Should().Be("Doe");
        }

        [Fact]
        public void GetById_ShouldCallMapper()
        {
            // Arrange
            var entity = new Employee
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                DocNumber = "123.456.789-01",
                Password = "password",
                Birthday = _validBirthday,
                RoleId = 1L,
                Role = new EmployeeRole { RoleId = 1L, Name = "Developer", Level = 2 }
            };

            var employeeModel = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
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

            var mockDbSet = CreateMockDbSet(new List<Employee> { entity });

            _mockContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            _mockMapper.Setup(m => m.Map<IEmployeeModel>(It.IsAny<Employee>()))
                .Returns(employeeModel);

            // Act
            var result = _repository.GetById(1L);

            // Assert
            result.Should().NotBeNull();
            _mockMapper.Verify(m => m.Map<IEmployeeModel>(It.IsAny<Employee>()), Times.Once);
        }

        [Fact]
        public void GetById_WithManagerAndPhones_ShouldIncludeRelationships()
        {
            // Arrange
            var manager = new Employee
            {
                EmployeeId = 2L,
                FirstName = "Manager",
                LastName = "Boss",
                Email = "manager@example.com",
                DocNumber = "987.654.321-09",
                Password = "password",
                Birthday = _validBirthday,
                RoleId = 2L
            };

            var entity = new Employee
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                DocNumber = "123.456.789-01",
                Password = "password",
                Birthday = _validBirthday,
                RoleId = 1L,
                ManagerId = 2L,
                Manager = manager,
                EmployeePhones = new List<EmployeePhone>
                {
                    new EmployeePhone { PhoneId = 1, Phone = "11987654321" },
                    new EmployeePhone { PhoneId = 2, Phone = "1133334444" }
                }
            };

            var employeeModel = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "123.456.789-01",
                Email = "john@example.com",
                Password = "password",
                Birthday = _validBirthday,
                RoleId = 1L,
                ManagerId = 2L
            });

            var mockDbSet = CreateMockDbSet(new List<Employee> { entity });

            _mockContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            _mockMapper.Setup(m => m.Map<IEmployeeModel>(It.IsAny<Employee>()))
                .Returns(employeeModel);

            // Act
            var result = _repository.GetById(1L);

            // Assert
            result.Should().NotBeNull();
            _mockMapper.Verify(m => m.Map<IEmployeeModel>(It.Is<Employee>(
                e => e.Manager != null && e.EmployeePhones.Count == 2)), Times.Once);
        }

        #endregion

        #region ListAll Tests

        [Fact]
        public void ListAll_WithEmptyDatabase_ShouldReturnEmptyList()
        {
            // Arrange
            var mockDbSet = CreateMockDbSet(new List<Employee>());

            _mockContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            _mockMapper.Setup(m => m.Map<IEnumerable<IEmployeeModel>>(It.IsAny<List<Employee>>()))
                .Returns(new List<IEmployeeModel>());

            // Act
            var result = _repository.ListAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void ListAll_WithMultipleEmployees_ShouldReturnAllModels()
        {
            // Arrange
            var entities = new List<Employee>
            {
                new Employee { EmployeeId = 1L, FirstName = "John", LastName = "Doe", Email = "john@example.com", DocNumber = "123.456.789-01", Password = "pass", Birthday = _validBirthday, RoleId = 1L },
                new Employee { EmployeeId = 2L, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", DocNumber = "987.654.321-09", Password = "pass", Birthday = _validBirthday, RoleId = 2L }
            };

            var models = new List<IEmployeeModel>
            {
                EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters { EmployeeId = 1L, FirstName = "John", LastName = "Doe", DocNumber = "123.456.789-01", Email = "john@example.com", Password = "pass", Birthday = _validBirthday, RoleId = 1L }),
                EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters { EmployeeId = 2L, FirstName = "Jane", LastName = "Smith", DocNumber = "987.654.321-09", Email = "jane@example.com", Password = "pass", Birthday = _validBirthday, RoleId = 2L })
            };

            var mockDbSet = CreateMockDbSet(entities);

            _mockContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            _mockMapper.Setup(m => m.Map<IEnumerable<IEmployeeModel>>(It.IsAny<List<Employee>>()))
                .Returns(models);

            // Act
            var result = _repository.ListAll().ToList();

            // Assert
            result.Should().HaveCount(2);
            result[0].FirstName.Should().Be("John");
            result[1].FirstName.Should().Be("Jane");
        }

        [Fact]
        public void ListAll_ShouldCallMapper()
        {
            // Arrange
            var entities = new List<Employee>
            {
                new Employee
                {
                    EmployeeId = 1L,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@example.com",
                    DocNumber = "123.456.789-01",
                    Password = "pass",
                    Birthday = _validBirthday,
                    RoleId = 1L,
                    Role = new EmployeeRole { RoleId = 1L, Name = "Developer", Level = 2 },
                    ManagerId = 2L,
                    Manager = new Employee { EmployeeId = 2L, FirstName = "Manager", LastName = "Boss", Email = "manager@example.com", DocNumber = "111.222.333-44", Password = "pass", Birthday = _validBirthday, RoleId = 2L },
                    EmployeePhones = new List<EmployeePhone> { new EmployeePhone { PhoneId = 1, Phone = "11987654321" } }
                }
            };

            var mockDbSet = CreateMockDbSet(entities);

            _mockContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            _mockMapper.Setup(m => m.Map<IEnumerable<IEmployeeModel>>(It.IsAny<List<Employee>>()))
                .Returns(new List<IEmployeeModel>());

            // Act
            _repository.ListAll();

            // Assert
            _mockMapper.Verify(m => m.Map<IEnumerable<IEmployeeModel>>(
                It.Is<List<Employee>>(list => 
                    list.Any(e => e.Role != null && e.Manager != null && e.EmployeePhones.Count != 0))), 
                Times.Once);
        }

        #endregion

        #region Update Tests

        [Fact]
        public void Update_WithNullEmployee_ShouldThrowArgumentNullException()
        {
            // Arrange & Act
            Action act = () => _repository.Update(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("employee");
        }

        [Fact]
        public void Update_WithNonExistentEmployee_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var employeeModel = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
            {
                EmployeeId = 999L,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "123.456.789-01",
                Email = "john@example.com",
                Password = "password",
                Birthday = _validBirthday,
                RoleId = 1L
            });

            var mockDbSet = CreateMockDbSet(new List<Employee>());

            _mockContext.Setup(c => c.Employees).Returns(mockDbSet.Object);

            // Act
            Action act = () => _repository.Update(employeeModel);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*Employee with ID 999 not found*");
        }

        [Fact]
        public void Update_WithExistingEmployee_ShouldUpdateEntity()
        {
            // Arrange
            var existingEntity = new Employee
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                DocNumber = "123.456.789-01",
                Password = "oldPassword",
                Birthday = _validBirthday,
                RoleId = 1L,
                EmployeePhones = new List<EmployeePhone>()
            };

            var employeeModel = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe Updated",
                DocNumber = "123.456.789-01",
                Email = "john.updated@example.com",
                Password = "newPassword",
                Birthday = _validBirthday,
                RoleId = 1L
            });

            var mockDbSet = CreateMockDbSet(new List<Employee> { existingEntity });

            _mockContext.Setup(c => c.Employees).Returns(mockDbSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            _mockMapper.Setup(m => m.Map(It.IsAny<IEmployeeModel>(), It.IsAny<Employee>()))
                .Returns(existingEntity);

            // Act
            _repository.Update(employeeModel);

            // Assert
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
            mockDbSet.Verify(m => m.Update(existingEntity), Times.Once);
        }

        [Fact]
        public void Update_WithEmptyPassword_ShouldKeepPreviousPassword()
        {
            // Arrange
            var existingEntity = new Employee
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                DocNumber = "123.456.789-01",
                Password = "oldPassword",
                Birthday = _validBirthday,
                RoleId = 1L,
                EmployeePhones = new List<EmployeePhone>()
            };

            var employeeModel = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe Updated",
                DocNumber = "123.456.789-01",
                Email = "john@example.com",
                Password = "", // Empty password
                Birthday = _validBirthday,
                RoleId = 1L
            });

            var mockDbSet = CreateMockDbSet(new List<Employee> { existingEntity });

            _mockContext.Setup(c => c.Employees).Returns(mockDbSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            _mockMapper.Setup(m => m.Map(It.IsAny<IEmployeeModel>(), It.IsAny<Employee>()))
                .Callback<IEmployeeModel, Employee>((model, entity) =>
                {
                    entity.Password = model.Password;
                })
                .Returns(existingEntity);

            // Act
            _repository.Update(employeeModel);

            // Assert
            existingEntity.Password.Should().Be("oldPassword");
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Update_WithWhitespacePassword_ShouldKeepPreviousPassword()
        {
            // Arrange
            var existingEntity = new Employee
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                DocNumber = "123.456.789-01",
                Password = "oldPassword",
                Birthday = _validBirthday,
                RoleId = 1L,
                EmployeePhones = new List<EmployeePhone>()
            };

            var employeeModel = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "123.456.789-01",
                Email = "john@example.com",
                Password = "   ", // Whitespace password
                Birthday = _validBirthday,
                RoleId = 1L
            });

            var mockDbSet = CreateMockDbSet(new List<Employee> { existingEntity });

            _mockContext.Setup(c => c.Employees).Returns(mockDbSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            _mockMapper.Setup(m => m.Map(It.IsAny<IEmployeeModel>(), It.IsAny<Employee>()))
                .Callback<IEmployeeModel, Employee>((model, entity) =>
                {
                    entity.Password = model.Password;
                })
                .Returns(existingEntity);

            // Act
            _repository.Update(employeeModel);

            // Assert
            existingEntity.Password.Should().Be("oldPassword");
        }

        [Fact]
        public void Update_WithValidPassword_ShouldUpdatePassword()
        {
            // Arrange
            var existingEntity = new Employee
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                DocNumber = "123.456.789-01",
                Password = "oldPassword",
                Birthday = _validBirthday,
                RoleId = 1L,
                EmployeePhones = new List<EmployeePhone>()
            };

            var employeeModel = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                DocNumber = "123.456.789-01",
                Email = "john@example.com",
                Password = "newPassword",
                Birthday = _validBirthday,
                RoleId = 1L
            });

            var mockDbSet = CreateMockDbSet(new List<Employee> { existingEntity });

            _mockContext.Setup(c => c.Employees).Returns(mockDbSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            _mockMapper.Setup(m => m.Map(It.IsAny<IEmployeeModel>(), It.IsAny<Employee>()))
                .Callback<IEmployeeModel, Employee>((model, entity) =>
                {
                    entity.Password = model.Password;
                })
                .Returns(existingEntity);

            // Act
            _repository.Update(employeeModel);

            // Assert
            existingEntity.Password.Should().Be("newPassword");
        }

        [Fact]
        public void Update_ShouldCallMapper()
        {
            // Arrange
            var existingEntity = new Employee
            {
                EmployeeId = 1L,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                DocNumber = "123.456.789-01",
                Password = "password",
                Birthday = _validBirthday,
                RoleId = 1L,
                EmployeePhones = new List<EmployeePhone>
                {
                    new EmployeePhone { PhoneId = 1, Phone = "11987654321" }
                }
            };

            var employeeModel = EmployeeModel.Load(new EmployeeModel.EmployeeModelParameters
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

            var mockDbSet = CreateMockDbSet(new List<Employee> { existingEntity });

            _mockContext.Setup(c => c.Employees).Returns(mockDbSet.Object);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);

            _mockMapper.Setup(m => m.Map(It.IsAny<IEmployeeModel>(), It.IsAny<Employee>()))
                .Returns(existingEntity);

            // Act
            _repository.Update(employeeModel);

            // Assert
            _mockMapper.Verify(m => m.Map(It.IsAny<IEmployeeModel>(), 
                It.Is<Employee>(e => e.EmployeePhones != null)), Times.Once);
        }

        #endregion

        #region Helper Methods

        private static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return mockSet;
        }

        #endregion
    }
}
