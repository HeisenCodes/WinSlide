# WinSlide

A simple WPF (Windows Presentation Foundation) application that allows users to switch between Windows virtual desktops by scrolling at the bottom edge of the screen.

Easily navigate between virtual desktops without needing to use keyboard shortcuts or taskbar buttons.



# Libraries Used

[AutoItX .NET Assembly](https://www.autoitscript.com/forum/topic/177167-using-autoitx-from-c-net/) - Used to send Windows virtual desktop switching key commands.

[Gma.System.MouseKeyHook](https://github.com/gmamaladze/globalmousekeyhook) - For global mouse hook detection and scroll event handling.



# Getting Started

### **Installation**

1. Download the latest release from the *Releases* section.
2. Extract and run the executable — no installer required.

### **Usage**

* The app starts minimized with a **tray icon**.
* Click the tray icon to open the **GUI** window.
* Adjust:

  * **Edge Threshold** — distance in pixels from the bottom edge.
  * **Scroll Sensitivity** — scroll amount required to trigger a switch.

### **Uninstall**

Simply delete the executable.