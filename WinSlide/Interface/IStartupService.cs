namespace WinSlide.Interface;

public interface IStartupService
{
    void SetAppToStartOnStartup(bool startOnStartup);
    bool IsAppSetToStartOnStartup();
}
