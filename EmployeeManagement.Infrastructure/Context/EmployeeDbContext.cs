using EmployeeManagement.Domain.Common;
using EmployeeManagement.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Context;

/// <summary>
/// EF Core database context for EmployeeManagement database.
/// </summary>
public sealed class EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : DbContext(options)
{

    /// <summary>Employees collection mapped to <c>tblEmployee</c>.</summary>
    public DbSet<TblEmployee> TblEmployees => Set<TblEmployee>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblEmployee>(entity =>
        {
            entity.HasKey(e => e.EmpId);
            entity.ToTable("tblEmployee");

            entity.Property(e => e.EmpId).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Mobile).HasMaxLength(15).IsUnicode(false).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(100).IsUnicode(false).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();
        });
    }
}

