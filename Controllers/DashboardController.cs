using Microsoft.AspNetCore.Mvc;

namespace EmployeeDashboard.Controllers;

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}