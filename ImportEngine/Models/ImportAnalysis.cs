using EmployeeDashboard.Models;

namespace EmployeeDashboard.ImportEngine.Models;

public class ImportAnalysis
{
    public int TotalRows { get; set; }

    public int NewRows { get; set; }

    public int DuplicateRows { get; set; }

    public List<Employee> DuplicateEmployees { get; set; } = new();
    public string FilePath { get; set; } = "";

    public string FileName { get; set; } = "";
}