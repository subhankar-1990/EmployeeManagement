using EmployeeManagement.Domain.Models;

namespace EmployeeManagement.Application.Abstractions.Persistence;

/// <summary>
/// Persistence operations for employee records.
/// </summary>
public interface IEmployeeRepository
{
    /// <summary>Returns every employee ordered by employee id.</summary>
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Returns a single employee by employee id, or <see langword="null"/>.</summary>
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Inserts a new employee and returns the generated employee id.</summary>
    Task<int> AddAsync(Employee employee, CancellationToken cancellationToken = default);

    /// <summary>Updates the mutable fields of an existing employee. Returns <see langword="false"/> when not found.</summary>
    Task<bool> UpdateAsync(Employee employee, CancellationToken cancellationToken = default);

    /// <summary>Deletes an employee. Returns <see langword="false"/> when not found.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
