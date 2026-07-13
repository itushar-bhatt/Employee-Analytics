namespace EmployeeDashboard.Models;

public class UploadHistory
{
    public int Id { get; set; }

    public string FileName { get; set; } = "";

    public DateTime UploadDate { get; set; }

    public int TotalRows { get; set; }

    public int ImportedRows { get; set; }

    public int DuplicateRows { get; set; }
}