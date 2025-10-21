using Bcp.Web.Configuration;
using Bcp.Web.Contracts;
using Bcp.Web.Pages;
using Bcp.Web.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Localization;
using Refit;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var apiBaseAddress = "http://api:5163";

builder.Services.AddRefitClient<ITransactionFileApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseAddress))
    .AddStandardResilienceHandler();

builder.Services.Configure<FileUploadOptions>(
    builder.Configuration.GetSection(FileUploadOptions.Section));

// Configure data protection
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine("root", ".aspnet", "DataProtection-Keys")));

builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddRazorPages();

builder.Services.AddSignalR();

// App-wide culture: en-US
var defaultCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = [defaultCulture],
    SupportedUICultures = [defaultCulture]
};

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Error");
    _ = app.UseHsts();
}

app.UseRequestLocalization(localizationOptions);

app.UseStatusCodePagesWithReExecute("/Error", $"?errorMessage={ErrorModel.HTTP_ERROR_CODE_PREFIX}{{0}}");
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
