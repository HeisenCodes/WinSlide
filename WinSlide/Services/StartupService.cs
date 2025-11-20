using Microsoft.Win32;
using System.IO;
using WinSlide.Interface;
namespace WinSlide.Services;

public class StartupService : IStartupService
{
    private const string AppName = "WinSlide";
    private const string StartupRegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    public void SetAppToStartOnStartup(bool startOnStartup)
    {
        var registryKey = Registry.CurrentUser.OpenSubKey(StartupRegistryKey, true);

        if (startOnStartup)
        {
            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{AppName}.exe");
            registryKey.SetValue(AppName, exePath);
        }
        else
        {
            registryKey.DeleteValue(AppName, false);
        }
    }
}
