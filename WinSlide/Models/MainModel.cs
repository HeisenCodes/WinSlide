using AutoIt;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Runtime.InteropServices;
using WinSlide.AppSettings;
using WinSlide.Enums;
namespace WinSlide.Models;

public class MainModel : IDisposable
{
    public int _edgeThreshold = 1; // pixels from bottom considered "at edge"
    public ScrollSensitivity _scrollSenstivity = ScrollSensitivity.High;
    private int _cumulativeDelta = 0; // Track cumulative scroll movements
    private int? _lastNotchDelta = null; // Track the previous notch delta (up/down)

    private const int WH_MOUSE_LL = 14;
    private const int WM_MOUSEWHEEL = 0x020A;
    private const int SM_CYSCREEN = 1;

    private IntPtr _mouseHookID = IntPtr.Zero;
    private LowLevelMouseProc _proc;
    private int screenHeight;

    public MainModel(IOptions<MySettings> settings)
    {
        // Restore settings
        _edgeThreshold = settings.Value.EdgeThreshold;
        _scrollSenstivity = settings.Value.ScrollSensitivity;

        // Cache screen height
        screenHeight = GetSystemMetrics(SM_CYSCREEN); // physical pixels

        // Set the hook
        _proc = HookCallback;
        _mouseHookID = SetHook(_proc);
    }

    private IntPtr SetHook(LowLevelMouseProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }
    }


    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_MOUSEWHEEL)
        {
            MSLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);

            if (hookStruct.pt.y >= screenHeight - _edgeThreshold)
            {
                short delta = (short)((hookStruct.mouseData >> 16) & 0xffff);

                int notchDelta = delta / 120; // Convert delta to number of notches (120 per notch)

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

        return CallNextHookEx(_mouseHookID, nCode, wParam, lParam);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll")]
    static extern int GetSystemMetrics(int nIndex);

    public void Dispose()
    {
        // Unhook the mouse hook when the window is closed
        UnhookWindowsHookEx(_mouseHookID);
    }
}
