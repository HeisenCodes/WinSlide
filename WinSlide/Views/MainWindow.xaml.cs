using System.ComponentModel;
using System.Windows;
using WinSlide.ViewModels;
namespace WinSlide.Views;

public partial class MainWindow : Window
{
    MainWindowViewModel ViewModel { get; set; }

    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        this.DataContext = ViewModel = viewModel;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        ViewModel.SaveSettings(); // Save Settings
    }
}
