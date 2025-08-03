using PaperFy.Shared.Windows.Models;
using System.Runtime.InteropServices;

namespace PaperFy.Shared.Windows.Utilities
{
    internal static class TaskbarUtilities
    {
        public enum TaskbarPosition
        {
            Unknown = -1,
            Left,
            Top,
            Right,
            Bottom
        }

        public struct TaskbarInfo
        {
            public Rectangle Bounds;
            public TaskbarPosition Position;
            public bool IsAutoHide;
            public bool IsVisible;
        }

        private const string TASKBAR_CLASS_NAME = "Shell_TrayWnd";

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static TaskbarInfo GetTaskbarInfo()
        {
            var taskbarInfo = new TaskbarInfo();

            IntPtr taskbarHandle = FindWindow(TASKBAR_CLASS_NAME, null);
            if (taskbarHandle == IntPtr.Zero)
            {
                taskbarInfo.Position = TaskbarPosition.Unknown;
                return taskbarInfo;
            }

            taskbarInfo.IsVisible = IsWindowVisible(taskbarHandle);

            if (GetWindowRect(taskbarHandle, out RECT rect))
            {
                taskbarInfo.Bounds = new Rectangle(rect.Left, rect.Top,
                    rect.Right - rect.Left, rect.Bottom - rect.Top);

                // Determine taskbar position
                var screenBounds = Screen.Screens[0].Bounds; // Primary screen

                if (rect.Top <= screenBounds.Y && rect.Bottom < screenBounds.Bottom)
                    taskbarInfo.Position = TaskbarPosition.Top;
                else if (rect.Bottom >= screenBounds.Bottom && rect.Top > screenBounds.Y)
                    taskbarInfo.Position = TaskbarPosition.Bottom;
                else if (rect.Left <= screenBounds.X && rect.Right < screenBounds.Right)
                    taskbarInfo.Position = TaskbarPosition.Left;
                else if (rect.Right >= screenBounds.Right && rect.Left > screenBounds.X)
                    taskbarInfo.Position = TaskbarPosition.Right;
                else
                    taskbarInfo.Position = TaskbarPosition.Unknown;
            }

            return taskbarInfo;
        }

        public static Rectangle GetWorkingAreaForScreen(Screen screen)
        {
            // Use built-in working area calculation which excludes taskbar
            var bounds = screen.Bounds;
            var workingArea = screen.Native.WorkingArea;

            return new Rectangle(
                workingArea.X,
                workingArea.Y,
                workingArea.Width,
                workingArea.Height
            );
        }

        public static Rectangle CalculateExcludeTaskbarBounds(Screen screen)
        {
            var fullBounds = screen.Bounds;
            var workingArea = GetWorkingAreaForScreen(screen);

            // If working area equals full bounds, taskbar is hidden or auto-hide
            if (workingArea.Width == fullBounds.Width && workingArea.Height == fullBounds.Height)
            {
                return fullBounds;
            }

            // Return the working area (excludes taskbar)
            return workingArea;
        }
    }
}