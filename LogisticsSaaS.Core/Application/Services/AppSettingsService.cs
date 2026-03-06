namespace LogisticsSaaS.Core.Application.Services;

public class AppSettingsService
{
    public string Currency { get; set; } = "USD ($)";
    public string Language { get; set; } = "English";
    public bool IsDarkMode { get; set; } = true;
}
