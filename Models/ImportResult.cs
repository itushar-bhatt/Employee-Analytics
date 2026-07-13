namespace EmployeeDashboard.Models;

public class ImportResult
{
    public int TotalRows { get; set; }

    public int ImportedRows { get; set; }

    public int DuplicateRows { get; set; }
}