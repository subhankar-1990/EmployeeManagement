using EmployeeManagement.Domain.Common;
using EmployeeManagement.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Context;

/// <summary>
/// EF Core database context for EmployeeManagement database.
/// </summary>
public sealed class EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : DbContext(options)
{
    /// <summary>Authentication records mapped to <c>AuthMaster</c>.</summary>
    public DbSet<AuthMaster> AuthMasters => Set<AuthMaster>();

    /// <summary>Employees collection mapped to <c>tblEmployee</c>.</summary>
    public DbSet<TblEmployee> TblEmployees => Set<TblEmployee>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuthMaster>(entity =>
        {
            entity.HasKey(auth => auth.AuthId);

            entity.ToTable("AuthMaster");

            entity.Property(auth => auth.AuthId).HasDefaultValueSql("(newid())", "DF_AuthMaster_AuthId");
            entity.Property(auth => auth.PasswordHash).HasMaxLength(200).IsRequired();
            entity.Property(auth => auth.Role)
                .HasMaxLength(50)
                .IsRequired()
                .HasDefaultValue(Roles.Employee, "DF_AuthMaster_Role");
            entity.Property(auth => auth.IsLocked).HasDefaultValue(false, "DF_AuthMaster_IsLocked");

            entity.HasIndex(auth => auth.EmpNo).IsUnique().HasDatabaseName("UX_AuthMaster_EmpNo");
        });

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

