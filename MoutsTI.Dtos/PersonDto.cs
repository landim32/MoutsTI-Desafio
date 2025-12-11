using System.Text.Json.Serialization;

namespace MoutsTI.Dtos
{
    public class PersonDto
    {
        [JsonPropertyName("employeeId")]
        public long EmployeeId { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("fullName")]
        public string FullName { get; set; } = string.Empty;
    }
}
