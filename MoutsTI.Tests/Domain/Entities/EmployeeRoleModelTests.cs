using FluentAssertions;
using MoutsTI.Domain.Entities;

namespace MoutsTI.Tests.Domain.Entities
{
    public class EmployeeRoleModelTests
    {
        #region Create Tests

        [Fact]
        public void Create_WithValidParameters_ShouldCreateInstance()
        {
            // Arrange
            var name = "Developer";
            var level = 3;

            // Act
            var role = EmployeeRoleModel.Create(name, level);

            // Assert
            role.Should().NotBeNull();
            role.Name.Should().Be("Developer");
            role.Level.Should().Be(3);
            role.RoleId.Should().Be(0);
        }

        [Fact]
        public void Create_WithMinimumLevel_ShouldCreateInstance()
        {
            // Arrange
            var name = "Intern";
            var level = 1;

            // Act
            var role = EmployeeRoleModel.Create(name, level);

            // Assert
            role.Should().NotBeNull();
            role.Name.Should().Be("Intern");
            role.Level.Should().Be(1);
        }

        [Fact]
        public void Create_WithMaximumLevel_ShouldCreateInstance()
        {
            // Arrange
            var name = "Executive";
            var level = 100;

            // Act
            var role = EmployeeRoleModel.Create(name, level);

            // Assert
            role.Should().NotBeNull();
            role.Name.Should().Be("Executive");
            role.Level.Should().Be(100);
        }

        [Fact]
        public void Create_WithEmptyName_ShouldThrowArgumentException()
        {
            // Arrange
            var name = "";
            var level = 3;

            // Act
            Action act = () => EmployeeRoleModel.Create(name, level);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Role name cannot be empty*")
                .WithParameterName("name");
        }

        [Fact]
        public void Create_WithWhitespaceName_ShouldThrowArgumentException()
        {
            // Arrange
            var name = "   ";
            var level = 3;

            // Act
            Action act = () => EmployeeRoleModel.Create(name, level);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Role name cannot be empty*")
                .WithParameterName("name");
        }

        [Fact]
        public void Create_WithNullName_ShouldThrowArgumentException()
        {
            // Arrange
            string name = null!;
            var level = 3;

            // Act
            Action act = () => EmployeeRoleModel.Create(name, level);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Role name cannot be empty*")
                .WithParameterName("name");
        }

        [Fact]
        public void Create_WithNameExceeding80Characters_ShouldThrowArgumentException()
        {
            // Arrange
            var name = new string('A', 81);
            var level = 3;

            // Act
            Action act = () => EmployeeRoleModel.Create(name, level);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Role name cannot exceed 80 characters*")
                .WithParameterName("name");
        }

        [Fact]
        public void Create_WithNameExactly80Characters_ShouldCreateInstance()
        {
            // Arrange
            var name = new string('A', 80);
            var level = 3;

            // Act
            var role = EmployeeRoleModel.Create(name, level);

            // Assert
            role.Should().NotBeNull();
            role.Name.Should().HaveLength(80);
        }

        [Fact]
        public void Create_WithZeroLevel_ShouldThrowArgumentException()
        {
            // Arrange
            var name = "Developer";
            var level = 0;

            // Act
            Action act = () => EmployeeRoleModel.Create(name, level);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Role level must be greater than or equal to 1*")
                .WithParameterName("level");
        }

        [Fact]
        public void Create_WithNegativeLevel_ShouldThrowArgumentException()
        {
            // Arrange
            var name = "Developer";
            var level = -1;

            // Act
            Action act = () => EmployeeRoleModel.Create(name, level);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Role level must be greater than or equal to 1*")
                .WithParameterName("level");
        }

        [Fact]
        public void Create_WithLevelExceeding100_ShouldThrowArgumentException()
        {
            // Arrange
            var name = "Developer";
            var level = 101;

            // Act
            Action act = () => EmployeeRoleModel.Create(name, level);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Role level cannot exceed 100*")
                .WithParameterName("level");
        }

        [Fact]
        public void Create_WithSpecialCharactersInName_ShouldCreateInstance()
        {
            // Arrange
            var name = "Tech Lead (Senior) - Level 4";
            var level = 4;

            // Act
            var role = EmployeeRoleModel.Create(name, level);

            // Assert
            role.Should().NotBeNull();
            role.Name.Should().Be("Tech Lead (Senior) - Level 4");
        }

        #endregion

        #region Load Tests

        [Fact]
        public void Load_WithValidParameters_ShouldCreateInstance()
        {
            // Arrange
            var roleId = 1L;
            var name = "Developer";
            var level = 3;

            // Act
            var role = EmployeeRoleModel.Load(roleId, name, level);

            // Assert
            role.Should().NotBeNull();
            role.RoleId.Should().Be(1);
            role.Name.Should().Be("Developer");
            role.Level.Should().Be(3);
        }

        [Fact]
        public void Load_WithZeroRoleId_ShouldCreateInstance()
        {
            // Arrange
            var roleId = 0L;
            var name = "Developer";
            var level = 3;

            // Act
            var role = EmployeeRoleModel.Load(roleId, name, level);

            // Assert
            role.Should().NotBeNull();
            role.RoleId.Should().Be(0);
        }

        [Fact]
        public void Load_WithNegativeRoleId_ShouldCreateInstance()
        {
            // Arrange
            var roleId = -1L;
            var name = "Developer";
            var level = 3;

            // Act
            var role = EmployeeRoleModel.Load(roleId, name, level);

            // Assert
            role.Should().NotBeNull();
            role.RoleId.Should().Be(-1);
        }

        [Fact]
        public void Load_WithEmptyName_ShouldThrowArgumentException()
        {
            // Arrange
            var roleId = 1L;
            var name = "";
            var level = 3;

            // Act
            Action act = () => EmployeeRoleModel.Load(roleId, name, level);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Role name cannot be empty*")
                .WithParameterName("name");
        }

        [Fact]
        public void Load_WithInvalidLevel_ShouldThrowArgumentException()
        {
            // Arrange
            var roleId = 1L;
            var name = "Developer";
            var level = 0;

            // Act
            Action act = () => EmployeeRoleModel.Load(roleId, name, level);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Role level must be greater than or equal to 1*")
                .WithParameterName("level");
        }

        [Fact]
        public void Load_WithCompleteHierarchy_ShouldCreateAllRoles()
        {
            // Arrange & Act
            var roles = new[]
            {
                EmployeeRoleModel.Load(1, "Desenvolvedor Junior", 1),
                EmployeeRoleModel.Load(2, "Desenvolvedor Pleno", 2),
                EmployeeRoleModel.Load(3, "Desenvolvedor Senior", 3),
                EmployeeRoleModel.Load(4, "Tech Lead", 4),
                EmployeeRoleModel.Load(5, "Gerente de Projetos", 5),
                EmployeeRoleModel.Load(6, "Diretor", 6)
            };

            // Assert
            roles.Should().HaveCount(6);
            roles.Should().OnlyContain(r => r != null);
            roles.Should().BeInAscendingOrder(r => r.Level);
        }

        #endregion

        #region UpdateName Tests

        [Fact]
        public void UpdateName_WithValidName_ShouldUpdateName()
        {
            // Arrange
            var role = EmployeeRoleModel.Create("Developer", 3);
            var newName = "Senior Developer";

            // Act
            role.UpdateName(newName);

            // Assert
            role.Name.Should().Be("Senior Developer");
        }

        [Fact]
        public void UpdateName_WithEmptyName_ShouldThrowArgumentException()
        {
            // Arrange
            var role = EmployeeRoleModel.Create("Developer", 3);

            // Act
            Action act = () => role.UpdateName("");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Role name cannot be empty*")
                .WithParameterName("name");
        }

        [Fact]
        public void UpdateName_WithNullName_ShouldThrowArgumentException()
        {
            // Arrange
            var role = EmployeeRoleModel.Create("Developer", 3);

            // Act
            Action act = () => role.UpdateName(null!);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Role name cannot be empty*")
                .WithParameterName("name");
        }

        [Fact]
        public void UpdateName_WithNameExceeding80Characters_ShouldThrowArgumentException()
        {
            // Arrange
            var role = EmployeeRoleModel.Create("Developer", 3);
            var newName = new string('A', 81);

            // Act
            Action act = () => role.UpdateName(newName);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Role name cannot exceed 80 characters*")
                .WithParameterName("name");
        }

        [Fact]
        public void UpdateName_MultipleTimes_ShouldKeepLastValue()
        {
            // Arrange
            var role = EmployeeRoleModel.Create("Developer", 3);

            // Act
            role.UpdateName("Senior Developer");
            role.UpdateName("Lead Developer");
            role.UpdateName("Principal Developer");

            // Assert
            role.Name.Should().Be("Principal Developer");
        }

        #endregion

        #region UpdateLevel Tests

        [Fact]
        public void UpdateLevel_WithValidLevel_ShouldUpdateLevel()
        {
            // Arrange
            var role = EmployeeRoleModel.Create("Developer", 3);
            var newLevel = 5;

            // Act
            role.UpdateLevel(newLevel);

            // Assert
            role.Level.Should().Be(5);
        }

        [Fact]
        public void UpdateLevel_ToMinimumLevel_ShouldUpdateLevel()
        {
            // Arrange
            var role = EmployeeRoleModel.Create("Developer", 3);

            // Act
            role.UpdateLevel(1);

            // Assert
            role.Level.Should().Be(1);
        }

        [Fact]
        public void UpdateLevel_ToMaximumLevel_ShouldUpdateLevel()
        {
            // Arrange
            var role = EmployeeRoleModel.Create("Developer", 3);

            // Act
            role.UpdateLevel(100);

            // Assert
            role.Level.Should().Be(100);
        }

        [Fact]
        public void UpdateLevel_WithZeroLevel_ShouldThrowArgumentException()
        {
            // Arrange
            var role = EmployeeRoleModel.Create("Developer", 3);

            // Act
            Action act = () => role.UpdateLevel(0);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Role level must be greater than or equal to 1*")
                .WithParameterName("level");
        }

        [Fact]
        public void UpdateLevel_WithNegativeLevel_ShouldThrowArgumentException()
        {
            // Arrange
            var role = EmployeeRoleModel.Create("Developer", 3);

            // Act
            Action act = () => role.UpdateLevel(-1);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Role level must be greater than or equal to 1*")
                .WithParameterName("level");
        }

        [Fact]
        public void UpdateLevel_WithLevelExceeding100_ShouldThrowArgumentException()
        {
            // Arrange
            var role = EmployeeRoleModel.Create("Developer", 3);

            // Act
            Action act = () => role.UpdateLevel(101);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Role level cannot exceed 100*")
                .WithParameterName("level");
        }

        [Fact]
        public void UpdateLevel_MultipleTimes_ShouldKeepLastValue()
        {
            // Arrange
            var role = EmployeeRoleModel.Create("Developer", 3);

            // Act
            role.UpdateLevel(5);
            role.UpdateLevel(7);
            role.UpdateLevel(10);

            // Assert
            role.Level.Should().Be(10);
        }

        [Fact]
        public void UpdateLevel_ToSameLevel_ShouldKeepValue()
        {
            // Arrange
            var role = EmployeeRoleModel.Create("Developer", 3);

            // Act
            role.UpdateLevel(3);

            // Assert
            role.Level.Should().Be(3);
        }

        #endregion

        #region CanBeDeleted Tests

        [Fact]
        public void CanBeDeleted_ShouldReturnTrue()
        {
            // Arrange
            var role = EmployeeRoleModel.Create("Developer", 3);

            // Act
            var canBeDeleted = role.CanBeDeleted();

            // Assert
            canBeDeleted.Should().BeTrue();
        }

        [Fact]
        public void CanBeDeleted_ForNewRole_ShouldReturnTrue()
        {
            // Arrange
            var role = EmployeeRoleModel.Create("Intern", 1);

            // Act
            var canBeDeleted = role.CanBeDeleted();

            // Assert
            canBeDeleted.Should().BeTrue();
        }

        [Fact]
        public void CanBeDeleted_ForLoadedRole_ShouldReturnTrue()
        {
            // Arrange
            var role = EmployeeRoleModel.Load(1, "Developer", 3);

            // Act
            var canBeDeleted = role.CanBeDeleted();

            // Assert
            canBeDeleted.Should().BeTrue();
        }

        #endregion

        #region Equals Tests

        [Fact]
        public void Equals_WithSameRoleId_ShouldReturnTrue()
        {
            // Arrange
            var role1 = EmployeeRoleModel.Load(1, "Developer", 3);
            var role2 = EmployeeRoleModel.Load(1, "Senior Developer", 5);

            // Act
            var areEqual = role1.Equals(role2);

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void Equals_WithDifferentRoleId_ShouldReturnFalse()
        {
            // Arrange
            var role1 = EmployeeRoleModel.Load(1, "Developer", 3);
            var role2 = EmployeeRoleModel.Load(2, "Developer", 3);

            // Act
            var areEqual = role1.Equals(role2);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var role = EmployeeRoleModel.Load(1, "Developer", 3);

            // Act
            var areEqual = role.Equals(null);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void Equals_WithDifferentType_ShouldReturnFalse()
        {
            // Arrange
            var role = EmployeeRoleModel.Load(1, "Developer", 3);
            var other = new object();

            // Act
            var areEqual = role.Equals(other);

            // Assert
            areEqual.Should().BeFalse();
        }

        [Fact]
        public void Equals_WithSameInstance_ShouldReturnTrue()
        {
            // Arrange
            var role = EmployeeRoleModel.Load(1, "Developer", 3);

            // Act
            var areEqual = role.Equals(role);

            // Assert
            areEqual.Should().BeTrue();
        }

        [Fact]
        public void Equals_WithBothRoleIdZero_ShouldReturnTrue()
        {
            // Arrange
            var role1 = EmployeeRoleModel.Create("Developer", 3);
            var role2 = EmployeeRoleModel.Create("Senior", 5);

            // Act
            var areEqual = role1.Equals(role2);

            // Assert
            areEqual.Should().BeTrue(); // Both have RoleId = 0
        }

        #endregion

        #region GetHashCode Tests

        [Fact]
        public void GetHashCode_WithSameRoleId_ShouldReturnSameHashCode()
        {
            // Arrange
            var role1 = EmployeeRoleModel.Load(1, "Developer", 3);
            var role2 = EmployeeRoleModel.Load(1, "Senior Developer", 5);

            // Act
            var hash1 = role1.GetHashCode();
            var hash2 = role2.GetHashCode();

            // Assert
            hash1.Should().Be(hash2);
        }

        [Fact]
        public void GetHashCode_WithDifferentRoleId_ShouldReturnDifferentHashCode()
        {
            // Arrange
            var role1 = EmployeeRoleModel.Load(1, "Developer", 3);
            var role2 = EmployeeRoleModel.Load(2, "Developer", 3);

            // Act
            var hash1 = role1.GetHashCode();
            var hash2 = role2.GetHashCode();

            // Assert
            hash1.Should().NotBe(hash2);
        }

        [Fact]
        public void GetHashCode_ShouldBeConsistent()
        {
            // Arrange
            var role = EmployeeRoleModel.Load(1, "Developer", 3);

            // Act
            var hash1 = role.GetHashCode();
            var hash2 = role.GetHashCode();
            var hash3 = role.GetHashCode();

            // Assert
            hash1.Should().Be(hash2);
            hash2.Should().Be(hash3);
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_ShouldReturnFormattedString()
        {
            // Arrange
            var role = EmployeeRoleModel.Load(1, "Developer", 3);

            // Act
            var result = role.ToString();

            // Assert
            result.Should().Be("Developer (Nível 3)");
        }

        [Fact]
        public void ToString_WithDifferentLevel_ShouldReturnFormattedString()
        {
            // Arrange
            var role = EmployeeRoleModel.Load(2, "Senior Developer", 5);

            // Act
            var result = role.ToString();

            // Assert
            result.Should().Be("Senior Developer (Nível 5)");
        }

        [Fact]
        public void ToString_WithMinimumLevel_ShouldReturnFormattedString()
        {
            // Arrange
            var role = EmployeeRoleModel.Load(1, "Intern", 1);

            // Act
            var result = role.ToString();

            // Assert
            result.Should().Be("Intern (Nível 1)");
        }

        [Fact]
        public void ToString_WithMaximumLevel_ShouldReturnFormattedString()
        {
            // Arrange
            var role = EmployeeRoleModel.Load(1, "Executive", 100);

            // Act
            var result = role.ToString();

            // Assert
            result.Should().Be("Executive (Nível 100)");
        }

        [Fact]
        public void ToString_WithLongName_ShouldReturnFormattedString()
        {
            // Arrange
            var longName = "Senior Software Development Engineer - Team Lead";
            var role = EmployeeRoleModel.Load(1, longName, 7);

            // Act
            var result = role.ToString();

            // Assert
            result.Should().Be($"{longName} (Nível 7)");
        }

        [Fact]
        public void ToString_AfterUpdateName_ShouldReturnUpdatedString()
        {
            // Arrange
            var role = EmployeeRoleModel.Load(1, "Developer", 3);

            // Act
            role.UpdateName("Senior Developer");
            var result = role.ToString();

            // Assert
            result.Should().Be("Senior Developer (Nível 3)");
        }

        [Fact]
        public void ToString_AfterUpdateLevel_ShouldReturnUpdatedString()
        {
            // Arrange
            var role = EmployeeRoleModel.Load(1, "Developer", 3);

            // Act
            role.UpdateLevel(5);
            var result = role.ToString();

            // Assert
            result.Should().Be("Developer (Nível 5)");
        }

        #endregion

        #region Business Logic Tests

        [Fact]
        public void Role_ShouldMaintainImmutableRoleId()
        {
            // Arrange
            var role = EmployeeRoleModel.Load(1, "Developer", 3);
            var originalRoleId = role.RoleId;

            // Act
            role.UpdateName("Senior Developer");
            role.UpdateLevel(5);

            // Assert
            role.RoleId.Should().Be(originalRoleId);
        }

        [Fact]
        public void Role_WithBoundaryValues_ShouldWork()
        {
            // Arrange & Act
            var minRole = EmployeeRoleModel.Create("A", 1);
            var maxRole = EmployeeRoleModel.Create(new string('Z', 80), 100);

            // Assert
            minRole.Should().NotBeNull();
            minRole.Name.Should().Be("A");
            minRole.Level.Should().Be(1);

            maxRole.Should().NotBeNull();
            maxRole.Name.Should().HaveLength(80);
            maxRole.Level.Should().Be(100);
        }

        [Fact]
        public void Role_CreatedWithCreate_ShouldHaveZeroRoleId()
        {
            // Arrange & Act
            var role = EmployeeRoleModel.Create("Developer", 3);

            // Assert
            role.RoleId.Should().Be(0);
        }

        [Fact]
        public void Role_LoadedWithLoad_ShouldPreserveRoleId()
        {
            // Arrange & Act
            var role = EmployeeRoleModel.Load(42, "Developer", 3);

            // Assert
            role.RoleId.Should().Be(42);
        }

        [Fact]
        public void Role_AfterMultipleUpdates_ShouldMaintainConsistency()
        {
            // Arrange
            var role = EmployeeRoleModel.Load(1, "Developer", 3);

            // Act
            role.UpdateName("Senior Developer");
            role.UpdateLevel(5);
            role.UpdateName("Lead Developer");
            role.UpdateLevel(7);

            // Assert
            role.RoleId.Should().Be(1);
            role.Name.Should().Be("Lead Developer");
            role.Level.Should().Be(7);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(25)]
        [InlineData(50)]
        [InlineData(75)]
        [InlineData(100)]
        public void Role_WithVariousLevels_ShouldCreateSuccessfully(int level)
        {
            // Arrange & Act
            var role = EmployeeRoleModel.Create("Role", level);

            // Assert
            role.Should().NotBeNull();
            role.Level.Should().Be(level);
        }

        [Theory]
        [InlineData("Developer")]
        [InlineData("Senior Developer")]
        [InlineData("Tech Lead")]
        [InlineData("Engineering Manager")]
        [InlineData("Director of Engineering")]
        public void Role_WithVariousNames_ShouldCreateSuccessfully(string name)
        {
            // Arrange & Act
            var role = EmployeeRoleModel.Create(name, 3);

            // Assert
            role.Should().NotBeNull();
            role.Name.Should().Be(name);
        }

        #endregion

        #region Collection Tests

        [Fact]
        public void Roles_InList_CanBeComparedByRoleId()
        {
            // Arrange
            var roles = new List<EmployeeRoleModel>
            {
                EmployeeRoleModel.Load(3, "Senior", 3),
                EmployeeRoleModel.Load(1, "Junior", 1),
                EmployeeRoleModel.Load(2, "Pleno", 2)
            };

            // Act
            var sortedRoles = roles.OrderBy(r => r.RoleId).ToList();

            // Assert
            sortedRoles[0].RoleId.Should().Be(1);
            sortedRoles[1].RoleId.Should().Be(2);
            sortedRoles[2].RoleId.Should().Be(3);
        }

        [Fact]
        public void Roles_InList_CanBeSortedByLevel()
        {
            // Arrange
            var roles = new List<EmployeeRoleModel>
            {
                EmployeeRoleModel.Load(1, "Tech Lead", 4),
                EmployeeRoleModel.Load(2, "Junior", 1),
                EmployeeRoleModel.Load(3, "Senior", 3),
                EmployeeRoleModel.Load(4, "Pleno", 2)
            };

            // Act
            var sortedRoles = roles.OrderBy(r => r.Level).ToList();

            // Assert
            sortedRoles[0].Level.Should().Be(1);
            sortedRoles[1].Level.Should().Be(2);
            sortedRoles[2].Level.Should().Be(3);
            sortedRoles[3].Level.Should().Be(4);
        }

        [Fact]
        public void Roles_InHashSet_ShouldUseDuplicatesByRoleId()
        {
            // Arrange
            var role1 = EmployeeRoleModel.Load(1, "Developer", 3);
            var role2 = EmployeeRoleModel.Load(1, "Different Name", 5);
            var hashSet = new HashSet<EmployeeRoleModel>();

            // Act
            hashSet.Add(role1);
            hashSet.Add(role2);

            // Assert
            hashSet.Should().HaveCount(1); // Same RoleId, should be treated as duplicate
        }

        [Fact]
        public void Roles_InDictionary_CanBeKeyedByRoleId()
        {
            // Arrange
            var roles = new Dictionary<long, EmployeeRoleModel>
            {
                { 1, EmployeeRoleModel.Load(1, "Junior", 1) },
                { 2, EmployeeRoleModel.Load(2, "Pleno", 2) },
                { 3, EmployeeRoleModel.Load(3, "Senior", 3) }
            };

            // Act
            var juniorRole = roles[1];

            // Assert
            juniorRole.Name.Should().Be("Junior");
            juniorRole.Level.Should().Be(1);
        }

        #endregion
    }
}
