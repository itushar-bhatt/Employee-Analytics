using Microsoft.Data.Sqlite;


namespace EmployeeDashboard.Services;

public class DatabaseService
{
    private readonly string _connectionString = "Data Source=Database/EmployeeDashboard.db";

    public SqliteConnection GetConnection()
    {
        return new SqliteConnection(_connectionString);
    }

    public void CreateDatabase()
    {
        Directory.CreateDirectory("Database");

        using var connection = GetConnection();
        connection.Open();

        CreateEmployeesTable(connection);
        CreateUploadHistoryTable(connection);
    }

    private void CreateEmployeesTable(SqliteConnection connection)
    {
        string query = @"
            CREATE TABLE IF NOT EXISTS Employees
            (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,

                CompanyCode TEXT,
                EmployeeName TEXT,
                Email TEXT,
                ProjectName TEXT,
                TaskName TEXT,
                Status TEXT,
                WorkDate TEXT,
                Hours REAL
            );";

        using var command = new SqliteCommand(query, connection);
        command.ExecuteNonQuery();

        string uniqueIndex = @"
            CREATE UNIQUE INDEX IF NOT EXISTS IDX_Employee_Unique
            ON Employees
            (
                CompanyCode,
                EmployeeName,
                Email,
                ProjectName,
                TaskName,
                Status,
                WorkDate,
                Hours
            );";

            using var indexCommand = new SqliteCommand(uniqueIndex, connection);
            indexCommand.ExecuteNonQuery();
    }

    private void CreateUploadHistoryTable(SqliteConnection connection)
    {
        string query = @"
            CREATE TABLE IF NOT EXISTS UploadHistory
            (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,

                FileName TEXT,

                UploadDate TEXT,

                TotalRows INTEGER,

                ImportedRows INTEGER,

                DuplicateRows INTEGER,

                DurationInSeconds REAL
            );";

        using var command = new SqliteCommand(query, connection);
        command.ExecuteNonQuery();
    }
}