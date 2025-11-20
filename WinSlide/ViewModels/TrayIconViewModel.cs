using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using WinSlide.Interface;

namespace WinSlide.ViewModels;

public partial class TrayIconViewModel : ObservableObject
{
    private readonly IWindowService windowService;

    public TrayIconViewModel(IWindowService windowService)
    {
        this.windowService = windowService;
    }

    [RelayCommand]
    private void Show()
    {
        windowService.Show();
    }

    [RelayCommand]
    private void Exit()
    {
        Application.Current.Shutdown();
    }
}
