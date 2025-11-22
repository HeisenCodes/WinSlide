using Microsoft.Win32;
using WinSlide.Interface;
namespace WinSlide.Services;

public class StartupService : IStartupService
{
    private const string AppName = "WinSlide";
    private const string StartupRegistryPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    public void SetAppToStartOnStartup(bool enable)
    {
        using var key = Registry.CurrentUser.OpenSubKey(StartupRegistryPath, writable: true);

        if (key == null)
            return; // Cannot write — silently ignore.

        if (enable)
        {
            // Resolve the full path to the current executable
            string exePath = Environment.ProcessPath!;

            // Put quotes around path so it works with spaces
            key.SetValue(AppName, $"\"{exePath}\"", RegistryValueKind.String);
        }
        else
        {
            key.DeleteValue(AppName, throwOnMissingValue: false);
        }
    }

    public bool IsAppSetToStartOnStartup()
    {
        using var key = Registry.CurrentUser.OpenSubKey(StartupRegistryPath, writable: false);

        if (key?.GetValue(AppName) is string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        return false;
    }
}
