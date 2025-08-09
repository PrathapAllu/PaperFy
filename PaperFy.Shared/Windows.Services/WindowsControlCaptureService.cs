using PaperFy.Shared.Interface;
using PaperFy.Shared.Windows.Models;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace PaperFy.Shared.Windows.Services
{
    public class WindowsControlCaptureService : IControlCaptureService
    {
        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public string GetLabelAtPosition(Point point)
        {
            try
            {
                var windowPoint = new POINT { x = point.X, y = point.Y };
                IntPtr hWnd = WindowFromPoint(windowPoint);

                if (hWnd == IntPtr.Zero)
                    return "Unknown location";

                // Get application info
                var appInfo = GetApplicationInfo(hWnd);
                var controlInfo = GetControlInfo(hWnd);
                var contextInfo = GetContextualInfo(hWnd, point);

                // Build descriptive text
                return BuildClickDescription(appInfo, controlInfo, contextInfo);
            }
            catch (Exception ex)
            {
                return "Click captured";
            }
        }

        public bool IsPasswordAtPosition(Point point)
        {
            try
            {
                var windowPoint = new POINT { x = point.X, y = point.Y };
                IntPtr hWnd = WindowFromPoint(windowPoint);

                if (hWnd == IntPtr.Zero)
                    return false;

                var className = new StringBuilder(256);
                GetClassName(hWnd, className, className.Capacity);

                // Check for password field indicators
                return className.ToString().ToLower().Contains("edit") &&
                       IsPasswordControl(hWnd);
            }
            catch
            {
                return false;
            }
        }

        private ApplicationInfo GetApplicationInfo(IntPtr hWnd)
        {
            try
            {
                GetWindowThreadProcessId(hWnd, out uint processId);
                var process = Process.GetProcessById((int)processId);

                var windowTitle = new StringBuilder(256);
                GetWindowText(hWnd, windowTitle, windowTitle.Capacity);

                return new ApplicationInfo
                {
                    ProcessName = process.ProcessName,
                    WindowTitle = windowTitle.ToString(),
                    ExecutablePath = process.MainModule?.FileName ?? "",
                    ApplicationName = process.MainModule?.FileVersionInfo?.ProductName ?? process.ProcessName
                };
            }
            catch
            {
                return new ApplicationInfo { ProcessName = "Unknown", ApplicationName = "Unknown Application" };
            }
        }

        private ControlInfo GetControlInfo(IntPtr hWnd)
        {
            var className = new StringBuilder(256);
            var windowText = new StringBuilder(256);

            GetClassName(hWnd, className, className.Capacity);
            GetWindowText(hWnd, windowText, windowText.Capacity);

            return new ControlInfo
            {
                ClassName = className.ToString(),
                Text = windowText.ToString(),
                ControlType = DetermineControlType(className.ToString(), windowText.ToString())
            };
        }

        private ContextualInfo GetContextualInfo(IntPtr hWnd, Point clickPoint)
        {
            var context = new ContextualInfo();

            // Check if it's a file explorer window
            if (IsFileExplorer(hWnd))
            {
                context.IsFileExplorer = true;
                context.FilePath = GetSelectedFilePath(hWnd);
            }

            // Check for browser
            else if (IsBrowser(hWnd))
            {
                context.IsBrowser = true;
                context.Url = GetBrowserUrl(hWnd);
            }

            // Check for common applications
            else if (IsVisualStudio(hWnd))
            {
                context.IsIDE = true;
                context.FileName = GetActiveFileName(hWnd);
            }

            return context;
        }

        private string BuildClickDescription(ApplicationInfo appInfo, ControlInfo controlInfo, ContextualInfo contextInfo)
        {
            var description = new StringBuilder();

            // Start with action
            description.Append("Click on ");

            // Add specific control information if available
            if (!string.IsNullOrEmpty(controlInfo.Text) && controlInfo.Text.Trim().Length > 0)
            {
                description.Append($"\"{controlInfo.Text.Trim()}\" ");
            }
            else if (controlInfo.ControlType != "Unknown")
            {
                description.Append($"{controlInfo.ControlType} ");
            }

            // Add contextual information
            if (contextInfo.IsFileExplorer && !string.IsNullOrEmpty(contextInfo.FilePath))
            {
                description.Append($"in File Explorer ({System.IO.Path.GetFileName(contextInfo.FilePath)}) ");
            }
            else if (contextInfo.IsBrowser && !string.IsNullOrEmpty(contextInfo.Url))
            {
                var domain = GetDomainFromUrl(contextInfo.Url);
                description.Append($"in browser ({domain}) ");
            }
            else if (contextInfo.IsIDE && !string.IsNullOrEmpty(contextInfo.FileName))
            {
                description.Append($"in {appInfo.ApplicationName} ({System.IO.Path.GetFileName(contextInfo.FileName)}) ");
            }
            else
            {
                description.Append($"in {appInfo.ApplicationName} ");
            }

            // Add window context if different from application
            if (!string.IsNullOrEmpty(appInfo.WindowTitle) &&
                appInfo.WindowTitle != appInfo.ApplicationName &&
                !description.ToString().Contains(appInfo.WindowTitle))
            {
                description.Append($"- {appInfo.WindowTitle}");
            }

            return IControlCaptureService.GetTruncatedLabel(description.ToString().Trim());
        }

        private string DetermineControlType(string className, string text)
        {
            className = className.ToLower();

            if (className.Contains("button")) return "Button";
            if (className.Contains("edit")) return "Text Box";
            if (className.Contains("static")) return "Label";
            if (className.Contains("listbox")) return "List";
            if (className.Contains("combobox")) return "Dropdown";
            if (className.Contains("scrollbar")) return "Scrollbar";
            if (className.Contains("menu")) return "Menu";
            if (className.Contains("tab")) return "Tab";
            if (className.Contains("tree")) return "Tree View";
            if (className.Contains("toolbar")) return "Toolbar";

            return "Unknown";
        }

        private bool IsFileExplorer(IntPtr hWnd)
        {
            try
            {
                GetWindowThreadProcessId(hWnd, out uint processId);
                var process = Process.GetProcessById((int)processId);
                return process.ProcessName.Equals("explorer", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        private bool IsBrowser(IntPtr hWnd)
        {
            try
            {
                GetWindowThreadProcessId(hWnd, out uint processId);
                var process = Process.GetProcessById((int)processId);
                var processName = process.ProcessName.ToLower();

                return processName.Contains("chrome") ||
                       processName.Contains("firefox") ||
                       processName.Contains("edge") ||
                       processName.Contains("iexplore") ||
                       processName.Contains("opera");
            }
            catch
            {
                return false;
            }
        }

        private bool IsVisualStudio(IntPtr hWnd)
        {
            try
            {
                GetWindowThreadProcessId(hWnd, out uint processId);
                var process = Process.GetProcessById((int)processId);
                var processName = process.ProcessName.ToLower();

                return processName.Contains("devenv") || processName.Contains("code");
            }
            catch
            {
                return false;
            }
        }

        private string GetSelectedFilePath(IntPtr hWnd)
        {
            // This would require more complex implementation using IShellWindows
            // For now, return a placeholder that could be enhanced
            return "";
        }

        private string GetBrowserUrl(IntPtr hWnd)
        {
            // This would require browser-specific automation
            // For now, return empty - could be enhanced with browser automation
            return "";
        }

        private string GetActiveFileName(IntPtr hWnd)
        {
            // Extract file name from window title for IDE applications
            try
            {
                var windowTitle = new StringBuilder(256);
                GetWindowText(hWnd, windowTitle, windowTitle.Capacity);
                var title = windowTitle.ToString();

                // Common IDE patterns
                var patterns = new[] { " - ", " — ", " | " };
                foreach (var pattern in patterns)
                {
                    if (title.Contains(pattern))
                    {
                        var parts = title.Split(new[] { pattern }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length > 0)
                        {
                            // Usually the file name is the first part
                            return parts[0].Trim();
                        }
                    }
                }

                return "";
            }
            catch
            {
                return "";
            }
        }

        private string GetDomainFromUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                return uri.Host;
            }
            catch
            {
                return "web page";
            }
        }

        private bool IsPasswordControl(IntPtr hWnd)
        {
            // This would require checking window styles and properties
            // For now, return false - could be enhanced with Windows API calls
            return false;
        }

        private class ApplicationInfo
        {
            public string ProcessName { get; set; } = "";
            public string ApplicationName { get; set; } = "";
            public string WindowTitle { get; set; } = "";
            public string ExecutablePath { get; set; } = "";
        }

        private class ControlInfo
        {
            public string ClassName { get; set; } = "";
            public string Text { get; set; } = "";
            public string ControlType { get; set; } = "";
        }

        private class ContextualInfo
        {
            public bool IsFileExplorer { get; set; }
            public bool IsBrowser { get; set; }
            public bool IsIDE { get; set; }
            public string FilePath { get; set; } = "";
            public string Url { get; set; } = "";
            public string FileName { get; set; } = "";
        }
    }
}