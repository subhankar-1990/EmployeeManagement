using EmployeeManagement.Domain.Common;
using EmployeeManagement.Domain.Models;

namespace EmployeeManagement.Application.Abstractions.Persistence;

/// <summary>
/// Persistence operations for employee records.
/// </summary>
public interface IEmployeeRepository
{
    /// <summary>
    /// Returns one page of employees ordered by employee id.
    /// </summary>
    /// <param name="pageNumber">One based page number.</param>
    /// <param name="pageSize">Maximum number of records to return.</param>
    /// <param name="search">
    /// Optional case insensitive term matched against the name, mobile number and email address.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested page together with the total number of matching employees.</returns>
    Task<PagedResult<Employee>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default);

    /// <summary>Returns a single employee by employee id, or <see langword="null"/>.</summary>
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Inserts a new employee and returns the generated employee id.</summary>
    Task<int> AddAsync(Employee employee, CancellationToken cancellationToken = default);

    /// <summary>Updates the mutable fields of an existing employee. Returns <see langword="false"/> when not found.</summary>
    Task<bool> UpdateAsync(Employee employee, CancellationToken cancellationToken = default);

    /// <summary>Deletes an employee. Returns <see langword="false"/> when not found.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
