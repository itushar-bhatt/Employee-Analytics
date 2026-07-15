using EmployeeDashboard.Models;
using EmployeeDashboard.ImportEngine.Models;

namespace EmployeeDashboard.ImportEngine.Interfaces;

public interface IDuplicateDetector
{
    DuplicateAnalysis Analyze(Employee employee);
}