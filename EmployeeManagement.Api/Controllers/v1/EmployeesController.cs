using Asp.Versioning;
using EmployeeManagement.Api.Middleware;
using EmployeeManagement.Application.Contracts.Common;
using EmployeeManagement.Application.Contracts.Employees;
using EmployeeManagement.Application.Services.Employees.Commands;
using EmployeeManagement.Application.Services.Employees.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.AspNetCore.RateLimiting;

namespace EmployeeManagement.Api.Controllers.V1;

/// <summary>
/// Manages employee records.
/// </summary>
/// <remarks>
/// Authentication is not wired into this API yet, so no <c>[Authorize]</c> attribute is applied;
/// add the JWT bearer middleware first and the endpoints can then be locked down the same way the
/// rest of the surface will be.
/// <para>
/// The read endpoints are served from the output cache, see <see cref="CachingExtensions"/>. Every
/// successful write evicts the <see cref="CachingExtensions.EmployeesTag"/> tag, so a cached
/// response can never outlive the data it was built from.
/// </para>
/// </remarks>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/employees")]
[Produces("application/json")]
[Tags("Employees")]
public sealed class EmployeesController(
    IMediator mediator,
    IOutputCacheStore outputCacheStore) : ControllerBase
{
    // -------------------------------------------------------------------------
    // GET api/v1/employees
    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns one page of employees ordered by employee id.
    /// </summary>
    /// <param name="pageNumber">One based page number. Defaults to the first page.</param>
    /// <param name="pageSize">Records per page (1-100). Defaults to 10.</param>
    /// <param name="search">
    /// Optional case insensitive term matched against the name, mobile number and email address.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">The requested page of employees.</response>
    /// <response code="400">The paging or search arguments failed validation.</response>
    /// <remarks>
    /// Served from the <c>employees-collection</c> output-cache policy, keyed by page number, page
    /// size and search term.
    /// </remarks>
    [HttpGet(Name = "Employees_GetEmployees")]
    [EnableRateLimiting(RateLimitingExtensions.ReadPolicy)]
    [OutputCache(PolicyName = CachingExtensions.CollectionPolicy)]
    [ProducesResponseType(typeof(PagedResponse<EmployeeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<PagedResponse<EmployeeResponse>>> GetEmployees(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var employees = await mediator.Send(
            new GetEmployeesQuery(pageNumber, pageSize, search),
            cancellationToken);

        return Ok(employees);
    }

    // -------------------------------------------------------------------------
    // GET api/v1/employees/{id}
    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns a single employee by employee id.
    /// </summary>
    /// <param name="id">The employee id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">The requested employee.</response>
    /// <response code="400">The supplied id is invalid.</response>
    /// <response code="404">No employee with the given id was found.</response>
    /// <remarks>
    /// Served from the <c>employees-item</c> output-cache policy. Failures are not cached: only a
    /// <c>200 OK</c> response is stored.
    /// </remarks>
    [HttpGet("{id:int}", Name = "Employees_GetEmployeeById")]
    [EnableRateLimiting(RateLimitingExtensions.ReadPolicy)]
    [OutputCache(PolicyName = CachingExtensions.ItemPolicy)]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<EmployeeResponse>> GetEmployeeById(
        int id,
        CancellationToken cancellationToken = default)
    {
        var employee = await mediator.Send(new GetEmployeeByIdQuery(id), cancellationToken);

        return employee is null ? NotFound() : Ok(employee);
    }

    // -------------------------------------------------------------------------
    // POST api/v1/employees
    // -------------------------------------------------------------------------

    /// <summary>
    /// Creates a new employee.
    /// </summary>
    /// <param name="request">Employee data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="201">The newly created employee.</response>
    /// <response code="400">The request body failed validation.</response>
    [HttpPost(Name = "Employees_CreateEmployee")]
    [EnableRateLimiting(RateLimitingExtensions.WritePolicy)]
    [ProducesResponseType(typeof(EmployeeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<EmployeeResponse>> CreateEmployee(
        [FromBody] CreateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateEmployeeCommand(
            request.Name,
            request.Mobile,
            request.Email,
            request.IsActive);

        var created = await mediator.Send(command, cancellationToken);

        // A new employee invalidates every cached collection.
        await outputCacheStore.EvictByTagAsync(CachingExtensions.EmployeesTag, cancellationToken);

        return CreatedAtAction(
            nameof(GetEmployeeById),
            new { id = created.Id },
            created);
    }

    // -------------------------------------------------------------------------
    // PUT api/v1/employees/{id}
    // -------------------------------------------------------------------------

    /// <summary>
    /// Replaces the mutable fields of an existing employee.
    /// </summary>
    /// <param name="id">Id of the employee to update.</param>
    /// <param name="request">Updated employee data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">The employee was updated successfully.</response>
    /// <response code="400">The request body or id failed validation.</response>
    /// <response code="404">No employee with the given id was found.</response>
    [HttpPut("{id:int}", Name = "Employees_UpdateEmployee")]
    [EnableRateLimiting(RateLimitingExtensions.WritePolicy)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> UpdateEmployee(
        int id,
        [FromBody] UpdateEmployeeRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateEmployeeCommand(
            id,
            request.Name,
            request.Mobile,
            request.Email,
            request.IsActive);

        var updated = await mediator.Send(command, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        // The updated employee invalidates its item entry and every cached collection.
        await outputCacheStore.EvictByTagAsync(CachingExtensions.EmployeesTag, cancellationToken);

        return NoContent();
    }

    // -------------------------------------------------------------------------
    // DELETE api/v1/employees/{id}
    // -------------------------------------------------------------------------

    /// <summary>
    /// Deletes an employee by employee id.
    /// </summary>
    /// <param name="id">Id of the employee to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">The employee was deleted successfully.</response>
    /// <response code="400">The supplied id is invalid.</response>
    /// <response code="404">No employee with the given id was found.</response>
    [HttpDelete("{id:int}", Name = "Employees_DeleteEmployee")]
    [EnableRateLimiting(RateLimitingExtensions.WritePolicy)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> DeleteEmployee(
        int id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await mediator.Send(new DeleteEmployeeCommand(id), cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        // The removed employee invalidates its item entry and every cached collection.
        await outputCacheStore.EvictByTagAsync(CachingExtensions.EmployeesTag, cancellationToken);

        return NoContent();
    }
}
