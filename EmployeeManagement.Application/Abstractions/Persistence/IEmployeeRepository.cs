using EmployeeManagement.Domain.Models;

namespace EmployeeManagement.Application.Abstractions.Persistence;

/// <summary>
/// Persistence operations for employee records.
/// </summary>
public interface IEmployeeRepository
{
    /// <summary>Returns every employee ordered by employee number.</summary>
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>Returns a single employee by surrogate key, or <see langword="null"/>.</summary>
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Returns a single employee by employee number, or <see langword="null"/>.</summary>
    Task<Employee?> GetByEmployeeNumberAsync(long employeeNumber, CancellationToken cancellationToken);

    /// <summary>Inserts a new employee and returns the generated surrogate key.</summary>
    Task<Guid> AddAsync(Employee employee, CancellationToken cancellationToken);

    /// <summary>Updates the mutable fields of an existing employee. Returns <see langword="false"/> when not found.</summary>
    Task<bool> UpdateAsync(Employee employee, CancellationToken cancellationToken);

    /// <summary>Deletes an employee. Returns <see langword="false"/> when not found.</summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

