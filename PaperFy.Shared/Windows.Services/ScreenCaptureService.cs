using PaperFy.Shared.Interface;
using PaperFy.Shared.Windows.Models;
using PaperFy.Shared.Windows.Services;

namespace PaperFy.Shared.Windows.Service
{
    public abstract class ScreenCaptureService<TImage> : IScreenCaptureService
    {
        private static readonly object SyncObject = new object();

        private static readonly TimeSpan DefaultFrequency = TimeSpan.FromMilliseconds(500.0);

        private const int MaximumScreenBufferSize = 10;

        private Dictionary<Screen, Queue<(long, TImage)>> ScreenBuffer = new Dictionary<Screen, Queue<(long, TImage)>>();

        public TimeSpan Frequency { get; set; } = DefaultFrequency;

        public bool IsPaused { get; private set; }

        public bool IsRunning { get; private set; }

        public Screen GetScreen(Point point)
        {
            return Screen.FromPoint(point);
        }

        public byte[] GetImmediateScreenshot(Point point, long timestamp)
        {
            Screen screen = Screen.FromPoint(point);
            lock (SyncObject)
            {
                TImage val = CaptureScreen(screen);
                if (ScreenBuffer.ContainsKey(screen))
                {
                    ScreenBuffer[screen].Enqueue((timestamp, val));
                }
                return EncodeNativeImage(val);
            }
        }

        public byte[] GetPriorScreenshot(Point point, long timestamp)
        {
            Screen screen = Screen.FromPoint(point);
            lock (SyncObject)
            {
                if (!ScreenBuffer.ContainsKey(screen))
                {
                    return EncodeNativeImage(CaptureScreen(screen));
                }
                (long, TImage)[] array = ScreenBuffer[screen].ToArray();
                for (int num = array.Length - 1; num >= 0; num--)
                {
                    try
                    {
                        if (array[num].Item1 <= timestamp)
                        {
                            return EncodeNativeImage(array[num].Item2);
                        }
                    }
                    catch (Exception ex)
                    {
                        return EncodeNativeImage(CaptureScreen(screen));
                    }
                }
                try
                {
                    return EncodeNativeImage(CaptureScreen(screen));
                }
                catch (Exception ex2)
                {
                    return null;
                }
            }
        }

        public void Pause()
        {
            lock (SyncObject)
            {
                if (IsRunning && !IsPaused)
                {
                    IsPaused = true;
                }
            }
        }

        public void Resume()
        {
            if (IsPaused)
            {
                IsPaused = false;
            }
        }

        public void Start()
        {
            lock (SyncObject)
            {
                if (!IsRunning)
                {
                    IsRunning = true;
                    InitializeCaptureTask();
                }
            }
        }

        public void Stop()
        {
            lock (SyncObject)
            {
                if (!IsRunning)
                {
                    return;
                }
                IsPaused = false;
                IsRunning = false;
                Thread.Sleep(Frequency);
                foreach (Screen item in ScreenBuffer.Keys.ToList())
                {
                    if (ScreenBuffer.TryGetValue(item, out var value))
                    {
                        while (value.Count > 0)
                        {
                            (value.Dequeue().Item2 as IDisposable)?.Dispose();
                        }
                    }
                }
                ScreenBuffer.Clear();
            }
        }

        private void CaptureScreens(long timestamp)
        {
            foreach (Screen item2 in new List<Screen>(Screen.Screens))
            {
                TImage item = CaptureScreen(item2);
                lock (SyncObject)
                {
                    if (!IsRunning)
                    {
                        break;
                    }
                    if (!ScreenBuffer.ContainsKey(item2))
                    {
                        ScreenBuffer[item2] = new Queue<(long, TImage)>();
                    }
                    ScreenBuffer[item2].Enqueue((timestamp, item));
                    while (ScreenBuffer[item2].Count > 10)
                    {
                        (ScreenBuffer[item2].Dequeue().Item2 as IDisposable)?.Dispose();
                    }
                }
            }
        }

        protected abstract TImage CaptureScreen(Screen screen);

        protected abstract byte[] EncodeNativeImage(TImage nativeImage);

        private void InitializeCaptureTask()
        {
            Task.Run(delegate
            {
                while (IsRunning)
                {
                    long currentTimestamp = SystemService.Instance.CurrentTimestamp;
                    if (!IsPaused)
                    {
                        CaptureScreens(currentTimestamp);
                    }
                    double num = Frequency.TotalMilliseconds - (double)(SystemService.Instance.CurrentTimestamp - currentTimestamp);
                    if (num > 0.0)
                    {
                        Thread.Sleep((int)num);
                    }
                }
            });
        }
    }
}


