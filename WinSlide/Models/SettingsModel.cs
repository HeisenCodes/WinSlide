using System.ComponentModel.DataAnnotations;
using WinSlide.Enums;

namespace WinSlide.Models;

public class SettingsModel
{
    [Range(1, 50)]
    public int EdgeThreshold { get; set; }

    [Range(1, 3)]
    public ScrollSensitivity ScrollSensitivity { get; set; }
}
