using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using EmployeeDashboard.Data;
using EmployeeDashboard.Models;

namespace EmployeeDashboard.Pages;

public class EmployeeExplorerModel : PageModel
{
    private readonly EmployeeRepository _repository;

    public List<Employee> Employees { get; set; } = new();

    public EmployeeExplorerModel(EmployeeRepository repository)
    {
        _repository = repository;
    }

    [BindProperty(SupportsGet = true)]
    public string Search { get; set; } = "";

    public void OnGet()
    {
        if (string.IsNullOrWhiteSpace(Search))
        {
            Employees = _repository.GetAllEmployees();
        }
        else
        {
            Employees = _repository.SearchEmployees(Search);
        }
    }
}