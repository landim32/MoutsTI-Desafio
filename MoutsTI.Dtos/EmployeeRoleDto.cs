using System.Text.Json.Serialization;

namespace MoutsTI.Dtos
{
    public class EmployeeRoleDto
    {
        [JsonPropertyName("roleId")]
        public long RoleId { get; set; }
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("level")]
        public int Level { get; set; }
    }
}
