using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using UiEmbed.Components;
using UiEmbed.Services;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5001");

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/user";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AppAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<AppAuthStateProvider>());
builder.Services.AddSingleton<CabinetStateService>();
builder.Services.AddSingleton<HeaderStateService>();
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/login", async (LoginRequest req, IConfiguration config, HttpContext context) =>
{
    if (!AppAuthStateProvider.VerifyPin(config, req.Role, req.Pin)) return Results.BadRequest();
    var principal = AppAuthStateProvider.CreatePrincipal(req.Name, req.Role);
    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
        principal, new AuthenticationProperties { IsPersistent = true });
    return Results.Ok();
});

app.MapPost("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Ok();
});

app.Run();

// ponytail: /login+/logout skip antiforgery; PIN is the credential and CSRF buys nothing on a
// localhost HMI. Add RequireAntiforgery + token header if this ever exposes a network surface.
public sealed record LoginRequest(string Name, string Role, string Pin);