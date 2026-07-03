using Microsoft.Data.Sqlite;
using EmployeeDashboard.Models;

namespace EmployeeDashboard.Data;

public class EmployeeRepository
{
    private readonly DatabaseService _databaseService;

    public EmployeeRepository(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public int Insert(
        Employee employee,
        SqliteConnection connection,
        SqliteTransaction transaction)
    {
        string query = @"
            INSERT OR IGNORE INTO Employees
            (
                CompanyCode,
                EmployeeName,
                Email,
                ProjectName,
                TaskName,
                Status,
                WorkDate,
                Hours
            )
            VALUES
            (
                @CompanyCode,
                @EmployeeName,
                @Email,
                @ProjectName,
                @TaskName,
                @Status,
                @WorkDate,
                @Hours
            );";

        using var command = new SqliteCommand(query, connection, transaction);

        command.Parameters.AddWithValue("@CompanyCode", employee.CompanyCode);
        command.Parameters.AddWithValue("@EmployeeName", employee.EmployeeName);
        command.Parameters.AddWithValue("@Email", employee.Email);
        command.Parameters.AddWithValue("@ProjectName", employee.ProjectName);
        command.Parameters.AddWithValue("@TaskName", employee.TaskName);
        command.Parameters.AddWithValue("@Status", employee.Status);
        command.Parameters.AddWithValue(
            "@WorkDate",
            employee.WorkDate.ToString("yyyy-MM-dd")
        );
        command.Parameters.AddWithValue("@Hours", employee.Hours);

        return command.ExecuteNonQuery();
    }
    public List<Employee> GetAllEmployees()
    {
        List<Employee> employees = new();

        using var connection = _databaseService.GetConnection();
        connection.Open();

        string query = "SELECT * FROM Employees";

        using var command = new SqliteCommand(query, connection);

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            employees.Add(MapEmployee(reader));
        }

        return employees;
    }

    public int GetEmployeeCount()
    {
        using var connection = _databaseService.GetConnection();
        connection.Open();

        string query = "SELECT COUNT(*) FROM Employees";

        using var command = new SqliteCommand(query, connection);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public List<Employee> SearchEmployees(string searchText)
    {
        List<Employee> employees = new();

        using var connection = _databaseService.GetConnection();

        connection.Open();

        string query = @"
            SELECT *
            FROM Employees
            WHERE EmployeeName LIKE @Search
            OR Email LIKE @Search
            OR ProjectName LIKE @Search
            ORDER BY EmployeeName;";

        using var command = new SqliteCommand(query, connection);

        command.Parameters.AddWithValue("@Search", $"%{searchText}%");

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            employees.Add(MapEmployee(reader));
        }

        return employees;
    }

    private Employee MapEmployee(SqliteDataReader reader)
{
    return new Employee
    {
        CompanyCode = reader["CompanyCode"].ToString()!,
        EmployeeName = reader["EmployeeName"].ToString()!,
        Email = reader["Email"].ToString()!,
        ProjectName = reader["ProjectName"].ToString()!,
        TaskName = reader["TaskName"].ToString()!,
        Status = reader["Status"].ToString()!,
        WorkDate = DateTime.Parse(reader["WorkDate"].ToString()!),
        Hours = Convert.ToDecimal(reader["Hours"])
    };
}
}