using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IEmployeeRepository<IEmployeeModel> employeeRepository, 
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEmployeeModel?> AuthenticateAsync(string email, string password)
        {
            _logger.LogInformation("Authentication attempt for email: {Email}", email);

            try
            {
                var employee = await _employeeRepository.GetByEmailAsync(email);

                if (employee == null)
                {
                    _logger.LogWarning("Authentication failed: Employee not found for email: {Email}", email);
                    return null;
                }

                // Verifica se a senha corresponde (assumindo que a senha está hasheada com SHA256)
                if (!VerifyPassword(password, employee.Password))
                {
                    _logger.LogWarning("Authentication failed: Invalid password for email: {Email}", email);
                    return null;
                }

                _logger.LogInformation("Authentication successful for employee: {EmployeeId} - {Email}", employee.EmployeeId, employee.Email);
                return employee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication for email: {Email}", email);
                throw;
            }
        }

        public string GenerateJwtToken(IEmployeeModel employee)
        {
            _logger.LogDebug("Generating JWT token for employee: {EmployeeId} - {Email}", employee.EmployeeId, employee.Email);

            try
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

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                _logger.LogInformation("JWT token generated successfully for employee: {EmployeeId} - Expires at: {ExpirationTime}", 
                    employee.EmployeeId, token.ValidTo);

                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token for employee: {EmployeeId}", employee.EmployeeId);
                throw;
            }
        }

        public string HashPassword(string password)
        {
            _logger.LogDebug("Hashing password");

            try
            {
                using var sha256 = SHA256.Create();
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hash = Convert.ToBase64String(bytes);

                _logger.LogDebug("Password hashed successfully");
                return hash;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error hashing password");
                throw;
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            _logger.LogDebug("Verifying password");

            try
            {
                // Hash a senha fornecida e compare com a armazenada
                var hashedPassword = HashPassword(enteredPassword);
                var isValid = hashedPassword == storedPasswordHash;

                _logger.LogDebug("Password verification result: {IsValid}", isValid);
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying password");
                throw;
            }
        }
    }
}