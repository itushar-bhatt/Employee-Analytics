using Microsoft.Data.Sqlite;


namespace EmployeeDashboard.Data;

public class DatabaseService
{
    // Connection string for the SQLite database.
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
    
    // Creates the Employees table if it doesn't exist.
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
            // Execute the query to create the Employees table.

            using var createCommand = new SqliteCommand(query, connection);
            createCommand.ExecuteNonQuery();

        try
        {
            using var command = new SqliteCommand(
                "ALTER TABLE Employees ADD COLUMN UploadHistoryId INTEGER;",
                connection);

            command.ExecuteNonQuery();
        }
        catch(SqliteException)
        {
            // Column already exists.
        }


        // Create a unique index to prevent duplicate entries.
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

    // Creates the UploadHistory table if it doesn't exist.
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

                DuplicateRows INTEGER
            );";

        using var command = new SqliteCommand(query, connection);
        command.ExecuteNonQuery();
    }
}