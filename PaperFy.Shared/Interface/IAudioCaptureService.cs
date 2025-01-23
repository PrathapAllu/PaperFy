namespace PaperFy.Shared.Interface
{
    public interface IAudioCaptureService : IDisposable
    {
        internal const int DefaultFrameLength = 512;

        bool IsRunning { get; }

        string[] Sources { get; }

        string GetFilePath(Guid sessionID);

        void Pause();

        void Resume();

        Guid Start(int sourceIndex, int frameLength = 512);

        byte[] Stop();
    }
}