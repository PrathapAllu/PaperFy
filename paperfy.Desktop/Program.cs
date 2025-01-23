using Avalonia;
using Avalonia.ReactiveUI;
using PaperFy.Shared.AppManager;
using PaperFy.Shared.Windows.Services;
using System;

namespace Paperfy.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            ApplicationManager.PlatformSystemService = new WindowsSystemService();
            var screenCapture = new WindowsScreenCaptureService();
            ApplicationManager.ScreenCaptureService = screenCapture;
            ApplicationManager.ControlCaptureService = new WindowsControlCaptureService();
            ApplicationManager.Run(BuildAvaloniaApp, args);
        }
        catch (Exception exception)
        {
            
        }
    }


    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}
