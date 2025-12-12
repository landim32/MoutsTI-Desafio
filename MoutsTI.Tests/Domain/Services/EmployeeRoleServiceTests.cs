using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MoutsTI.Domain.Entities;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Domain.Services;
using MoutsTI.Dtos;
using MoutsTI.Infra.Interfaces.Repositories;

namespace MoutsTI.Tests.Domain.Services
{
    public class EmployeeRoleServiceTests
    {
        private Mock<IEmployeeRoleRepository<IEmployeeRoleModel>> _mockRepository;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<EmployeeRoleService>> _mockLogger;
        private EmployeeRoleService _service;

        public EmployeeRoleServiceTests()
        {
            _mockRepository = new Mock<IEmployeeRoleRepository<IEmployeeRoleModel>>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<EmployeeRoleService>>();

            _service = new EmployeeRoleService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithNullRepository_ShouldThrowArgumentNullException()
        {
            // Arrange & Act
            Action act = () => new EmployeeRoleService(null!, _mockMapper.Object, _mockLogger.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("repository");
        }

        [Fact]
        public void Constructor_WithNullMapper_ShouldThrowArgumentNullException()
        {
            // Arrange & Act
            Action act = () => new EmployeeRoleService(_mockRepository.Object, null!, _mockLogger.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("mapper");
        }

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateInstance()
        {
            // Arrange & Act
            var service = new EmployeeRoleService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);

            // Assert
            service.Should().NotBeNull();
        }

        #endregion

        #region ListAll Tests

        [Fact]
        public void ListAll_WhenRepositoryReturnsEmptyList_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyRolesList = new List<IEmployeeRoleModel>();
            var emptyDtosList = new List<EmployeeRoleDto>();

            _mockRepository
                .Setup(x => x.ListAll())
                .Returns(emptyRolesList);

            _mockMapper
                .Setup(x => x.Map<IList<EmployeeRoleDto>>(emptyRolesList))
                .Returns(emptyDtosList);

            // Act
            var result = _service.ListAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mockRepository.Verify(x => x.ListAll(), Times.Once);
            _mockMapper.Verify(x => x.Map<IList<EmployeeRoleDto>>(emptyRolesList), Times.Once);
        }

        [Fact]
        public void ListAll_WhenRepositoryReturnsSingleRole_ShouldReturnSingleDto()
        {
            // Arrange
            var role = EmployeeRoleModel.Load(1, "Developer", 2);
            var rolesList = new List<IEmployeeRoleModel> { role };

            var roleDto = new EmployeeRoleDto
            {
                RoleId = 1,
                Name = "Developer",
                Level = 2
            };
            var dtosList = new List<EmployeeRoleDto> { roleDto };

            _mockRepository
                .Setup(x => x.ListAll())
                .Returns(rolesList);

            _mockMapper
                .Setup(x => x.Map<IList<EmployeeRoleDto>>(rolesList))
                .Returns(dtosList);

            // Act
            var result = _service.ListAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].RoleId.Should().Be(1);
            result[0].Name.Should().Be("Developer");
            result[0].Level.Should().Be(2);
            _mockRepository.Verify(x => x.ListAll(), Times.Once);
            _mockMapper.Verify(x => x.Map<IList<EmployeeRoleDto>>(rolesList), Times.Once);
        }

        [Fact]
        public void ListAll_WhenRepositoryReturnsMultipleRoles_ShouldReturnAllDtos()
        {
            // Arrange
            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, "Desenvolvedor Junior", 1),
                EmployeeRoleModel.Load(2, "Desenvolvedor Pleno", 2),
                EmployeeRoleModel.Load(3, "Desenvolvedor Senior", 3),
                EmployeeRoleModel.Load(4, "Tech Lead", 4),
                EmployeeRoleModel.Load(5, "Gerente de Projetos", 5),
                EmployeeRoleModel.Load(6, "Diretor", 6)
            };

            var dtos = new List<EmployeeRoleDto>
            {
                new EmployeeRoleDto { RoleId = 1, Name = "Desenvolvedor Junior", Level = 1 },
                new EmployeeRoleDto { RoleId = 2, Name = "Desenvolvedor Pleno", Level = 2 },
                new EmployeeRoleDto { RoleId = 3, Name = "Desenvolvedor Senior", Level = 3 },
                new EmployeeRoleDto { RoleId = 4, Name = "Tech Lead", Level = 4 },
                new EmployeeRoleDto { RoleId = 5, Name = "Gerente de Projetos", Level = 5 },
                new EmployeeRoleDto { RoleId = 6, Name = "Diretor", Level = 6 }
            };

            _mockRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            _mockMapper
                .Setup(x => x.Map<IList<EmployeeRoleDto>>(roles))
                .Returns(dtos);

            // Act
            var result = _service.ListAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(6);
            result.Should().BeEquivalentTo(dtos, options => options.WithStrictOrdering());
            _mockRepository.Verify(x => x.ListAll(), Times.Once);
            _mockMapper.Verify(x => x.Map<IList<EmployeeRoleDto>>(roles), Times.Once);
        }

        [Fact]
        public void ListAll_WhenRepositoryReturnsRoles_ShouldHaveCorrectHierarchy()
        {
            // Arrange
            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, "Junior", 1),
                EmployeeRoleModel.Load(2, "Pleno", 2),
                EmployeeRoleModel.Load(3, "Senior", 3)
            };

            var dtos = new List<EmployeeRoleDto>
            {
                new EmployeeRoleDto { RoleId = 1, Name = "Junior", Level = 1 },
                new EmployeeRoleDto { RoleId = 2, Name = "Pleno", Level = 2 },
                new EmployeeRoleDto { RoleId = 3, Name = "Senior", Level = 3 }
            };

            _mockRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            _mockMapper
                .Setup(x => x.Map<IList<EmployeeRoleDto>>(roles))
                .Returns(dtos);

            // Act
            var result = _service.ListAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            
            // Verify hierarchy is maintained
            result[0].Level.Should().BeLessThan(result[1].Level);
            result[1].Level.Should().BeLessThan(result[2].Level);
            
            // Verify all levels are positive
            result.Should().OnlyContain(dto => dto.Level > 0);
        }

        [Fact]
        public void ListAll_ShouldCallRepositoryOnlyOnce()
        {
            // Arrange
            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, "Developer", 1)
            };

            var dtos = new List<EmployeeRoleDto>
            {
                new EmployeeRoleDto { RoleId = 1, Name = "Developer", Level = 1 }
            };

            _mockRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            _mockMapper
                .Setup(x => x.Map<IList<EmployeeRoleDto>>(roles))
                .Returns(dtos);

            // Act
            _service.ListAll();

            // Assert
            _mockRepository.Verify(x => x.ListAll(), Times.Once);
        }

        [Fact]
        public void ListAll_ShouldCallMapperWithCorrectParameters()
        {
            // Arrange
            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, "Developer", 1)
            };

            var dtos = new List<EmployeeRoleDto>
            {
                new EmployeeRoleDto { RoleId = 1, Name = "Developer", Level = 1 }
            };

            _mockRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            _mockMapper
                .Setup(x => x.Map<IList<EmployeeRoleDto>>(roles))
                .Returns(dtos);

            // Act
            _service.ListAll();

            // Assert
            _mockMapper.Verify(
                x => x.Map<IList<EmployeeRoleDto>>(It.Is<IEnumerable<IEmployeeRoleModel>>(
                    list => list.Count() == 1 && list.First().RoleId == 1
                )),
                Times.Once
            );
        }

        [Fact]
        public void ListAll_WhenCalledMultipleTimes_ShouldReturnConsistentResults()
        {
            // Arrange
            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, "Developer", 1),
                EmployeeRoleModel.Load(2, "Senior", 2)
            };

            var dtos = new List<EmployeeRoleDto>
            {
                new EmployeeRoleDto { RoleId = 1, Name = "Developer", Level = 1 },
                new EmployeeRoleDto { RoleId = 2, Name = "Senior", Level = 2 }
            };

            _mockRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            _mockMapper
                .Setup(x => x.Map<IList<EmployeeRoleDto>>(roles))
                .Returns(dtos);

            // Act
            var result1 = _service.ListAll();
            var result2 = _service.ListAll();

            // Assert
            result1.Should().BeEquivalentTo(result2);
            _mockRepository.Verify(x => x.ListAll(), Times.Exactly(2));
        }

        [Fact]
        public void ListAll_WithRolesContainingSpecialCharacters_ShouldMapCorrectly()
        {
            // Arrange
            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, "Developer & Designer", 1),
                EmployeeRoleModel.Load(2, "Tech Lead (Senior)", 2)
            };

            var dtos = new List<EmployeeRoleDto>
            {
                new EmployeeRoleDto { RoleId = 1, Name = "Developer & Designer", Level = 1 },
                new EmployeeRoleDto { RoleId = 2, Name = "Tech Lead (Senior)", Level = 2 }
            };

            _mockRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            _mockMapper
                .Setup(x => x.Map<IList<EmployeeRoleDto>>(roles))
                .Returns(dtos);

            // Act
            var result = _service.ListAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Name.Should().Contain("&");
            result[1].Name.Should().Contain("(");
        }

        [Fact]
        public void ListAll_WithRolesHavingLongNames_ShouldMapCorrectly()
        {
            // Arrange
            var longName = new string('A', 80); // Maximum length from validation
            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, longName, 1)
            };

            var dtos = new List<EmployeeRoleDto>
            {
                new EmployeeRoleDto { RoleId = 1, Name = longName, Level = 1 }
            };

            _mockRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            _mockMapper
                .Setup(x => x.Map<IList<EmployeeRoleDto>>(roles))
                .Returns(dtos);

            // Act
            var result = _service.ListAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Name.Should().HaveLength(80);
        }

        [Fact]
        public void ListAll_WithMaximumLevelRoles_ShouldMapCorrectly()
        {
            // Arrange
            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, "Executive", 100) // Maximum level from validation
            };

            var dtos = new List<EmployeeRoleDto>
            {
                new EmployeeRoleDto { RoleId = 1, Name = "Executive", Level = 100 }
            };

            _mockRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            _mockMapper
                .Setup(x => x.Map<IList<EmployeeRoleDto>>(roles))
                .Returns(dtos);

            // Act
            var result = _service.ListAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result[0].Level.Should().Be(100);
        }

        #endregion

        #region Integration-like Tests

        [Fact]
        public void ListAll_WithCompleteRoleHierarchy_ShouldReturnOrderedByLevel()
        {
            // Arrange - Simulating the complete hierarchy from the database
            var roles = new List<IEmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, "Desenvolvedor Junior", 1),
                EmployeeRoleModel.Load(2, "Desenvolvedor Pleno", 2),
                EmployeeRoleModel.Load(3, "Desenvolvedor Senior", 3),
                EmployeeRoleModel.Load(4, "Tech Lead", 4),
                EmployeeRoleModel.Load(5, "Gerente de Projetos", 5),
                EmployeeRoleModel.Load(6, "Diretor", 6)
            };

            var dtos = roles.Select(r => new EmployeeRoleDto
            {
                RoleId = r.RoleId,
                Name = r.Name,
                Level = r.Level
            }).ToList();

            _mockRepository
                .Setup(x => x.ListAll())
                .Returns(roles);

            _mockMapper
                .Setup(x => x.Map<IList<EmployeeRoleDto>>(roles))
                .Returns(dtos);

            // Act
            var result = _service.ListAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(6);
            
            // Verify each role is correctly mapped
            for (int i = 0; i < result.Count; i++)
            {
                result[i].RoleId.Should().Be(roles[i].RoleId);
                result[i].Name.Should().Be(roles[i].Name);
                result[i].Level.Should().Be(roles[i].Level);
            }
            
            // Verify hierarchy is maintained
            for (int i = 0; i < result.Count - 1; i++)
            {
                result[i].Level.Should().BeLessThan(result[i + 1].Level);
            }
        }

        #endregion
    }
}
