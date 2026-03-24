namespace RootControl.Models;

public sealed class AppSettings
{
    public string WebUrl { get; set; } = string.Empty;
    public bool RefreshEnabled { get; set; }
    public string RefreshInterval { get; set; } = "00:15";
    public bool RefreshOnIdleOnly { get; set; }
    public string IdleRefreshDelay { get; set; } = "00:05";
    public bool ForceHardRefresh { get; set; }
    public bool BlockInputs { get; set; }
    public string MasterPinHash { get; set; } = string.Empty;

    public bool HasMinimumConfiguration()
    {
        return !string.IsNullOrWhiteSpace(WebUrl)
            && !string.IsNullOrWhiteSpace(MasterPinHash);
    }
}
