using System.ComponentModel;
using System.Windows;
using WinSlide.Interface;
using WinSlide.ViewModels;
namespace WinSlide.Views;

public partial class MainWindow : Window
{
    private readonly IWindowService windowService;

    MainWindowViewModel ViewModel { get; set; }

    public MainWindow(MainWindowViewModel viewModel, IWindowService windowService)
    {
        InitializeComponent();
        this.DataContext = ViewModel = viewModel;
        this.windowService = windowService;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        ViewModel.SaveSettings(); // Save Settings
        windowService.Close();
    }
}
