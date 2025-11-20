using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using WinSlide.AppSettings;
using WinSlide.Interface;
using WinSlide.Models;
using WinSlide.Services;
using WinSlide.ViewModels;
using WinSlide.Views;


namespace WinSlide;

public partial class App : Application
{
    // Mutex to ensure a single instance of the app
    private static Mutex _mutex;

    [STAThread]
    public static void Main(string[] args)
    {
        // Create a mutex with a unique name (this ensures only one instance is running)
        bool isNewInstance;
        _mutex = new Mutex(true, "{E07D58D1-FB24-4697-9834-5B9C86102A6A}", out isNewInstance);

        if (isNewInstance)
        {

            using IHost host = CreateHostBuilder(args).Build();
            host.Start();

            // Force creation of the MainModel
            host.Services.GetRequiredService<MainModel>();

            // Force creation of the tray icon
            host.Services.GetRequiredService<TrayIconView>();

            App app = new();
            app.InitializeComponent();
            app.Run();
        }
        else
        {
            MessageBox.Show("The application is already running.", "App Already Running", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) => { })
        .ConfigureServices((hostContext, services) =>
        {
            services.AddSingleton<TrayIconView>();
            services.AddSingleton<TrayIconViewModel>();

            services.AddTransient<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();

            services.AddSingleton<MainModel>();

            services.AddSingleton<IStartupService, StartupService>();
            services.AddSingleton<IWindowService, WindowService>();

            services.AddOptions<MySettings>()
                .Configure(options =>
                {
                    // HARD DEFAUTS
                    options.EdgeThreshold = 1;
                    options.ScrollSensitivity = Enums.ScrollSensitivity.High;
                })
                .Bind(hostContext.Configuration.GetSection("Settings"))
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
}
