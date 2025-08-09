using Interop.UIAutomationClient;
using PaperFy.Shared.Interface;
using PaperFy.Shared.Windows.Models;
using System.Runtime.InteropServices;
using System.Text;

namespace PaperFy.Shared.Windows.Services
{
    public class WindowsControlCaptureService : IControlCaptureService, IDisposable
    {
        private readonly IUIAutomation _automation;

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern IntPtr ChildWindowFromPointEx(IntPtr hwnd, POINT pt, uint flags);

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        private const uint CWP_SKIPINVISIBLE = 0x0001;
        private const uint CWP_SKIPDISABLED = 0x0002;
        private const uint CWP_SKIPTRANSPARENT = 0x0004;

        public WindowsControlCaptureService()
        {
            try
            {
                _automation = new CUIAutomation();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UI Automation failed to initialize: {ex.Message}");
                _automation = null;
            }
        }

        public string GetLabelAtPosition(Point point)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Getting label at position: ({point.X}, {point.Y})");

                string result = TryUIAutomation(point);
                if (!string.IsNullOrEmpty(result))
                {
                    System.Diagnostics.Debug.WriteLine($"UI Automation result: {result}");
                    return result;
                }

                result = TryWin32Approach(point);
                if (!string.IsNullOrEmpty(result))
                {
                    System.Diagnostics.Debug.WriteLine($"Win32 result: {result}");
                    return result;
                }

                System.Diagnostics.Debug.WriteLine("No text found, returning coordinate");
                return $"Click at ({point.X}, {point.Y})";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in GetLabelAtPosition: {ex.Message}");
                return $"Click at ({point.X}, {point.Y})";
            }
        }

        private string TryUIAutomation(Point point)
        {
            if (_automation == null)
                return string.Empty;

            try
            {
                var element = _automation.ElementFromPoint(new tagPOINT { x = point.X, y = point.Y });
                if (element == null)
                    return string.Empty;

                var name = element.CurrentName?.Trim();
                if (!string.IsNullOrEmpty(name) && name != "Desktop")
                {
                    System.Diagnostics.Debug.WriteLine($"Found element name: {name}");
                    return IControlCaptureService.GetTruncatedLabel(name);
                }

                var controlType = element.CurrentLocalizedControlType?.Trim();
                if (!string.IsNullOrEmpty(controlType))
                {
                    System.Diagnostics.Debug.WriteLine($"Found control type: {controlType}");
                    return IControlCaptureService.GetTruncatedLabel(controlType);
                }

                var className = element.CurrentClassName?.Trim();
                if (!string.IsNullOrEmpty(className))
                {
                    System.Diagnostics.Debug.WriteLine($"Found class name: {className}");
                    return IControlCaptureService.GetTruncatedLabel(GetFriendlyControlName(className));
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UI Automation error: {ex.Message}");
                return string.Empty;
            }
        }

        private string TryWin32Approach(Point point)
        {
            try
            {
                var windowHandle = WindowFromPoint(new POINT { X = point.X, Y = point.Y });
                if (windowHandle == IntPtr.Zero)
                    return string.Empty;

                System.Diagnostics.Debug.WriteLine($"Found window handle: {windowHandle}");

                var clientPoint = new POINT { X = point.X, Y = point.Y };
                ScreenToClient(windowHandle, ref clientPoint);

                var childHandle = ChildWindowFromPointEx(windowHandle, clientPoint,
                    CWP_SKIPINVISIBLE | CWP_SKIPDISABLED | CWP_SKIPTRANSPARENT);

                if (childHandle != IntPtr.Zero && childHandle != windowHandle)
                {
                    System.Diagnostics.Debug.WriteLine($"Found child window: {childHandle}");
                    var childText = GetWindowText(childHandle);
                    if (!string.IsNullOrEmpty(childText))
                        return IControlCaptureService.GetTruncatedLabel(childText);

                    var childClassName = GetWindowClassName(childHandle);
                    if (!string.IsNullOrEmpty(childClassName))
                        return IControlCaptureService.GetTruncatedLabel(GetFriendlyControlName(childClassName));
                }

                var windowText = GetWindowText(windowHandle);
                if (!string.IsNullOrEmpty(windowText))
                    return IControlCaptureService.GetTruncatedLabel(windowText);

                var className = GetWindowClassName(windowHandle);
                if (!string.IsNullOrEmpty(className))
                    return IControlCaptureService.GetTruncatedLabel(GetFriendlyControlName(className));

                return string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Win32 approach error: {ex.Message}");
                return string.Empty;
            }
        }

        public bool IsPasswordAtPosition(Point point)
        {
            try
            {
                if (_automation == null)
                    return false;

                var element = _automation.ElementFromPoint(new tagPOINT { x = point.X, y = point.Y });
                return element?.CurrentIsPassword != 0;
            }
            catch
            {
                return false;
            }
        }

        private string GetWindowText(IntPtr hWnd)
        {
            try
            {
                if (!IsWindowVisible(hWnd))
                    return string.Empty;

                var sb = new StringBuilder(512);
                var length = GetWindowText(hWnd, sb, sb.Capacity);
                var text = length > 0 ? sb.ToString().Trim() : string.Empty;

                if (!string.IsNullOrEmpty(text))
                    System.Diagnostics.Debug.WriteLine($"Window text: '{text}'");

                return text;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetWindowText error: {ex.Message}");
                return string.Empty;
            }
        }

        private string GetWindowClassName(IntPtr hWnd)
        {
            try
            {
                var sb = new StringBuilder(256);
                var length = GetClassName(hWnd, sb, sb.Capacity);
                var className = length > 0 ? sb.ToString() : string.Empty;

                if (!string.IsNullOrEmpty(className))
                    System.Diagnostics.Debug.WriteLine($"Class name: '{className}'");

                return className;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetClassName error: {ex.Message}");
                return string.Empty;
            }
        }

        private string GetFriendlyControlName(string className)
        {
            var friendly = className.ToLower() switch
            {
                "button" => "Button",
                "edit" => "Text Field",
                "static" => "Label",
                "listbox" => "List Box",
                "combobox" => "Dropdown",
                "scrollbar" => "Scroll Bar",
                "msctls_trackbar32" => "Slider",
                "tooltips_class32" => "Tooltip",
                "richedit20w" => "Rich Text Box",
                var x when x.Contains("button") => "Button",
                var x when x.Contains("edit") => "Text Field",
                var x when x.Contains("list") => "List",
                _ => className
            };

            System.Diagnostics.Debug.WriteLine($"Friendly name for '{className}': '{friendly}'");
            return friendly;
        }

        public void Dispose()
        {
            if (_automation != null)
            {
                try
                {
                    Marshal.ReleaseComObject(_automation);
                }
                catch { }
            }
        }
    }
}