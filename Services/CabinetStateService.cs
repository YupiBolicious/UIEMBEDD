namespace UiEmbed.Services;

public sealed class CabinetStateService
{
    private string _bscId = "BSC-1";
    private string _serialNumber = "2024-000001";
    private string _operatorName = "Operator";
    private int _temperatureC = 25;
    private string _timeText = "4:39 PM";
    private string _dateText = "Oct 07 2024";
    private string _sashStatus = "Safe height";
    private int _filterLifePercent = 100;
    private string _filterLifeLabel = "Excellent";
    private double _downflowMs = 0.32;
    private double _inflowMs = 0.45;
    private bool _isSafe = true;

    public event Action? OnChange;

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
}
