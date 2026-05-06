using EnterpriseAIOrchestrator.Web.Models;
using EnterpriseAIOrchestrator.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.Configure<ApiOptions>(builder.Configuration);
builder.Services.AddHttpClient<IEnterpriseAiOrchestratorApiClient, EnterpriseAiOrchestratorApiClient>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ApiOptions>>().Value;
    client.BaseAddress = options.GetBaseUri();
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
