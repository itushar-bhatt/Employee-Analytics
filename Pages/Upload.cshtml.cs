using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EmployeeDashboard.Services;

namespace EmployeeDashboard.Pages;

public class UploadModel : PageModel
{
    private readonly CsvImportService _csvImportService;

    public UploadModel(CsvImportService csvImportService)
    {
        _csvImportService = csvImportService;
    }

    [BindProperty]
    public IFormFile CsvFile { get; set; }

    public string Message { get; set; } = "";

    public void OnGet()
    {

    }

    public async Task OnPostAsync()
    {
        if (CsvFile == null)
        {
            Message = "Please select a CSV file.";
            return;
        }

        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        Directory.CreateDirectory(uploadsFolder);

        string filePath = Path.Combine(uploadsFolder, CsvFile.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await CsvFile.CopyToAsync(stream);
        }

        var result = _csvImportService.Import(filePath);

        Message =
        $@"Total Rows : {result.TotalRows}

        Imported : {result.ImportedRows}

        Duplicates : {result.DuplicateRows}"; 
   }
}