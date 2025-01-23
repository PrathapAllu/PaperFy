using PaperFy.Shared.Capture;
using PaperFy.Shared.Windows.Events;
using PaperFy.Shared.Windows.Models;
using PaperFy.Shared.Windows.Services;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace PaperFy.Shared.Windows.Utilities
{
    internal static class MouseUtilities
    {
        private struct POINT
        {
            public int x;

            public int y;
        }

        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;

            public uint mouseData;

            public uint flags;

            public uint time;

            public nint dwExtraInfo;
        }

        private static readonly object SyncObject = new object();

        private static nint NativeHook = IntPtr.Zero;

        private const int WH_MOUSE_LL = 14;

        private const int WM_LBUTTONDOWN = 513;

        private const int WM_RBUTTONDOWN = 516;

        private const int WM_MBUTTONDOWN = 519;

        internal static bool IsListening => NativeHook != IntPtr.Zero;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern nint SetWindowsHookEx(int idHook, KeyboardUtilities.LowLevelEventProc lpfn, nint hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(nint hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern nint GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern nint GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(nint hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(nint hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern int GetDoubleClickTime();

        internal static void Listen()
        {
            lock (SyncObject)
            {
                if (IsListening)
                {
                    return;
                }
                using Process process = Process.GetCurrentProcess();
                ProcessModule mainModule = process.MainModule;
                try
                {
                    NativeHook = SetWindowsHookEx(14, MouseHookCallback, GetModuleHandle(mainModule.ModuleName), 0u);
                }
                finally
                {
                    ((IDisposable)mainModule)?.Dispose();
                }
            }
        }

        internal static void Stop()
        {
            lock (SyncObject)
            {
                if (IsListening)
                {
                    UnhookWindowsHookEx(NativeHook);
                    NativeHook = IntPtr.Zero;
                }
            }
        }

        private static (string, string) GetApplicationNameAndBundle(int processID)
        {
            try
            {
                FileVersionInfo fileVersionInfo = Process.GetProcessById(processID)?.MainModule?.FileVersionInfo;
                if (fileVersionInfo != null)
                {
                    return (fileVersionInfo.FileDescription, fileVersionInfo.CompanyName + "__" + fileVersionInfo.OriginalFilename);
                }
            }
            catch (Exception exception)
            {
                
            }
            return (null, null);
        }

        private static nint MouseHookCallback(int nCode, nint wParam, nint lParam)
        {
            if (nCode >= 0 && (wParam == 513 || wParam == 516 || wParam == 519))
            {
                MSLLHOOKSTRUCT mSLLHOOKSTRUCT = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                MouseButton button = wParam switch
                {
                    519 => MouseButton.Middle,
                    516 => MouseButton.Right,
                    _ => MouseButton.Left,
                };
                GetWindowThreadProcessId(GetForegroundWindow(), out var lpdwProcessId);
                (string, string) applicationNameAndBundle = GetApplicationNameAndBundle((int)lpdwProcessId);
                MouseEvent mouseEvent = new MouseEvent(mSLLHOOKSTRUCT.pt.x, mSLLHOOKSTRUCT.pt.y, button, applicationNameAndBundle.Item1, applicationNameAndBundle.Item2, SystemService.Instance.CurrentTimestamp);
                EventAggregator.Instance.Publish(new MouseCaptureEvent(mouseEvent));
            }
            return CallNextHookEx(NativeHook, nCode, wParam, lParam);
        }
    }
}


