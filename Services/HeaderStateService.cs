namespace UiEmbed.Services;

public enum HeaderMode
{
    Status,
    Title,
    Hidden
}

/// <summary>
/// Controls the dynamic zone of the persistent header. The ESCO brand block
/// always renders; views opt into Status (cabinet banner), Title (custom
/// text), or Hidden (logo only, the default).
/// </summary>
public sealed class HeaderStateService
{
    private HeaderMode _mode = HeaderMode.Hidden;
    private string _title = string.Empty;

    public event Action? OnChange;

    public HeaderMode Mode => _mode;
    public string Title => _title;

    public void ShowStatus() => Set(HeaderMode.Status, string.Empty);
    public void ShowTitle(string title) => Set(HeaderMode.Title, title);
    public void ShowLogoOnly() => Set(HeaderMode.Hidden, string.Empty);

    private void Set(HeaderMode mode, string title)
    {
        _mode = mode;
        _title = title;
        OnChange?.Invoke();
    }
}
