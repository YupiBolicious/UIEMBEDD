namespace UiEmbed.Services;

using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

public sealed class AppAuthStateProvider : AuthenticationStateProvider
{
    // ponytail: PIN hashes live in appsettings; cookie auth over software-only RBAC on a
    // local HMI is bypassable by anyone with device access. Tamper-proofing needs a hardware
    // keyswitch/PLC interlock - out of scope.
    private const string AuthType = "uiembed";

    private readonly IJSRuntime _js;
    private readonly IHttpContextAccessor _http;
    private ClaimsPrincipal _principal = AnonymousPrincipal();

    public AppAuthStateProvider(IJSRuntime js, IHttpContextAccessor http)
    {
        _js = js;
        _http = http;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_principal.Identity?.IsAuthenticated == true)
            return Task.FromResult(new AuthenticationState(_principal));
        var user = _http.HttpContext?.User;
        return Task.FromResult(user is { Identity.IsAuthenticated: true }
            ? new AuthenticationState(user)
            : new AuthenticationState(_principal));
    }

    public string? UserName => _principal.Identity?.Name;
    public bool IsLoggedIn => _principal.Identity?.IsAuthenticated == true;

    public string? CurrentRole => _principal.FindFirstValue(RoleClaimType);

    public IEnumerable<string> Roles =>
        _principal.FindAll(ClaimTypes.Role).Select(c => c.Value);

    public bool CanAccess(string role) => Roles.Contains(role);

    public async Task<bool> LoginAsync(string name, string role, string pin)
    {
        if (!await CallLoginAsync(name, role, pin)) return false;
        SetPrincipal(name, role);
        return true;
    }

    public async Task<bool> SwitchRoleAsync(string role, string pin)
    {
        if (CanAccess(role)) return true;
        var name = _principal.Identity?.Name ?? "Operator";
        if (!await CallLoginAsync(name, role, pin)) return false;
        SetPrincipal(name, role);
        return true;
    }

    public async Task LogoutAsync()
    {
        await CallLogoutAsync();
        SetAnonymous();
    }

    public static ClaimsPrincipal CreatePrincipal(string name, string role)
        => new(new ClaimsIdentity(
            AllowedRoles(role).Select(r => new Claim(ClaimTypes.Role, r))
                .Append(new Claim(RoleClaimType, role))
                .Prepend(new Claim(ClaimTypes.Name, name)),
            AuthType));

    public static bool VerifyPin(IConfiguration config, string role, string pin)
    {
        var expected = config[$"Auth:Pins:{role}"];
        if (string.IsNullOrEmpty(expected)) return false;
        return Hash($"{role}:{pin}") == expected;
    }

    public static string RoleForName(string name)
        => (name ?? "").Trim().ToLowerInvariant() switch
        {
            "admin" or "administrator" => "Admin",
            "service" => "Service",
            _ => "Operator"
        };

    private const string RoleClaimType = "uiembed:role";

    private void SetPrincipal(string name, string role)
    {
        _principal = CreatePrincipal(name, role);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private void SetAnonymous()
    {
        _principal = AnonymousPrincipal();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private async Task<bool> CallLoginAsync(string name, string role, string pin)
    {
        try
        {
            return await _js.InvokeAsync<bool>("uiauth.login", name, role, pin);
        }
        catch (JSException) { return false; }
        catch (InvalidOperationException) { return false; }
        catch (JSDisconnectedException) { return false; }
    }

    private async Task CallLogoutAsync()
    {
        try
        {
            await _js.InvokeVoidAsync("uiauth.logout");
        }
        catch (JSException) { }
        catch (InvalidOperationException) { }
        catch (JSDisconnectedException) { }
    }

    private static string[] AllowedRoles(string role) => role switch
    {
        "Admin" => new[] { "Operator", "Admin", "Service" },
        _ => new[] { role }
    };

    private static string Hash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }

    private static ClaimsPrincipal AnonymousPrincipal() => new(new ClaimsIdentity());
}