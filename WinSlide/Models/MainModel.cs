using AutoIt;
using Gma.System.MouseKeyHook;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.Windows.Forms;
using WinSlide.Enums;
namespace WinSlide.Models;

public class MainModel : IDisposable
{
    private int _edgeThreshold_dpi_scaled; // distance from bottom considered "at edge"
    public ScrollSensitivity _scrollSenstivity = ScrollSensitivity.High;
    private int _cumulativeDelta = 0; // Track cumulative scroll movements
    private int? _lastNotchDelta = null; // Track the previous notch delta (up/down)

    private int screenHeight;
    private float dpiScale;

    private IKeyboardMouseEvents _globalMouseHook;

    public MainModel(IOptions<SettingsModel> settings)
    {
        // Get DPI scaling factor
        dpiScale = GetDpiScale();

        // Restore settings
        EdgeThreshold = settings.Value.EdgeThreshold;
        _scrollSenstivity = settings.Value.ScrollSensitivity;

        // Cache screen height 
        screenHeight = Screen.PrimaryScreen.Bounds.Height; // Physical pixels adjusted for DPI scaling


        // Initialize the global mouse hook
        _globalMouseHook = Hook.GlobalEvents();

        // Subscribe to the mouse wheel event
        _globalMouseHook.MouseWheel += GlobalMouseHook_MouseWheel;
    }

    public int EdgeThreshold
    {
        get => field;
        set
        {
            field = value;
            _edgeThreshold_dpi_scaled = (int)(value * dpiScale);
        }
    }

    // Method to retrieve DPI scale for the current screen
    private float GetDpiScale()
    {
        // Get DPI scale (e.g., 1.25 for 125% scaling, 1.5 for 150% scaling)
        using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
        {
            return g.DpiX / 96f; // Standard DPI is 96, so this gives the scaling factor.
        }
    }

    // Event handler for the mouse wheel
    private void GlobalMouseHook_MouseWheel(object? sender, MouseEventArgs e)
    {
        if (e.Y >= screenHeight - _edgeThreshold_dpi_scaled)
        {
            // Convert delta to number of notches (120 per notch)
            int notchDelta = e.Delta / 120;

            // Check if opposite scroll input is detected (scroll up after scroll down, or vice versa)
            if (_lastNotchDelta.HasValue && notchDelta != _lastNotchDelta.Value)
            {
                // Clear cumulative notches if the direction is opposite
                _cumulativeDelta = 0;
            }

            // Adjust cumulative notches based on the scroll direction
            _cumulativeDelta += notchDelta;

            // Check if the cumulative notches exceed the threshold based on sensitivity
            if (Math.Abs(_cumulativeDelta) >= (int)_scrollSenstivity)
            {
                // If scrolling up (positive delta), send left hotkey
                if (_cumulativeDelta > 0)
                {
                    AutoItX.Send("#^{LEFT}");
                }
                // If scrolling down (negative delta), send right hotkey
                else if (_cumulativeDelta < 0)
                {
                    AutoItX.Send("#^{RIGHT}");
                }

                // Reset cumulative notches after action is triggered
                _cumulativeDelta = 0;
            }

            // Store the current notch direction for the next comparison
            _lastNotchDelta = notchDelta;
        }
    }

    public void Dispose()
    {
        // Unhook the mouse hook when the window is closed
        _globalMouseHook.Dispose();
    }
}