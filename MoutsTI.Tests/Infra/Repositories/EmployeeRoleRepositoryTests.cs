using AutoMapper;
using FluentAssertions;
using Moq;
using MoutsTI.Domain.Entities;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Infra.Context;
using MoutsTI.Infra.Repositories;
using System.Linq;

namespace MoutsTI.Tests.Infra.Repositories
{
    public class EmployeeRoleRepositoryTests
    {
        private readonly Mock<MoutsTIContext> _mockContext;
        private readonly Mock<IMapper> _mockMapper;
        private readonly EmployeeRoleRepository _repository;

        public EmployeeRoleRepositoryTests()
        {
            _mockContext = new Mock<MoutsTIContext>();
            _mockMapper = new Mock<IMapper>();
            _repository = new EmployeeRoleRepository(_mockContext.Object, _mockMapper.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
        {   
            // Arrange & Act
            Action act = () => { var unused = new EmployeeRoleRepository(null!, _mockMapper.Object); };

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("context");
        }

        [Fact]
        public void Constructor_WithNullMapper_ShouldThrowArgumentNullException()
        {
            // Arrange & Act
            Action act = () => { var unused = new EmployeeRoleRepository(_mockContext.Object, null!); };

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("mapper");
        }

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateInstance()
        {
            // Arrange & Act
            var repository = new EmployeeRoleRepository(_mockContext.Object, _mockMapper.Object);

            // Assert
            repository.Should().NotBeNull();
        }

        #endregion

        #region ListAll Tests

        [Fact]
        public void ListAll_WithEmptyDatabase_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyList = new List<EmployeeRole>();
            var expectedModels = new List<IEmployeeRoleModel>();

            _mockContext.Setup(c => c.EmployeeRoles)
                .Returns(CreateMockDbSet(emptyList).Object);

            _mockMapper.Setup(m => m.Map<IEnumerable<IEmployeeRoleModel>>(It.IsAny<List<EmployeeRole>>()))
                .Returns(expectedModels);

            // Act
            var result = _repository.ListAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mockMapper.Verify(m => m.Map<IEnumerable<IEmployeeRoleModel>>(It.IsAny<List<EmployeeRole>>()), Times.Once);
        }

        [Fact]
        public void ListAll_WithSingleRole_ShouldReturnSingleModel()
        {
            // Arrange
            var entities = new List<EmployeeRole>
            {
                new EmployeeRole { RoleId = 1L, Name = "Developer", Level = 2 }
            };

            var expectedModel = EmployeeRoleModel.Load(1L, "Developer", 2);
            var expectedModels = new List<IEmployeeRoleModel> { expectedModel };

            _mockContext.Setup(c => c.EmployeeRoles)
                .Returns(CreateMockDbSet(entities).Object);

            _mockMapper.Setup(m => m.Map<IEnumerable<IEmployeeRoleModel>>(It.IsAny<List<EmployeeRole>>()))
                .Returns(expectedModels);

            // Act
            var result = _repository.ListAll().ToList();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].RoleId.Should().Be(1L);
            result[0].Name.Should().Be("Developer");
            result[0].Level.Should().Be(2);
            _mockMapper.Verify(m => m.Map<IEnumerable<IEmployeeRoleModel>>(It.Is<List<EmployeeRole>>(
                list => list.Count == 1 && list[0].RoleId == 1L)), Times.Once);
        }

        [Fact]
        public void ListAll_WithMultipleRoles_ShouldReturnAllModels()
        {
            // Arrange
            var entities = new List<EmployeeRole>
            {
                new EmployeeRole { RoleId = 1L, Name = "Junior Developer", Level = 1 },
                new EmployeeRole { RoleId = 2L, Name = "Senior Developer", Level = 3 },
                new EmployeeRole { RoleId = 3L, Name = "Tech Lead", Level = 5 }
            };

            var expectedModels = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1L, "Junior Developer", 1),
                EmployeeRoleModel.Load(2L, "Senior Developer", 3),
                EmployeeRoleModel.Load(3L, "Tech Lead", 5)
            };

            _mockContext.Setup(c => c.EmployeeRoles)
                .Returns(CreateMockDbSet(entities).Object);

            _mockMapper.Setup(m => m.Map<IEnumerable<IEmployeeRoleModel>>(It.IsAny<List<EmployeeRole>>()))
                .Returns(expectedModels);

            // Act
            var result = _repository.ListAll().ToList();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result[0].Name.Should().Be("Junior Developer");
            result[1].Name.Should().Be("Senior Developer");
            result[2].Name.Should().Be("Tech Lead");
            _mockMapper.Verify(m => m.Map<IEnumerable<IEmployeeRoleModel>>(It.Is<List<EmployeeRole>>(
                list => list.Count == 3)), Times.Once);
        }

        [Fact]
        public void ListAll_CalledMultipleTimes_ShouldReturnConsistentResults()
        {
            // Arrange
            var entities = new List<EmployeeRole>
            {
                new EmployeeRole { RoleId = 1L, Name = "Developer", Level = 2 }
            };

            var expectedModel = EmployeeRoleModel.Load(1L, "Developer", 2);
            var expectedModels = new List<IEmployeeRoleModel> { expectedModel };

            _mockContext.Setup(c => c.EmployeeRoles)
                .Returns(CreateMockDbSet(entities).Object);

            _mockMapper.Setup(m => m.Map<IEnumerable<IEmployeeRoleModel>>(It.IsAny<List<EmployeeRole>>()))
                .Returns(expectedModels);

            // Act
            var result1 = _repository.ListAll().ToList();
            var result2 = _repository.ListAll().ToList();

            // Assert
            result1.Should().HaveCount(1);
            result2.Should().HaveCount(1);
            result1[0].RoleId.Should().Be(result2[0].RoleId);
            result1[0].Name.Should().Be(result2[0].Name);
            result1[0].Level.Should().Be(result2[0].Level);
            _mockMapper.Verify(m => m.Map<IEnumerable<IEmployeeRoleModel>>(It.IsAny<List<EmployeeRole>>()), Times.Exactly(2));
        }

        [Fact]
        public void ListAll_WithDifferentLevels_ShouldReturnAllRoles()
        {
            // Arrange
            var entities = new List<EmployeeRole>
            {
                new EmployeeRole { RoleId = 1L, Name = "Intern", Level = 1 },
                new EmployeeRole { RoleId = 2L, Name = "Manager", Level = 50 },
                new EmployeeRole { RoleId = 3L, Name = "Director", Level = 100 }
            };

            var expectedModels = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1L, "Intern", 1),
                EmployeeRoleModel.Load(2L, "Manager", 50),
                EmployeeRoleModel.Load(3L, "Director", 100)
            };

            _mockContext.Setup(c => c.EmployeeRoles)
                .Returns(CreateMockDbSet(entities).Object);

            _mockMapper.Setup(m => m.Map<IEnumerable<IEmployeeRoleModel>>(It.IsAny<List<EmployeeRole>>()))
                .Returns(expectedModels);

            // Act
            var result = _repository.ListAll().ToList();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(r => r.Level == 1);
            result.Should().Contain(r => r.Level == 50);
            result.Should().Contain(r => r.Level == 100);
        }

        [Fact]
        public void ListAll_ShouldCallMapperWithCorrectEntityList()
        {
            // Arrange
            var entities = new List<EmployeeRole>
            {
                new EmployeeRole { RoleId = 1L, Name = "Developer", Level = 2 },
                new EmployeeRole { RoleId = 2L, Name = "Designer", Level = 2 }
            };

            var expectedModels = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1L, "Developer", 2),
                EmployeeRoleModel.Load(2L, "Designer", 2)
            };

            _mockContext.Setup(c => c.EmployeeRoles)
                .Returns(CreateMockDbSet(entities).Object);

            _mockMapper.Setup(m => m.Map<IEnumerable<IEmployeeRoleModel>>(It.IsAny<List<EmployeeRole>>()))
                .Returns(expectedModels);

            // Act
            var result = _repository.ListAll();

            // Assert
            _mockMapper.Verify(m => m.Map<IEnumerable<IEmployeeRoleModel>>(It.Is<List<EmployeeRole>>(
                list => list.Count == 2 && 
                        list.Any(e => e.RoleId == 1L && e.Name == "Developer") &&
                        list.Any(e => e.RoleId == 2L && e.Name == "Designer"))), Times.Once);
        }

        [Fact]
        public void ListAll_WithLongRoleName_ShouldHandleCorrectly()
        {
            // Arrange
            var longName = new string('A', 80); // Nome com 80 caracteres
            var entities = new List<EmployeeRole>
            {
                new EmployeeRole { RoleId = 1L, Name = longName, Level = 1 }
            };

            var expectedModel = EmployeeRoleModel.Load(1L, longName, 1);
            var expectedModels = new List<IEmployeeRoleModel> { expectedModel };

            _mockContext.Setup(c => c.EmployeeRoles)
                .Returns(CreateMockDbSet(entities).Object);

            _mockMapper.Setup(m => m.Map<IEnumerable<IEmployeeRoleModel>>(It.IsAny<List<EmployeeRole>>()))
                .Returns(expectedModels);

            // Act
            var result = _repository.ListAll().ToList();

            // Assert
            result.Should().HaveCount(1);
            result[0].Name.Should().HaveLength(80);
            result[0].Name.Should().Be(longName);
        }

        [Fact]
        public void ListAll_ShouldCallToListOnDbSet()
        {
            // Arrange
            var entities = new List<EmployeeRole>
            {
                new EmployeeRole { RoleId = 1L, Name = "Developer", Level = 2 }
            };

            var mockDbSet = CreateMockDbSet(entities);
            var expectedModels = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1L, "Developer", 2)
            };

            _mockContext.Setup(c => c.EmployeeRoles)
                .Returns(mockDbSet.Object);

            _mockMapper.Setup(m => m.Map<IEnumerable<IEmployeeRoleModel>>(It.IsAny<List<EmployeeRole>>()))
                .Returns(expectedModels);

            // Act
            var result = _repository.ListAll();

            // Assert
            result.Should().NotBeNull();
            _mockContext.Verify(c => c.EmployeeRoles, Times.Once);
        }

        [Fact]
        public void ListAll_WhenMapperReturnsNull_ShouldReturnNull()
        {
            // Arrange
            var entities = new List<EmployeeRole>
            {
                new EmployeeRole { RoleId = 1L, Name = "Developer", Level = 2 }
            };

            _mockContext.Setup(c => c.EmployeeRoles)
                .Returns(CreateMockDbSet(entities).Object);

            _mockMapper.Setup(m => m.Map<IEnumerable<IEmployeeRoleModel>>(It.IsAny<List<EmployeeRole>>()))
                .Returns((IEnumerable<IEmployeeRoleModel>)null!);

            // Act
            var result = _repository.ListAll();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ListAll_WithSpecialCharactersInName_ShouldHandleCorrectly()
        {
            // Arrange
            var specialName = "Developer / Designer & Architect";
            var entities = new List<EmployeeRole>
            {
                new EmployeeRole { RoleId = 1L, Name = specialName, Level = 3 }
            };

            var expectedModel = EmployeeRoleModel.Load(1L, specialName, 3);
            var expectedModels = new List<IEmployeeRoleModel> { expectedModel };

            _mockContext.Setup(c => c.EmployeeRoles)
                .Returns(CreateMockDbSet(entities).Object);

            _mockMapper.Setup(m => m.Map<IEnumerable<IEmployeeRoleModel>>(It.IsAny<List<EmployeeRole>>()))
                .Returns(expectedModels);

            // Act
            var result = _repository.ListAll().ToList();

            // Assert
            result.Should().HaveCount(1);
            result[0].Name.Should().Be(specialName);
        }

        [Fact]
        public void ListAll_WithMaxLevel_ShouldHandleCorrectly()
        {
            // Arrange
            var entities = new List<EmployeeRole>
            {
                new EmployeeRole { RoleId = 1L, Name = "CEO", Level = 100 }
            };

            var expectedModel = EmployeeRoleModel.Load(1L, "CEO", 100);
            var expectedModels = new List<IEmployeeRoleModel> { expectedModel };

            _mockContext.Setup(c => c.EmployeeRoles)
                .Returns(CreateMockDbSet(entities).Object);

            _mockMapper.Setup(m => m.Map<IEnumerable<IEmployeeRoleModel>>(It.IsAny<List<EmployeeRole>>()))
                .Returns(expectedModels);

            // Act
            var result = _repository.ListAll().ToList();

            // Assert
            result.Should().HaveCount(1);
            result[0].Level.Should().Be(100);
        }

        [Fact]
        public void ListAll_WithMinLevel_ShouldHandleCorrectly()
        {
            // Arrange
            var entities = new List<EmployeeRole>
            {
                new EmployeeRole { RoleId = 1L, Name = "Intern", Level = 1 }
            };

            var expectedModel = EmployeeRoleModel.Load(1L, "Intern", 1);
            var expectedModels = new List<IEmployeeRoleModel> { expectedModel };

            _mockContext.Setup(c => c.EmployeeRoles)
                .Returns(CreateMockDbSet(entities).Object);

            _mockMapper.Setup(m => m.Map<IEnumerable<IEmployeeRoleModel>>(It.IsAny<List<EmployeeRole>>()))
                .Returns(expectedModels);

            // Act
            var result = _repository.ListAll().ToList();

            // Assert
            result.Should().HaveCount(1);
            result[0].Level.Should().Be(1);
        }

        #endregion

        #region Helper Methods

        private static Mock<Microsoft.EntityFrameworkCore.DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return mockSet;
        }

        #endregion
    }
}
