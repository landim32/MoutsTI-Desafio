using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoutsTI.Domain.Services.Interfaces;
using MoutsTI.Dtos;

namespace MoutsTI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IEmployeeService employeeService, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtém todos os funcionários
        /// </summary>
        /// <returns>Lista de funcionários</returns>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(IList<EmployeeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAll()
        {
            try
            {
                var employees = _employeeService.ListAll();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all employees");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while retrieving employees." });
            }
        }

        /// <summary>
        /// Obtém um funcionário por ID
        /// </summary>
        /// <param name="id">ID do funcionário</param>
        /// <returns>Funcionário encontrado</returns>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetById(long id)
        {
            try
            {
                var employee = _employeeService.GetById(id);

                if (employee == null)
                {
                    return NotFound(new { message = $"Employee with ID {id} not found." });
                }

                return Ok(employee);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for GetById: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee with ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while retrieving the employee." });
            }
        }

        /// <summary>
        /// Cria um novo funcionário
        /// </summary>
        /// <param name="employee">Dados do funcionário</param>
        /// <returns>ID do funcionário criado</returns>
        //[Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Create([FromBody] EmployeeDto employee)
        {
            if (employee == null)
            {
                return BadRequest(new { message = "Employee data is required." });
            }

            try
            {
                var employeeId = _employeeService.Add(employee);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = employeeId },
                    new { employeeId, message = "Employee created successfully." });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating employee");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while creating the employee." });
            }
        }

        /// <summary>
        /// Atualiza um funcionário existente
        /// </summary>
        /// <param name="id">ID do funcionário</param>
        /// <param name="employee">Dados atualizados do funcionário</param>
        /// <returns>Resultado da operação</returns>
        //[Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Update(long id, [FromBody] EmployeeDto employee)
        {
            if (employee == null)
            {
                return BadRequest(new { message = "Employee data is required." });
            }

            if (id != employee.EmployeeId)
            {
                return BadRequest(new { message = "ID in URL does not match ID in body." });
            }

            try
            {
                _employeeService.Update(employee);
                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                _logger.LogWarning(ex, "Employee not found for update: {Id}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while updating employee: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument while updating employee: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee with ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while updating the employee." });
            }
        }

        /// <summary>
        /// Exclui um funcionário
        /// </summary>
        /// <param name="id">ID do funcionário</param>
        /// <returns>Resultado da operação</returns>
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(long id)
        {
            try
            {
                _employeeService.Delete(id);
                return NoContent();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
            {
                _logger.LogWarning(ex, "Employee not found for deletion: {Id}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument for delete: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting employee with ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while deleting the employee." });
            }
        }
    }
}
