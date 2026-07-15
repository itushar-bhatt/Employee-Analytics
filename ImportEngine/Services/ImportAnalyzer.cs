using Microsoft.VisualBasic.FileIO;
using EmployeeDashboard.Models;
using EmployeeDashboard.ImportEngine.Interfaces;
using EmployeeDashboard.ImportEngine.Models;

namespace EmployeeDashboard.ImportEngine.Services;

public class ImportAnalyzer
{
    private readonly IDuplicateDetector _duplicateDetector;

    public ImportAnalyzer(IDuplicateDetector duplicateDetector)
    {
        _duplicateDetector = duplicateDetector;
    }

    public ImportAnalysis Analyze(string filePath)
    {
        ImportAnalysis result = new();

        result.FilePath = filePath;

        using var parser = new TextFieldParser(filePath);

        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;

        // Skip CSV Header
        parser.ReadFields();

        while (!parser.EndOfData)
        {
            string[] data = parser.ReadFields()!;

            result.TotalRows++;

            Employee employee = CreateEmployee(data);

            var duplicateResult =
                _duplicateDetector.Analyze(employee);

            if (duplicateResult.IsDuplicate)
            {
                result.DuplicateRows++;

                result.DuplicateEmployees.Add(employee);
            }
            else
            {
                result.NewRows++;
            }
        }

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
}