
using System.Diagnostics;
using EmployeeDashboard.Data;
using EmployeeDashboard.ImportEngine.Interfaces;
using EmployeeDashboard.ImportEngine.Detectors;
using EmployeeDashboard.ImportEngine.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSession();
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddSingleton<EmployeeRepository>();
builder.Services.AddSingleton<CsvImportService>();
builder.Services.AddSingleton<UploadHistoryRepository>();
builder.Services.AddSingleton<IDuplicateDetector, CompositeDuplicateDetector>();
builder.Services.AddSingleton<ImportAnalyzer>();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<DatabaseService>();
    database.CreateDatabase();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
