namespace EmployeeDashboard.Models;

public class Employee
{
    public string CompanyCode { get; set; } = "";
    public string EmployeeName { get; set; } = "";
    public string Email { get; set; } = "";
    public string ProjectName { get; set; } = "";
    public string TaskName { get; set; } = "";
    public string Status { get; set; } = "";

    public DateTime WorkDate { get; set; }

    public decimal Hours { get; set; }
}