using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using WinSlide.Interface;
using WinSlide.Models;
using WinSlide.Services;
using WinSlide.ViewModels;
using WinSlide.Views;


namespace WinSlide;

public partial class App : Application
{
    private TaskbarIcon notifyIcon;

    private IServiceProvider Services { get; set; }

    [STAThread]
    public static void Main(string[] args)
    {
        using var mutex = new Mutex(true, "{E07D58D1-FB24-4697-9834-5B9C86102A6A}", out bool isNewInstance);

        if (!isNewInstance)
        {
            MessageBox.Show("The application is already running.");
            return;
        }

        using IHost host = CreateHostBuilder(args).Build();
        host.Start();

        App app = new();
        app.Services = host.Services;      // <-- STORE DI container here
        host.Services.GetRequiredService<MainModel>(); // Force creation of the MainModel

        app.InitializeComponent();
        app.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {

            services.AddSingleton<TrayIconViewModel>();

            services.AddTransient<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();

            services.AddSingleton<MainModel>();

            services.AddSingleton<IStartupService, StartupService>();
            services.AddSingleton<IWindowService, WindowService>();

            services.AddOptions<SettingsModel>()
                .Configure(options =>
                {
                    // HARD DEFAUTS
                    options.EdgeThreshold = 1;
                    options.ScrollSensitivity = Enums.ScrollSensitivity.High;
                })
                .Bind(hostContext.Configuration)
                .PostConfigure(options =>
                {
                    // VALIDATION + FALLBACK
                    if (options.EdgeThreshold < 1 || options.EdgeThreshold > 50)
                    {
                        options.EdgeThreshold = 1;
                    }

                    if (!Enum.IsDefined(typeof(Enums.ScrollSensitivity), options.ScrollSensitivity))
                    {
                        options.ScrollSensitivity = Enums.ScrollSensitivity.High;
                    }
                }
                );
        });

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");
        notifyIcon.DataContext = Services.GetRequiredService<TrayIconViewModel>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
        base.OnExit(e);
    }
}
