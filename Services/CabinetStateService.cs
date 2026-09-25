namespace UiEmbed.Services;

using System.Globalization;

public sealed class CabinetStateService : IDisposable
{
    private readonly System.Threading.Timer _clockTimer;

    private string _bscId = "BSC-1";
    private string _serialNumber = "2024-000001";
    private string _operatorName = "Operator";
    private int _temperatureC = 25;
    private string _timeText = string.Empty;
    private string _dateText = string.Empty;
    private string _sashStatus = "Safe height";
    private int _filterLifePercent = 100;
    private string _filterLifeLabel = "Excellent";
    private double _downflowMs = 0.32;
    private double _inflowMs = 0.45;
    private bool _isSafe = true;
    private bool _blowerOn = true;
    private bool _lightOn = true;
    private int _lightLevelPct = 100;
    private bool _uvOn;
    private bool _alarmMuted = true;
    private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Today);
    private bool _showCalendar;
    private bool _showUser;
    private CabinetCondition _condition = CabinetCondition.Safe;

    public event Action? OnChange;

    public CabinetStateService()
    {
        var now = DateTime.Now;
        _timeText = now.ToString("h:mm tt", CultureInfo.InvariantCulture);
        _dateText = FormatDate(now);
        _clockTimer = new System.Threading.Timer(_ => Tick(), null, 1000, 1000);
    }

    private static string FormatDate(DateTime date) =>
        date.ToString("MMM dd yyyy", CultureInfo.InvariantCulture);

    private void Tick()
    {
        var now = DateTime.Now;
        _timeText = now.ToString("h:mm tt", CultureInfo.InvariantCulture);
        // Keep the calendar-chosen date until the real clock reaches it.
        if (_selectedDate == DateOnly.FromDateTime(now))
        {
            _dateText = FormatDate(now);
        }
        OnChange?.Invoke();
    }

    public void Dispose() => _clockTimer.Dispose();

    public string BscId { get => _bscId; set { _bscId = value; NotifyStateChanged(); } }
    public string SerialNumber { get => _serialNumber; set { _serialNumber = value; NotifyStateChanged(); } }
    public string OperatorName { get => _operatorName; set { _operatorName = value; NotifyStateChanged(); } }
    public int TemperatureC { get => _temperatureC; set { _temperatureC = value; NotifyStateChanged(); } }
    public string TimeText { get => _timeText; set { _timeText = value; NotifyStateChanged(); } }
    public string DateText { get => _dateText; set { _dateText = value; NotifyStateChanged(); } }
    public string SashStatus { get => _sashStatus; set { _sashStatus = value; NotifyStateChanged(); } }
    public int FilterLifePercent { get => _filterLifePercent; set { _filterLifePercent = value; NotifyStateChanged(); } }
    public string FilterLifeLabel { get => _filterLifeLabel; set { _filterLifeLabel = value; NotifyStateChanged(); } }
    public double DownflowMs { get => _downflowMs; set { _downflowMs = value; NotifyStateChanged(); } }
    public double InflowMs { get => _inflowMs; set { _inflowMs = value; NotifyStateChanged(); } }
    public bool IsSafe { get => _isSafe; set { _isSafe = value; NotifyStateChanged(); } }
    public bool BlowerOn { get => _blowerOn; set { _blowerOn = value; NotifyStateChanged(); } }
    public bool LightOn { get => _lightOn; set { _lightOn = value; NotifyStateChanged(); } }
    public int LightLevelPct { get => _lightLevelPct; set { _lightLevelPct = Math.Clamp(value, 0, 100); NotifyStateChanged(); } }
    public bool UvOn { get => _uvOn; set { _uvOn = value; NotifyStateChanged(); } }
    public bool AlarmMuted { get => _alarmMuted; set { _alarmMuted = value; NotifyStateChanged(); } }
    public DateOnly SelectedDate { get => _selectedDate; set { _selectedDate = value; NotifyStateChanged(); } }
    public bool ShowCalendar { get => _showCalendar; set { _showCalendar = value; NotifyStateChanged(); } }
    public bool ShowUser{get => _showUser; set {_showUser=value; NotifyStateChanged();}}

    public CabinetCondition Condition { get => _condition; set { _condition = value; NotifyStateChanged(); } }

    public string TemperatureText => $"Temp: {TemperatureC}°C";
    public string SashText => $"Sash : {SashStatus}";
    public string FilterLifeText => $"Filter Life : {FilterLifePercent}% ({FilterLifeLabel})";
    public string DownflowText => $"Downflow : {DownflowMs:F2} m/s";
    public string InflowText => $"Inflow : {InflowMs:F2} m/s";
    public string BscDisplay => $"{BscId} / {SerialNumber}";

    private void NotifyStateChanged() => OnChange?.Invoke();
    public void NotifyChanged() => OnChange?.Invoke();

    public void UpdateDownflow(double v) => DownflowMs = v;
    public void UpdateInflow(double v) => InflowMs = v;
    public void UpdateTemperature(int v) => TemperatureC = v;
    public void SetOperator(string v) => OperatorName = v;
    public void ToggleBlower() => BlowerOn = !BlowerOn;
    public void ToggleLight() => LightOn = !LightOn;
    public void ToggleUv() => UvOn = !UvOn;
    public void ToggleAlarmMuted() => AlarmMuted = !AlarmMuted;

    public void SetLightLevel(int pct)
    {
        LightLevelPct = pct;
        LightOn = LightLevelPct > 0;
    }

    public void OpenCalendar() => ShowCalendar = true;
    public void CloseCalendar() => ShowCalendar = false;
    public void OpenUser() => ShowUser = true;
    public void CloseUser() => ShowUser = false;

    public void ConfirmDate(DateOnly date)
    {
        SelectedDate = date;
        DateText = date.ToString("MMM dd yyyy", CultureInfo.InvariantCulture);
        ShowCalendar = false;
    }
}
