using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using PaperFy.Shared.Actions;
using PaperFy.Shared.AppManager;
using PaperFy.Shared.Capture;
using PaperFy.Shared.Common.Extensions;
using PaperFy.Shared.Interface;
using PaperFy.Shared.Windows.Events;
using PaperFy.Shared.Windows.Models;
using Point = PaperFy.Shared.Windows.Models.Point;
using Screenshot = PaperFy.Shared.Actions.Screenshot;

namespace PaperFy.Shared.Windows.Services
{
    public class DocumenterService : IDocumenterService, IDisposable
    {
        protected IScreenCaptureService ScreenCaptureService { get; }

        protected IControlCaptureService ControlCaptureService { get; }

        public IimageProcessor IimageProcessor { get; }

        public bool IsIdle => throw new NotImplementedException();

        public bool IsRunning => throw new NotImplementedException();

        public bool IsTranscribingAudio { get; set; }

        private long _previousEndTimestamp;

        public long? LastMaximized { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private MouseEvent LastMouseEvent = new MouseEvent(0, 0, MouseButton.Left, string.Empty, string.Empty, SystemService.Instance.CurrentTimestamp);

        public DocumenterService(IScreenCaptureService screenCaptureService, IControlCaptureService ControlCaptureService)
        {
            ScreenCaptureService = screenCaptureService;
            ControlCaptureService = ControlCaptureService;

            EventAggregator.Instance.Subscribe<KeyboardCaptureEvent>(OnKeyboardCaptureEvent);
            EventAggregator.Instance.Subscribe<MouseCaptureEvent>(OnMouseCaptureEvent);
        }

        public void StartCapture()
        {
            ScreenCaptureService.Start();
        }

        public void StopCapture()
        {
            ScreenCaptureService.Stop();
        }

        public void SaveScreenshot(byte[] screenshot, Point position, long timestamp)
        {
            string filename = $"screenshot_{timestamp}.png";
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "Paperfy",
                DateTime.Now.ToString("yyyy-MM-dd"),
                "screenshots",
                filename
            );

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, screenshot);
        }

        public (byte[], float, Point) CaptureScreenshot(Point position, long timestamp)
        {
            byte[] screenshot = ScreenCaptureService.GetImmediateScreenshot(position, timestamp);
            Screen screen = ScreenCaptureService.GetScreen(position);
            return (
                screenshot,
                screen?.Scaling ?? 1f,
                new Point((position.X - screen?.Bounds.X) ?? 0, (position.Y - screen?.Bounds.Y) ?? 0)
            );
        }

        private static readonly object SyncObject = new object();

        protected long StartTimestamp { get; private set; }

        private DocumentingState state;

        public DocumentingState State
        {
            get
            {
                return state;
            }
            set
            {
                if (state == value)
                {
                    return;
                }
                DocumentingState old = state;
                state = value;
                if (State != DocumentingState.Recording && State != DocumentingState.Paused)
                {
                    IScreenCaptureService screenCaptureService = ScreenCaptureService;
                    if (screenCaptureService != null && screenCaptureService.IsRunning)
                    {
                        ScreenCaptureService?.Stop();
                    }
                }
            }
        }

        public async void StartDocumenting(bool isTranscribingAudio)
        {
            lock (SyncObject)
            {
                try
                {
                    if (State != 0)
                    {
                        return;
                    }
                    StartTimestamp = SystemService.Instance.CurrentTimestamp;
                    SystemService.Instance.PlatformSystemService?.StartKeyboardListener();
                    SystemService.Instance.PlatformSystemService?.StartMouseListener();
                    State = DocumentingState.Recording;
                    IsTranscribingAudio = isTranscribingAudio;
                    ScreenCaptureService?.Start();
                }
                catch (Exception ex)
                {

                    StopDocumenting().Wait();
                }
            }
            try
            {
                if (IsTranscribingAudio)
                {
                    //TODO: Enable Audio Services
                }
            }
            catch (Exception ex2)
            {
                await StopDocumenting();
            }
        }

        public void Dispose()
        {
            EventAggregator.Instance.Unsubscribe<KeyboardCaptureEvent>(OnKeyboardCaptureEvent);
            EventAggregator.Instance.Unsubscribe<MouseCaptureEvent>(OnMouseCaptureEvent);
            StopDocumenting();
        }

        public void CancelDocumenting()
        {
            throw new NotImplementedException();
        }

        public void PauseDocumenting()
        {
            throw new NotImplementedException();
        }

        public void ResumeDocumenting()
        {
            throw new NotImplementedException();
        }

        public async Task StopDocumenting()
        {
            lock (SyncObject)
            {
                try
                {
                    if (State == DocumentingState.Idle)
                        return;

                    ScreenCaptureService?.Stop();
                    SystemService.Instance.PlatformSystemService?.StopKeyboardListener();
                    SystemService.Instance.PlatformSystemService?.StopMouseListener();

                    State = DocumentingState.Idle;
                }
                catch (Exception ex)
                {
                    State = DocumentingState.Errored;
                    throw;
                }
            }
        }

        private void SaveScreenshotLocally(byte[] screenshot, MouseAction action)
        {
            string filename = $"screenshot_{action.EndTimestamp}.png";
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "PaperFy", DateTime.Now.ToString("yyyy-MM-dd"), "screenshots", filename);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, screenshot);
        }

        private bool IsValidMousePoint(int x, int y)
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime classicDesktopStyleApplicationLifetime)
            {
                foreach (Window window in classicDesktopStyleApplicationLifetime.Windows)
                {
                    if (window.WindowState != WindowState.Minimized && window.IsVisible)
                    {
                        Rectangle rectangle = new Rectangle(window.Position.X, window.Position.Y, (int)window.Bounds.Width, (int)window.Bounds.Height);
                        if (rectangle.Contains(new global::PaperFy.Shared.Windows.Models.Point(x, y)))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private long GetLastActionTimestamp()
        {
            return StartTimestamp;
        }

        private void OnKeyboardCaptureEvent(KeyboardCaptureEvent args)
        {
            if (State != DocumentingState.Recording)
            {
                return;
            }
            KeyboardCombinationAction keyboardCombinationAction;
            if (args.KeyboardEvent.IsCombination)
            {
                keyboardCombinationAction = new KeyboardCombinationAction(args.KeyboardEvent, GetLastActionTimestamp(), LastMouseEvent.Duplicate<global::PaperFy.Shared.Windows.Models.Point>());
                //if (scribeAction != null && scribeAction.Kind == ActionKind.KeyboardCombinationAction && scribeAction is KeyboardCombinationAction keyboardCombinationAction2 && keyboardCombinationAction2.IsMergeable(keyboardCombinationAction))
                //{
                //    keyboardCombinationAction2.CombineWith(keyboardCombinationAction);
                //    return;
                //}
                //if (keyboardCombinationAction.HasNonShiftModifiers)
                //{
                //    goto IL_00ef;
                //}
                //if (args.KeyboardEvent.IsEnter)
                //{
                //    if (scribeAction2 != null && scribeAction2.Kind == ActionKind.KeyboardSequenceAction)
                //    {
                //        goto IL_00ef;
                //    }
                //}
            }
            return;
        }

        private (byte[], float, global::PaperFy.Shared.Windows.Models.Point) GetImmediateScreenshotInformation(global::PaperFy.Shared.Windows.Models.Point position, long timestamp)
        {
            global::PaperFy.Shared.Windows.Models.Point point = new global::PaperFy.Shared.Windows.Models.Point(position.X, position.Y);
            byte[] item = ScreenCaptureService?.GetImmediateScreenshot(point, timestamp, Settings.Instance.DontIncludeTaskBar);
            Screen screen = ScreenCaptureService?.GetScreen(point);
            return (item, screen?.Scaling ?? 1f, new global::PaperFy.Shared.Windows.Models.Point((point.X - screen?.Bounds.X).GetValueOrDefault(), (point.Y - screen?.Bounds.Y).GetValueOrDefault()));
        }

        private void OnMouseCaptureEvent(MouseCaptureEvent args)
        {
            if (State != DocumentingState.Recording)
            {
                return;
            }
            if (!IsValidMousePoint(args.MouseEvent.X, args.MouseEvent.Y))
            {
                return;
            }
            byte[] array = null;
            float num = 0f;
            global::PaperFy.Shared.Windows.Models.Point point = default(global::PaperFy.Shared.Windows.Models.Point);

            LastMouseEvent = args.MouseEvent;
            new global::PaperFy.Shared.Windows.Models.Point(LastMouseEvent.X, LastMouseEvent.Y);
            (byte[], float, global::PaperFy.Shared.Windows.Models.Point) immediateScreenshotInformation = GetImmediateScreenshotInformation(new global::PaperFy.Shared.Windows.Models.Point(LastMouseEvent.X, LastMouseEvent.Y), LastMouseEvent.Timestamp);
            array = immediateScreenshotInformation.Item1;
            num = immediateScreenshotInformation.Item2;
            point = immediateScreenshotInformation.Item3;
            MouseAction mouseAction = new MouseAction(
                null,
                _previousEndTimestamp > 0 ? _previousEndTimestamp : StartTimestamp,
                args.MouseEvent.Timestamp,
                point,
                args.MouseEvent.Button,
                1u,
                num,
                new Screenshot(null, ScreenshotUploadStatus.Waiting),
                args.MouseEvent.ApplicationName,
                args.MouseEvent.ApplicationBundle
            );

            mouseAction.TargetText = ControlCaptureService?.GetLabelAtPosition(new global::PaperFy.Shared.Windows.Models.Point(args.MouseEvent.X, args.MouseEvent.Y));

            if (State != DocumentingState.Recording) return;
            if (!IsValidMousePoint(args.MouseEvent.X, args.MouseEvent.Y)) return;

            LastMouseEvent = args.MouseEvent;
            var clickPoint = new Point(LastMouseEvent.X, LastMouseEvent.Y);

            var (screenshot, _, _) = GetImmediateScreenshotInformation(clickPoint, LastMouseEvent.Timestamp);

            ApplicationManager.IimageProcessor?.AddImage(screenshot, clickPoint);
            //SaveScreenshotLocally(array, mouseAction);
        }
    }
}
