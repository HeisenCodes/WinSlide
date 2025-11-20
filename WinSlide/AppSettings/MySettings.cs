using System.ComponentModel.DataAnnotations;
using WinSlide.Enums;

namespace WinSlide.AppSettings;

public class SettingsWrapper
{
    public SettingsWrapper(MySettings settings)
    {
        Settings = settings;
    }

    public MySettings Settings { get; set; }

}

public class MySettings
{
    [Range(1, 50)]
    public int EdgeThreshold { get; set; }

    [Range(1, 3)]
    public ScrollSensitivity ScrollSensitivity { get; set; }
}
