using System.Text.Json.Serialization;

namespace MoutsTI.Dtos
{
    public class EmployeeDto : PersonDto
    {
        [JsonPropertyName("docNumber")]
        public string DocNumber { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("birthday")]
        public DateTime Birthday { get; set; }

        [JsonPropertyName("roleId")]
        public long RoleId { get; set; }

        [JsonPropertyName("managerId")]
        public long? ManagerId { get; set; }

        [JsonPropertyName("role")]
        public EmployeeRoleDto? Role { get; set; }

        [JsonPropertyName("manager")]
        public ManagerDto? Manager { get; set; }

        [JsonPropertyName("phones")]
        public List<string> Phones { get; set; } = new();
    }
}
