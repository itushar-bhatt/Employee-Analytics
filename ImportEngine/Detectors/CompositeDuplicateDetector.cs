using EmployeeDashboard.Data;
using EmployeeDashboard.ImportEngine.Interfaces;
using EmployeeDashboard.ImportEngine.Models;
using EmployeeDashboard.Models;

namespace EmployeeDashboard.ImportEngine.Detectors;

public class CompositeDuplicateDetector : IDuplicateDetector
{
    private readonly EmployeeRepository _repository;

    public CompositeDuplicateDetector(EmployeeRepository repository)
    {
        _repository = repository;
    }

    public DuplicateAnalysis Analyze(Employee employee)
    {
        bool exists = _repository.Exists(employee);

        return new DuplicateAnalysis
        {
            IsDuplicate = exists,
            Reason = exists
                ? "Matched using composite key."
                : ""
        };
    }
}