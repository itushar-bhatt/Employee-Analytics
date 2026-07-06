using Microsoft.AspNetCore.Mvc.RazorPages;
using EmployeeDashboard.Data;
using EmployeeDashboard.Models;

namespace EmployeeDashboard.Pages;

public class UploadHistoryModel : PageModel
{
    private readonly UploadHistoryRepository _repository;

    public List<UploadHistory> Uploads { get; set; } = new();

    public UploadHistoryModel(UploadHistoryRepository repository)
    {
        _repository = repository;
    }

    public void OnGet()
    {
        Uploads = _repository.GetAll();
    }
}