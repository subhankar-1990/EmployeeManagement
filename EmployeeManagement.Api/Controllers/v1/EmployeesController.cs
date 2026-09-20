using Asp.Versioning;
using EmployeeManagement.Application.Contracts.Common;
using EmployeeManagement.Application.Contracts.Employees;
using EmployeeManagement.Application.Services.Employees.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers.V1;

/// <summary>
/// Manages employee records.
/// </summary>
/// <remarks>
/// The controller currently exposes read access only. Authentication is not wired into this API
/// yet, so no <c>[Authorize]</c> attribute is applied; add the JWT bearer middleware first and the
/// endpoint can then be locked down the same way the rest of the surface will be.
/// </remarks>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/employees")]
[Produces("application/json")]
[Tags("Employees")]
public sealed class EmployeesController(IMediator mediator) : ControllerBase
{
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
    [HttpGet(Name = "Employees_GetEmployees")]
    [ProducesResponseType(typeof(PagedResponse<EmployeeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
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
}
