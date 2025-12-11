namespace MoutsTI.Dtos
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public EmployeeDto Employee { get; set; } = null!;
    }
}