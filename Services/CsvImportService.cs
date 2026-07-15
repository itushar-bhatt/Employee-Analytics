using Microsoft.VisualBasic.FileIO;
using EmployeeDashboard.Models;
using EmployeeDashboard.ImportEngine.Enums;

namespace EmployeeDashboard.Data;

public class CsvImportService
{
    private readonly DatabaseService _databaseService;
    private readonly EmployeeRepository _employeeRepository;
    private readonly UploadHistoryRepository _uploadHistoryRepository;

    public CsvImportService(
        DatabaseService databaseService,
        EmployeeRepository employeeRepository,
        UploadHistoryRepository uploadHistoryRepository)
    {
        _databaseService = databaseService;
        _employeeRepository = employeeRepository;
        _uploadHistoryRepository = uploadHistoryRepository;
    }

    public ImportResult Import(string filePath, ImportMode mode)
    {
        ImportResult result = new();

        using var connection = _databaseService.GetConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        using var parser = new TextFieldParser(filePath);

        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;

        // Skip Header
        parser.ReadFields();

        while (!parser.EndOfData)
        {
            string[] data = parser.ReadFields()!;

            result.TotalRows++;

            Employee employee = CreateEmployee(data);

            switch (mode)
            {
                case ImportMode.ImportAnyway:

                    // INSERT OR IGNORE returns 1 if inserted, 0 if ignored (duplicate)
                    int rowsAffected = _employeeRepository.Insert(
                        employee,
                        connection,
                        transaction);

                    if (rowsAffected > 0)
                    {
                        result.ImportedRows++;
                    }
                    else
                    {
                        result.DuplicateRows++;
                    }

                    break;

                case ImportMode.NewRowsOnly:

                    if (!_employeeRepository.Exists(employee))
                    {
                        _employeeRepository.Insert(
                            employee,
                            connection,
                            transaction);

                        result.ImportedRows++;
                    }
                    else
                    {
                        result.DuplicateRows++;
                    }

                    break;
            }
        }

        transaction.Commit();

        SaveUploadHistory(filePath, result);

        return result;
    }

    private Employee CreateEmployee(string[] data)
    {
        return new Employee
        {
            CompanyCode = data[0].Trim(),
            EmployeeName = data[1].Trim(),
            Email = data[2].Trim(),
            ProjectName = data[3].Trim(),
            TaskName = data[4].Trim(),
            Status = data[5].Trim(),
            WorkDate = DateTime.Parse(data[6]),
            Hours = decimal.Parse(data[7])
        };
    }

    private void SaveUploadHistory(
        string filePath,
        ImportResult result)
    {
        UploadHistory history = new()
        {
            FileName = Path.GetFileName(filePath),
            UploadDate = DateTime.Now,
            TotalRows = result.TotalRows,
            ImportedRows = result.ImportedRows,
            DuplicateRows = result.DuplicateRows
        };

        _uploadHistoryRepository.Insert(history);
    }
}