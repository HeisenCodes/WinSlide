using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using WinSlide.AppSettings;
using WinSlide.Enums;
using WinSlide.Interface;
using WinSlide.Models;

namespace WinSlide.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly MainModel model;
        private readonly IStartupService _startupService;
        private bool _startOnStartup;

        public MainWindowViewModel(MainModel model, IStartupService startupService)
        {
            this.model = model;
            this._startupService = startupService;

            // Load the current startup setting
            StartOnStartup = IsAppSetToStartOnStartup();
        }

        public bool StartOnStartup
        {
            get => _startOnStartup;
            set
            {
                if (_startOnStartup != value)
                {
                    _startOnStartup = value;
                    _startupService.SetAppToStartOnStartup(value);
                    OnPropertyChanged();
                }
            }
        }

        public int EdgeThreshold
        {
            get => model._edgeThreshold;
            set
            {
                model._edgeThreshold = value;
                OnPropertyChanged();
            }
        }

        public ScrollSensitivity ScrollSensitivity
        {
            get => model._scrollSenstivity;
            set
            {
                model._scrollSenstivity = value;
                OnPropertyChanged();
            }
        }

        private bool IsAppSetToStartOnStartup()
        {
            // Check if the app is currently set to start on startup
            var registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            var appPath = registryKey.GetValue("WinSlide")?.ToString();
            return !string.IsNullOrEmpty(appPath);
        }

        public void SaveSettings()
        {
            var mySettings = new MySettings
            {
                EdgeThreshold = this.EdgeThreshold,
                ScrollSensitivity = this.ScrollSensitivity
            };

            var wrapper = new SettingsWrapper(mySettings);

            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json"), JsonSerializer.Serialize(wrapper, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}
