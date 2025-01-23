using PaperFy.Shared.Windows.Models;

namespace PaperFy.Shared.Interface
{
    public interface IScreenCaptureService
    {
        TimeSpan Frequency { get; set; }

        bool IsRunning { get; }

        byte[] GetImmediateScreenshot(Point point, long timestamp);

        byte[] GetPriorScreenshot(Point point, long timestamp);

        Screen GetScreen(Point point);

        void Pause();

        void Resume();

        void Start();

        void Stop();
    }
}