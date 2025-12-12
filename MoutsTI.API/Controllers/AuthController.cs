using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoutsTI.Domain.Services.Interfaces;
using MoutsTI.Dtos;

namespace MoutsTI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService, 
            IMapper mapper, 
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for login request. Email: {Email}", loginDto.Email);
                return BadRequest(ModelState);
            }

            try
            {
                var employee = await _authService.AuthenticateAsync(loginDto.Email, loginDto.Password);

                if (employee == null)
                {
                    _logger.LogWarning("Login failed for email: {Email} - Invalid credentials", loginDto.Email);
                    return Unauthorized(new { message = "Email ou senha inválidos." });
                }

                var token = _authService.GenerateJwtToken(employee);
                var jwtSettings = _configuration.GetSection("JwtSettings");
                var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "60");

                var response = new LoginResponseDto
                {
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes),
                    Employee = _mapper.Map<EmployeeDto>(employee)
                };

                _logger.LogInformation("Login successful for employee: {EmployeeId} - {Email}", employee.EmployeeId, employee.Email);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", loginDto.Email);
                return StatusCode(500, new { message = "Erro interno ao processar o login." });
            }
        }

        [HttpGet("validate")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            var employeeId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            _logger.LogDebug("Token validation request. EmployeeId: {EmployeeId}, Email: {Email}", employeeId, email);

            try
            {
                _logger.LogInformation("Token validated successfully for employee: {EmployeeId}", employeeId);

                return Ok(new
                {
                    valid = true,
                    employeeId,
                    email
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token for employee: {EmployeeId}", employeeId);
                return StatusCode(500, new { message = "Erro ao validar token." });
            }
        }
    }
}