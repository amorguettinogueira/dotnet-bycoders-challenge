using Refit;
using bcp.UI.Pages;
using bcp.UI.Services;

var builder = WebApplication.CreateBuilder(args);

string apiBaseAddress = "http://api:5163";

builder.Services.AddRefitClient<ITransactionTypeApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseAddress));

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error", $"?errorMessage={ErrorModel.HTTP_ERROR_CODE_PREFIX}{{0}}");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
