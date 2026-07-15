using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EmployeeDashboard.Data;
using EmployeeDashboard.ImportEngine.Services;
using System.Text.Json;
using EmployeeDashboard.ImportEngine.Enums;

namespace EmployeeDashboard.Pages;

public class UploadModel : PageModel
{
    private readonly CsvImportService _csvImportService;
    private readonly ImportAnalyzer _importAnalyzer;

    public UploadModel(
        CsvImportService csvImportService,
        ImportAnalyzer importAnalyzer)
    {
        _csvImportService = csvImportService;
        _importAnalyzer = importAnalyzer;
    }

    [BindProperty]
    public IFormFile? CsvFile { get; set; }

    public string Message { get; set; } = "";

    public void OnGet()
    {
        if (TempData.ContainsKey("SuccessMessage"))
        {
            Message = TempData["SuccessMessage"]!.ToString()!;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (CsvFile == null)
        {
            Message = "Please select a CSV file.";
            return Page();
        }

        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        Directory.CreateDirectory(uploadsFolder);

        string filePath = Path.Combine(uploadsFolder, CsvFile.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await CsvFile.CopyToAsync(stream);
        }

        // Analyze the file first
        var analysis = _importAnalyzer.Analyze(filePath);

        analysis.FilePath = filePath;
        analysis.FileName = CsvFile.FileName;

        if (analysis.DuplicateRows > 0)
        {
            HttpContext.Session.SetString(
                "ImportAnalysis",
                JsonSerializer.Serialize(analysis));

            return RedirectToPage("/ImportAnalysis");
        }
        else
        {
            // No duplicates, import directly
            var result = _csvImportService.Import(filePath, ImportMode.ImportAnyway);

            Message =
            $@"Import Successful

        Total Rows : {result.TotalRows}

        Imported : {result.ImportedRows}    

        Duplicates : {result.DuplicateRows}";
            
            return Page();
        }
    }
}