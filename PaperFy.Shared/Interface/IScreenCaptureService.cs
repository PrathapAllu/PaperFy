using PaperFy.Shared.Windows.Models;

namespace PaperFy.Shared.Interface
{
    public interface IScreenCaptureService
    {
        TimeSpan Frequency { get; set; }

        bool IsRunning { get; }

        byte[] GetImmediateScreenshot(Point point, long timestamp, bool excludeTaskbar = false);

        byte[] GetPriorScreenshot(Point point, long timestamp, bool excludeTaskbar = false);

        Screen GetScreen(Point point);

        void Pause();

        void Resume();

        void Start();

        void Stop();
    }
}