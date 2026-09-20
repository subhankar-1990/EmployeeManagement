using EmployeeManagement.Application.Abstractions.Persistence;
using EmployeeManagement.Domain.Models;
using EmployeeManagement.Infrastructure.Context;
using EmployeeManagement.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories.Employees;

/// <summary>
/// EF Core implementation of <see cref="IEmployeeRepository"/>.
/// </summary>
internal sealed class EmployeeRepository(EmployeeDbContext context) : IEmployeeRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken)
    {
        var employees = await context.EmployeeMasters
            .AsNoTracking()
            .OrderBy(employee => employee.EmpNo)
            .ToListAsync(cancellationToken);

        return employees.Select(ToModel).ToList();
    }

    /// <inheritdoc />
    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var employee = await context.EmployeeMasters
            .AsNoTracking()
            .FirstOrDefaultAsync(record => record.EmpId == id, cancellationToken);

        return employee is null ? null : ToModel(employee);
    }

    /// <inheritdoc />
    public async Task<Employee?> GetByEmployeeNumberAsync(long employeeNumber, CancellationToken cancellationToken)
    {
        var employee = await context.EmployeeMasters
            .AsNoTracking()
            .FirstOrDefaultAsync(record => record.EmpNo == employeeNumber, cancellationToken);

        return employee is null ? null : ToModel(employee);
    }

    /// <inheritdoc />
    public async Task<Guid> AddAsync(Employee employee, CancellationToken cancellationToken)
    {
        var entity = new EmployeeMaster
        {
            EmpName = employee.Name,
            Mobile = employee.MobileNumber,
            Email = employee.Email,
            CreateDate = employee.CreatedAt,
            IsActive = employee.IsActive
        };

        context.EmployeeMasters.Add(entity);

        await context.SaveChangesAsync(cancellationToken);

        return entity.EmpId;
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Employee employee, CancellationToken cancellationToken)
    {
        var entity = await context.EmployeeMasters
            .FirstOrDefaultAsync(record => record.EmpId == employee.Id, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        entity.EmpName = employee.Name;
        entity.Mobile = employee.MobileNumber;
        entity.Email = employee.Email;
        entity.IsActive = employee.IsActive;

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await context.EmployeeMasters
            .FirstOrDefaultAsync(record => record.EmpId == id, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        context.EmployeeMasters.Remove(entity);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static Employee ToModel(EmployeeMaster entity) => new()
    {
        Id = entity.EmpId,
        EmployeeNumber = entity.EmpNo,
        Name = entity.EmpName,
        MobileNumber = entity.Mobile,
        Email = entity.Email,
        CreatedAt = entity.CreateDate,
        IsActive = entity.IsActive
    };
}

