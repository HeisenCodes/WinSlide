using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Text;
using System.Text.Json;
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
            StartOnStartup = startupService.IsAppSetToStartOnStartup();
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
            get => model.EdgeThreshold;
            set
            {
                model.EdgeThreshold = value;
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

        public void SaveSettings()
        {
            var settings = new SettingsModel
            {
                EdgeThreshold = this.EdgeThreshold,
                ScrollSensitivity = this.ScrollSensitivity
            };

            string json = JsonSerializer.Serialize(settings,
                new JsonSerializerOptions { WriteIndented = true });

            string path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

            File.WriteAllText(path, json, Encoding.UTF8);
        }
    }
}
