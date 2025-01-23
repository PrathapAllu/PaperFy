using PaperFy.Shared.Windows.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PaperFy.Shared.Windows.Utilities
{
    internal static class DisplayUtilities
    {
        public enum ScreenOrientation
        {
            Angle0,
            Angle90,
            Angle180,
            Angle270
        }

        internal struct DEVMODE
        {
            private const int CCHDEVICENAME = 32;

            private const int CCHFORMNAME = 32;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;

            public short dmSpecVersion;

            public short dmDriverVersion;

            public short dmSize;

            public short dmDriverExtra;

            public int dmFields;

            public int dmPositionX;

            public int dmPositionY;

            public ScreenOrientation dmDisplayOrientation;

            public int dmDisplayFixedOutput;

            public short dmColor;

            public short dmDuplex;

            public short dmYResolution;

            public short dmTTOption;

            public short dmCollate;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;

            public short dmLogPixels;

            public int dmBitsPerPel;

            public int dmPelsWidth;

            public int dmPelsHeight;

            public int dmDisplayFlags;

            public int dmDisplayFrequency;

            public int dmICMMethod;

            public int dmICMIntent;

            public int dmMediaType;

            public int dmDitherType;

            public int dmReserved1;

            public int dmReserved2;

            public int dmPanningWidth;

            public int dmPanningHeight;
        }

        internal enum MONITOR_DPI_TYPE
        {
            MDT_Effective_DPI = 0,
            MDT_Angular_DPI = 1,
            MDT_Raw_DPI = 2,
            MDT_Default = 0
        }

        public struct RECT
        {
            public int Left;

            public int Top;

            public int Right;

            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(Models.Rectangle rectangle)
            {
                Left = rectangle.Left;
                Top = rectangle.Top;
                Right = rectangle.Right;
                Bottom = rectangle.Bottom;
            }
        }

        private const uint WDA_NONE = 0u;

        private const uint WDA_MONITOR = 1u;

        [DllImport("Shcore.dll")]
        private static extern int GetDpiForMonitor(nint hmonitor, MONITOR_DPI_TYPE dpiType, out uint dpiX, out uint dpiY);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(nint hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern nint MonitorFromRect([In] ref RECT lprc, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(nint hwnd, nint hdcBlt, int nFlags);

        [DllImport("user32.dll")]
        private static extern bool SetWindowDisplayAffinity(nint hWnd, uint dwAffinity);

        internal static byte[] CaptureWindow(nint windowHandle)
        {
            Models.Rectangle? windowRectangle = GetWindowRectangle(windowHandle);
            if (windowRectangle.HasValue)
            {
                using (Bitmap bitmap = new Bitmap(windowRectangle.Value.Width, windowRectangle.Value.Height, PixelFormat.Format32bppArgb))
                {
                    using Graphics graphics = Graphics.FromImage(bitmap);
                    nint hdc = graphics.GetHdc();
                    PrintWindow(windowHandle, hdc, 0);
                    using MemoryStream memoryStream = new MemoryStream();
                    bitmap.Save(memoryStream, ImageFormat.Png);
                    return memoryStream.ToArray();
                }
            }
            return null;
        }

        internal static float GetScreenScaling(Screen screen)
        {
            uint dpiX = 0u;
            RECT lprc = new RECT(screen.Bounds);
            GetDpiForMonitor(MonitorFromRect(ref lprc, 0u), MONITOR_DPI_TYPE.MDT_Effective_DPI, out dpiX, out var _);
            return (float)dpiX / 96f;
        }

        internal static Models.Rectangle? GetWindowRectangle(nint windowHandle)
        {
            if (GetWindowRect(windowHandle, out var lpRect))
            {
                return new Models.Rectangle(lpRect.Left, lpRect.Top, lpRect.Right - lpRect.Left, lpRect.Bottom - lpRect.Top);
            }
            return null;
        }

        internal static void HideWindowFromScreenCapture(nint windowHandle)
        {
            SetWindowDisplayAffinity(windowHandle, 1u);
        }
    }
}


