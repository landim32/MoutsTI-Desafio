using MoutsTI.Domain.Entities.Interfaces;

namespace MoutsTI.Domain.Entities
{
    public class EmployeeRoleModel : IEmployeeRoleModel
    {
        public long RoleId { get; private set; }
        public string Name { get; private set; }
        public int Level { get; private set; }

        // Construtor privado para garantir que o objeto seja criado apenas através de métodos de fábrica
        private EmployeeRoleModel(long roleId, string name, int level)
        {
            RoleId = roleId;
            Name = name;
            Level = level;
        }

        // Construtor para criação de nova role (sem ID)
        private EmployeeRoleModel(string name, int level)
        {
            ValidateName(name);
            ValidateLevel(level);

            Name = name;
            Level = level;
        }

        // Factory method para criar uma nova role
        public static EmployeeRoleModel Create(string name, int level)
        {
            return new EmployeeRoleModel(name, level);
        }

        // Factory method para reconstruir uma role existente (da base de dados)
        public static EmployeeRoleModel Load(long roleId, string name, int level)
        {
            ValidateName(name);
            ValidateLevel(level);

            return new EmployeeRoleModel(roleId, name, level);
        }

        // Métodos de negócio para alterar propriedades
        public void UpdateName(string name)
        {
            ValidateName(name);
            Name = name;
        }

        public void UpdateLevel(int level)
        {
            ValidateLevel(level);
            Level = level;
        }

        // Validações de regras de negócio
        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Role name cannot be empty.", nameof(name));

            if (name.Length > 80)
                throw new ArgumentException("Role name cannot exceed 80 characters.", nameof(name));
        }

        private static void ValidateLevel(int level)
        {
            if (level < 1)
                throw new ArgumentException("Role level must be greater than or equal to 1.", nameof(level));

            if (level > 100)
                throw new ArgumentException("Role level cannot exceed 100.", nameof(level));
        }

        // Método para validar se a role pode ser excluída
        public bool CanBeDeleted()
        {
            // Regra de negócio: pode adicionar lógica adicional aqui
            // Por exemplo, verificar se não há funcionários associados
            return true;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not EmployeeRoleModel other)
                return false;

            return RoleId == other.RoleId;
        }

        public override int GetHashCode()
        {
            return RoleId.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Name} (Level {Level})";
        }
    }
}
