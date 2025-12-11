using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MoutsTI.Domain.Entities.Interfaces;
using MoutsTI.Domain.Services.Interfaces;
using MoutsTI.Infra.Interfaces.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MoutsTI.Domain.Services
{
    public class AuthService : IAuthService
    {
        private readonly IEmployeeRepository<IEmployeeModel> _employeeRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IEmployeeRepository<IEmployeeModel> employeeRepository, IConfiguration configuration)
        {
            _employeeRepository = employeeRepository;
            _configuration = configuration;
        }

        public async Task<IEmployeeModel?> AuthenticateAsync(string email, string password)
        {
            var employee = await _employeeRepository.GetByEmailAsync(email);

            if (employee == null)
                return null;

            // Verifica se a senha corresponde (assumindo que a senha está hasheada com SHA256)
            if (!VerifyPassword(password, employee.Password))
                return null;

            return employee;
        }

        public string GenerateJwtToken(IEmployeeModel employee)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var issuer = jwtSettings["Issuer"] ?? "MoutsTI.API";
            var audience = jwtSettings["Audience"] ?? "MoutsTI.Client";
            var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, employee.EmployeeId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, employee.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, employee.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, employee.LastName),
                new Claim("RoleId", employee.RoleId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            // Hash a senha fornecida e compare com a armazenada
            var hashedPassword = HashPassword(enteredPassword);
            return hashedPassword == storedPasswordHash;
        }
    }
}