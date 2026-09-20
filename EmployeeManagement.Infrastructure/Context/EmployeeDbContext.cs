using EmployeeManagement.Domain.Common;
using EmployeeManagement.Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Context;

/// <summary>
/// EF Core context for the EmployeeManagement database.
/// </summary>
public sealed class EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : DbContext(options)
{
    /// <summary>Authentication records.</summary>
    public DbSet<AuthMaster> AuthMasters => Set<AuthMaster>();

    /// <summary>Employee records.</summary>
    public DbSet<EmployeeMaster> EmployeeMasters => Set<EmployeeMaster>();

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

        modelBuilder.Entity<EmployeeMaster>(entity =>
        {
            entity.HasKey(employee => employee.EmpId);

            entity.ToTable("EmployeeMaster");

            entity.Property(employee => employee.EmpId).HasDefaultValueSql("(newid())", "DF_EmployeeMaster_EmpId");
            entity.Property(employee => employee.EmpNo).ValueGeneratedOnAdd();
            entity.Property(employee => employee.EmpName).HasMaxLength(100).IsRequired();
            entity.Property(employee => employee.Mobile).HasMaxLength(20);
            entity.Property(employee => employee.Email).HasMaxLength(254);
            entity.Property(employee => employee.CreateDate).HasColumnType("datetime");
            entity.Property(employee => employee.IsActive).HasDefaultValue(true, "DF_EmployeeMaster_IsActive");

            entity.HasIndex(employee => employee.EmpNo).IsUnique().HasDatabaseName("UX_EmployeeMaster_EmpNo");
        });
    }
}

