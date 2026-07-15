using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using EmployeeDashboard.ImportEngine.Models;
using EmployeeDashboard.Data;
using EmployeeDashboard.ImportEngine.Enums;

namespace EmployeeDashboard.Pages;

public class ImportAnalysisModel : PageModel
{
    private readonly CsvImportService _csvImportService;

    public ImportAnalysisModel(
        CsvImportService csvImportService)
    {
        _csvImportService = csvImportService;
    }

    public ImportAnalysis? Analysis { get; private set; }

    public IActionResult OnGet()
    {
        Analysis = GetAnalysis();

        if (Analysis == null)
        {
            return RedirectToPage("/Upload");
        }

        return Page();
    }

    // Reads the analysis object stored in Session
    private ImportAnalysis? GetAnalysis()
    {
        string? json = HttpContext.Session.GetString("ImportAnalysis");

        if (string.IsNullOrEmpty(json))
            return null;

        return JsonSerializer.Deserialize<ImportAnalysis>(json);
    }

    // Import all rows including duplicates
    public IActionResult OnPostImportAnyway()
    {
        var analysis = GetAnalysis();

        if (analysis == null)
            return RedirectToPage("/Upload");

        var result = _csvImportService.Import(
            analysis.FilePath,
            ImportMode.ImportAnyway);

        HttpContext.Session.Remove("ImportAnalysis");

        TempData["SuccessMessage"] = $@"Import Completed Successfully!

        Total Rows : {result.TotalRows}

        Imported : {result.ImportedRows}

        Duplicates : {result.DuplicateRows}";

        return RedirectToPage("/Upload");
    }

    // Import only new rows
    public IActionResult OnPostImportNewRows()
    {
        var analysis = GetAnalysis();

        if (analysis == null)
            return RedirectToPage("/Upload");

        var result = _csvImportService.Import(
            analysis.FilePath,
            ImportMode.NewRowsOnly);

        HttpContext.Session.Remove("ImportAnalysis");

        TempData["SuccessMessage"] = $@"Import Completed Successfully!

        Total Rows : {result.TotalRows}

        Imported : {result.ImportedRows}

        Duplicates : {result.DuplicateRows}";

        return RedirectToPage("/Upload");
    }

    // Cancel import
    public IActionResult OnPostCancel()
    {
        var analysis = GetAnalysis();

        if (analysis != null && System.IO.File.Exists(analysis.FilePath))
        {
            System.IO.File.Delete(analysis.FilePath);
        }

        HttpContext.Session.Remove("ImportAnalysis");

        return RedirectToPage("/Upload");
    }
}