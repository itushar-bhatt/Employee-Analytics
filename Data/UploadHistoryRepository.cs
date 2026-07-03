using Microsoft.Data.Sqlite;
using EmployeeDashboard.Models;

namespace EmployeeDashboard.Data;

public class UploadHistoryRepository
{
    private readonly DatabaseService _databaseService;

    public UploadHistoryRepository(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public void Insert(UploadHistory history)
    {
        using var connection = _databaseService.GetConnection();
        connection.Open();

        string query = @"
            INSERT INTO UploadHistory
            (
                FileName,
                UploadDate,
                TotalRows,
                ImportedRows,
                DuplicateRows,
                DurationInSeconds
            )
            VALUES
            (
                @FileName,
                @UploadDate,
                @TotalRows,
                @ImportedRows,
                @DuplicateRows,
                @DurationInSeconds
            );";

        using var command = new SqliteCommand(query, connection);

        command.Parameters.AddWithValue("@FileName", history.FileName);
        command.Parameters.AddWithValue("@UploadDate", history.UploadDate.ToString("yyyy-MM-dd HH:mm:ss"));
        command.Parameters.AddWithValue("@TotalRows", history.TotalRows);
        command.Parameters.AddWithValue("@ImportedRows", history.ImportedRows);
        command.Parameters.AddWithValue("@DuplicateRows", history.DuplicateRows);
        command.Parameters.AddWithValue("@DurationInSeconds", history.DurationInSeconds);

        command.ExecuteNonQuery();
    }

    public List<UploadHistory> GetAll()
    {
        List<UploadHistory> history = new();

        using var connection = _databaseService.GetConnection();
        connection.Open();

        string query = @"
            SELECT *
            FROM UploadHistory
            ORDER BY UploadDate DESC;";

        using var command = new SqliteCommand(query, connection);

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            history.Add(new UploadHistory
            {
                FileName = reader["FileName"].ToString()!,
                UploadDate = DateTime.Parse(reader["UploadDate"].ToString()!),
                TotalRows = Convert.ToInt32(reader["TotalRows"]),
                ImportedRows = Convert.ToInt32(reader["ImportedRows"]),
                DuplicateRows = Convert.ToInt32(reader["DuplicateRows"]),
                DurationInSeconds = Convert.ToDouble(reader["DurationInSeconds"])
            });
        }

        return history;
    }
}