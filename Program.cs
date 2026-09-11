using UiEmbed.Components;
using UiEmbed.Services;
// using Radzen;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5001");

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
// builder.Services.AddRadzenComponents();
builder.Services.AddSingleton<CabinetStateService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();