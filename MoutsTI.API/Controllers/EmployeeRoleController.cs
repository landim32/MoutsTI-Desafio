using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoutsTI.Domain.Services.Interfaces;
using MoutsTI.Dtos;

namespace MoutsTI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeRoleController : ControllerBase
    {
        private readonly IEmployeeRoleService _roleService;
        private readonly ILogger<EmployeeRoleController> _logger;

        public EmployeeRoleController(IEmployeeRoleService roleService, ILogger<EmployeeRoleController> logger)
        {
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(IList<EmployeeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAll()
        {
            try
            {
                var roles = _roleService.ListAll();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all roles");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while retrieving roles." });
            }
        }
    }
}
