using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WinSlide.Interface;
using WinSlide.Views;

namespace WinSlide.Services;

public class WindowService : IWindowService
{
    private readonly IServiceProvider _services;
    private MainWindow? _mainWindow;

    public WindowService(IServiceProvider services)
    {
        this._services = services;
    }

    public void Close()
    {
        _mainWindow = null;
    }

    public void Show()
    {
        // If there is an open window, bring it to the front
        if (_mainWindow != null && _mainWindow.IsVisible)
        {
            if (_mainWindow.WindowState == WindowState.Minimized)
            {
                _mainWindow.WindowState = WindowState.Normal;
            }

            _mainWindow.Activate();  // Bring the window to the front
            return;  // Exit to avoid opening a new instance
        }

        // Resolve the window from DI (Dependency Injection)
        _mainWindow = _services.GetRequiredService<MainWindow>();

        // Calculate the center position on the screen
        var screenWidth = SystemParameters.PrimaryScreenWidth;
        var screenHeight = SystemParameters.PrimaryScreenHeight;

        // Set the window's position to the center of the screen
        _mainWindow.Left = (screenWidth - _mainWindow.Width) / 2;
        _mainWindow.Top = (screenHeight - _mainWindow.Height) / 2;

        // Show the window
        _mainWindow.Show();
    }
}
