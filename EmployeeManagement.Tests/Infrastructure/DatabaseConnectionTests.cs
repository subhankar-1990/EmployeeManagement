using System.Data;
using Microsoft.Data.SqlClient;

namespace EmployeeManagement.Tests.Infrastructure;

public sealed class DatabaseConnectionTests
{
    private const string ConnectionString = "workstation id=EmployeeManagement.mssql.somee.com;packet size=4096;user id=s_sarkar_SQLLogin_1;pwd=y2qne6swj4;data source=EmployeeManagement.mssql.somee.com;persist security info=False;initial catalog=EmployeeManagement;TrustServerCertificate=True";

    [Fact]
    public async Task CanSampleTopRows()
    {
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT TOP 3 EmpId, Name, Mobile, Email, IsActive FROM dbo.tblEmployee";

        await using var reader = await command.ExecuteReaderAsync();
        var rows = new List<string>();
        while (await reader.ReadAsync())
        {
            var empId = reader.GetInt32(0);
            var name = reader.GetString(1);
            var mobile = reader.GetString(2);
            var email = reader.GetString(3);
            var isActive = reader.GetBoolean(4);
            rows.Add($"EmpId={empId}, Name={name}, Mobile={mobile}, Email={email}, IsActive={isActive}");
        }

        Console.WriteLine($"[Sample tblEmployee Rows]:\n{string.Join("\n", rows)}");
    }
}
