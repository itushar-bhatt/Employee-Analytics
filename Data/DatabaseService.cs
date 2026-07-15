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

        RemoveUniqueConstraintIfExists(connection);
        CreateEmployeesTable(connection);
        CreateUploadHistoryTable(connection);
    }
    
    // Remove UNIQUE constraint if it exists from old database schema
    private void RemoveUniqueConstraintIfExists(SqliteConnection connection)
    {
        try
        {
            // Check if the old table with UNIQUE constraint exists
            using var checkCommand = new SqliteCommand(
                "SELECT sql FROM sqlite_master WHERE type='table' AND name='Employees';", 
                connection);
            
            var existingSchema = checkCommand.ExecuteScalar()?.ToString();
            
            if (!string.IsNullOrEmpty(existingSchema) && existingSchema.Contains("UNIQUE"))
            {
                // Create new table without UNIQUE constraint
                using var createNewTable = new SqliteCommand(@"
                    CREATE TABLE Employees_new
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
                    );", connection);
                createNewTable.ExecuteNonQuery();
                
                // Copy data from old table
                using var copyData = new SqliteCommand(
                    "INSERT INTO Employees_new SELECT * FROM Employees;", 
                    connection);
                copyData.ExecuteNonQuery();
                
                // Drop old table
                using var dropOldTable = new SqliteCommand(
                    "DROP TABLE Employees;", connection);
                dropOldTable.ExecuteNonQuery();
                
                // Rename new table
                using var renameTable = new SqliteCommand(
                    "ALTER TABLE Employees_new RENAME TO Employees;", 
                    connection);
                renameTable.ExecuteNonQuery();
            }
        }
        catch (SqliteException)
        {
            // If anything fails, continue with normal table creation
        }
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