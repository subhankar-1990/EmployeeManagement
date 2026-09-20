using EmployeeManagement.Application.Abstractions.Persistence;
using EmployeeManagement.Domain.Common;
using EmployeeManagement.Domain.Models;

namespace EmployeeManagement.Tests.Fakes;

/// <summary>
/// Test double for <see cref="IEmployeeRepository"/> that pages an in-memory list.
/// </summary>
internal sealed class FakeEmployeeRepository(IReadOnlyList<Employee> employees) : IEmployeeRepository
{
    /// <summary>Creates a repository over the supplied employees.</summary>
    public static FakeEmployeeRepository Containing(params Employee[] employees) => new(employees);

    /// <inheritdoc />
    public Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(employees);

    /// <inheritdoc />
    public Task<PagedResult<Employee>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var matches = Filter(search);

        var items = matches
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Task.FromResult(new PagedResult<Employee>(items, pageNumber, pageSize, matches.Count));
    }

    /// <inheritdoc />
    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(employees.FirstOrDefault(employee => employee.EmpId == id));

    /// <inheritdoc />
    public Task<int> AddAsync(Employee employee, CancellationToken cancellationToken = default) =>
        Task.FromResult(employee.EmpId);

    /// <inheritdoc />
    public Task<bool> UpdateAsync(Employee employee, CancellationToken cancellationToken = default) =>
        Task.FromResult(employees.Any(existing => existing.EmpId == employee.EmpId));

    /// <inheritdoc />
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) =>
        Task.FromResult(employees.Any(employee => employee.EmpId == id));

    private IReadOnlyList<Employee> Filter(string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return employees;
        }

        return employees
            .Where(employee =>
                employee.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                || employee.Mobile.Contains(search, StringComparison.OrdinalIgnoreCase)
                || employee.Email.Contains(search, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
