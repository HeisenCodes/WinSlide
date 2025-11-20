using System.Windows.Controls;
using WinSlide.ViewModels;
namespace WinSlide.Views;

public partial class TrayIconView : UserControl, IDisposable
{
    public TrayIconView(TrayIconViewModel viewModel)
    {
        InitializeComponent();
        this.DataContext = viewModel;
    }

    public void Dispose()
    {
        TrayIcon.Dispose();
    }
}
