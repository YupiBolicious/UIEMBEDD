namespace UiEmbed.Services;

public enum HmiAlertLevel
{
    Warning,   // Orange / Amber header banner (as in Wired Connection reference)
    Danger,    // Crimson / Red header banner
    Info,      // Sky / Cyan header banner
    Success    // Emerald / Green header banner
}

/// <summary>
/// Options payload passed when raising an HMI modal alert.
/// </summary>
public class HmiAlertOptions
{
    public string Title { get; set; } = "Wired Connection";
    public string Message { get; set; } = "Wired connection has been configured!\nPlease restart the system to perform the configuration.";
    public HmiAlertLevel Level { get; set; } = HmiAlertLevel.Warning;
    public string ConfirmText { get; set; } = "Restart";
    public string CancelText { get; set; } = "Later";
    public bool ShowCancel { get; set; } = true;
    public bool DismissableBackdrop { get; set; } = false;
}

/// <summary>
/// Global injectable C# Blazor alert/notification service (hook/helper).
/// Inject @inject IHmiAlertService AlertService into any Blazor component or page
/// and call:
///   await AlertService.ShowAsync("Wired Connection", "Wired connection has been configured!...", confirmText: "Restart", cancelText: "Later");
/// Or customized via HmiAlertOptions.
/// </summary>
public interface IHmiAlertService
{
    event Action? OnChange;
    bool IsVisible { get; }
    HmiAlertOptions CurrentOptions { get; }

    Task<bool> ShowAsync(HmiAlertOptions options);
    Task<bool> ShowAsync(string title, string message, HmiAlertLevel level = HmiAlertLevel.Warning, string confirmText = "Restart", string cancelText = "Later");
    Task<bool> ShowWarningAsync(string title, string message, string confirmText = "Restart", string cancelText = "Later");
    Task<bool> ShowInfoAsync(string title, string message, string confirmText = "OK");

    void Confirm();
    void Cancel();
}

public class HmiAlertService : IHmiAlertService
{
    public event Action? OnChange;
    public bool IsVisible { get; private set; } = false;
    public HmiAlertOptions CurrentOptions { get; private set; } = new();

    private TaskCompletionSource<bool>? _tcs;

    public Task<bool> ShowAsync(HmiAlertOptions options)
    {
        CurrentOptions = options ?? new();
        IsVisible = true;
        _tcs = new TaskCompletionSource<bool>();
        NotifyStateChanged();
        return _tcs.Task;
    }

    public Task<bool> ShowAsync(string title, string message, HmiAlertLevel level = HmiAlertLevel.Warning, string confirmText = "Restart", string cancelText = "Later")
    {
        return ShowAsync(new HmiAlertOptions
        {
            Title = title,
            Message = message,
            Level = level,
            ConfirmText = confirmText,
            CancelText = cancelText,
            ShowCancel = !string.IsNullOrEmpty(cancelText)
        });
    }

    public Task<bool> ShowWarningAsync(string title, string message, string confirmText = "Restart", string cancelText = "Later")
    {
        return ShowAsync(title, message, HmiAlertLevel.Warning, confirmText, cancelText);
    }

    public Task<bool> ShowInfoAsync(string title, string message, string confirmText = "OK")
    {
        return ShowAsync(new HmiAlertOptions
        {
            Title = title,
            Message = message,
            Level = HmiAlertLevel.Info,
            ConfirmText = confirmText,
            ShowCancel = false
        });
    }

    public void Confirm()
    {
        IsVisible = false;
        NotifyStateChanged();
        _tcs?.TrySetResult(true);
    }

    public void Cancel()
    {
        IsVisible = false;
        NotifyStateChanged();
        _tcs?.TrySetResult(false);
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
