using Avalonia;
using PaperFy.Shared.Interface;
using PaperFy.Shared.Windows.Services;

namespace PaperFy.Shared.AppManager
{
    public static class ApplicationManager
    {
        public static IAudioCaptureService AudioCaptureService { get; set; }

        public static IControlCaptureService ControlCaptureService { get; set; }

        public static IDialogService DialogService { get; set; }

        public static IPlatformSystemService PlatformSystemService { get; set; }

        public static IScreenCaptureService ScreenCaptureService { get; set; }

        public static IDocumenterService DocumenterService { get; internal set; }

        public static IimageProcessor IimageProcessor { get; set; }

        static ApplicationManager()
        {

        }

        public static void Run(Func<AppBuilder> applicationBuilder, string[] arguments)
        {
            if (!Directory.Exists(SystemService.Instance.LocalApplicationDataPath))
            {
                Directory.CreateDirectory(SystemService.Instance.LocalApplicationDataPath);
            }
            try
            {
                InitializeServices();
                applicationBuilder().StartWithClassicDesktopLifetime(arguments);
            }
            catch (Exception exception)
            {

            }

        }

        private static void InitializeServices()
        {
            IimageProcessor = new ImageProcessor();
            DocumenterService = new DocumenterService(ScreenCaptureService, ControlCaptureService);
        }
    }
}
