using Avalonia.Input;
using PaperFy.Shared.Capture;
using PaperFy.Shared.Windows.Events;
using PaperFy.Shared.Windows.Services;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace PaperFy.Shared.Windows.Models
{
    internal static class KeyboardUtilities
    {
        public delegate nint LowLevelEventProc(int nCode, nint wParam, nint lParam);

        private static readonly object SyncObject = new object();

        private static Dictionary<Keys, Key> WindowsToAvalonia = new Dictionary<Keys, Key>
    {
        {
            Keys.None,
            Key.None
        },
        {
            Keys.Cancel,
            Key.Cancel
        },
        {
            Keys.Back,
            Key.Back
        },
        {
            Keys.Tab,
            Key.Tab
        },
        {
            Keys.LineFeed,
            Key.LineFeed
        },
        {
            Keys.Clear,
            Key.Clear
        },
        {
            Keys.Return,
            Key.Return
        },
        {
            Keys.Pause,
            Key.Pause
        },
        {
            Keys.Capital,
            Key.CapsLock
        },
        {
            Keys.KanaMode,
            Key.HangulMode
        },
        {
            Keys.JunjaMode,
            Key.JunjaMode
        },
        {
            Keys.FinalMode,
            Key.FinalMode
        },
        {
            Keys.HanjaMode,
            Key.KanjiMode
        },
        {
            Keys.Escape,
            Key.Escape
        },
        {
            Keys.IMEConvert,
            Key.ImeConvert
        },
        {
            Keys.IMENonconvert,
            Key.ImeNonConvert
        },
        {
            Keys.IMEAccept,
            Key.ImeAccept
        },
        {
            Keys.IMEModeChange,
            Key.ImeModeChange
        },
        {
            Keys.Space,
            Key.Space
        },
        {
            Keys.Prior,
            Key.PageUp
        },
        {
            Keys.Next,
            Key.PageDown
        },
        {
            Keys.End,
            Key.End
        },
        {
            Keys.Home,
            Key.Home
        },
        {
            Keys.Left,
            Key.Left
        },
        {
            Keys.Up,
            Key.Up
        },
        {
            Keys.Right,
            Key.Right
        },
        {
            Keys.Down,
            Key.Down
        },
        {
            Keys.Select,
            Key.Select
        },
        {
            Keys.Print,
            Key.Print
        },
        {
            Keys.Execute,
            Key.Execute
        },
        {
            Keys.Snapshot,
            Key.Snapshot
        },
        {
            Keys.Insert,
            Key.Insert
        },
        {
            Keys.Delete,
            Key.Delete
        },
        {
            Keys.Help,
            Key.Help
        },
        {
            Keys.D0,
            Key.D0
        },
        {
            Keys.D1,
            Key.D1
        },
        {
            Keys.D2,
            Key.D2
        },
        {
            Keys.D3,
            Key.D3
        },
        {
            Keys.D4,
            Key.D4
        },
        {
            Keys.D5,
            Key.D5
        },
        {
            Keys.D6,
            Key.D6
        },
        {
            Keys.D7,
            Key.D7
        },
        {
            Keys.D8,
            Key.D8
        },
        {
            Keys.D9,
            Key.D9
        },
        {
            Keys.A,
            Key.A
        },
        {
            Keys.B,
            Key.B
        },
        {
            Keys.C,
            Key.C
        },
        {
            Keys.D,
            Key.D
        },
        {
            Keys.E,
            Key.E
        },
        {
            Keys.F,
            Key.F
        },
        {
            Keys.G,
            Key.G
        },
        {
            Keys.H,
            Key.H
        },
        {
            Keys.I,
            Key.I
        },
        {
            Keys.J,
            Key.J
        },
        {
            Keys.K,
            Key.K
        },
        {
            Keys.L,
            Key.L
        },
        {
            Keys.M,
            Key.M
        },
        {
            Keys.N,
            Key.N
        },
        {
            Keys.O,
            Key.O
        },
        {
            Keys.P,
            Key.P
        },
        {
            Keys.Q,
            Key.Q
        },
        {
            Keys.R,
            Key.R
        },
        {
            Keys.S,
            Key.S
        },
        {
            Keys.T,
            Key.T
        },
        {
            Keys.U,
            Key.U
        },
        {
            Keys.V,
            Key.V
        },
        {
            Keys.W,
            Key.W
        },
        {
            Keys.X,
            Key.X
        },
        {
            Keys.Y,
            Key.Y
        },
        {
            Keys.Z,
            Key.Z
        },
        {
            Keys.LWin,
            Key.LWin
        },
        {
            Keys.RWin,
            Key.RWin
        },
        {
            Keys.Apps,
            Key.Apps
        },
        {
            Keys.Sleep,
            Key.Sleep
        },
        {
            Keys.NumPad0,
            Key.NumPad0
        },
        {
            Keys.NumPad1,
            Key.NumPad1
        },
        {
            Keys.NumPad2,
            Key.NumPad2
        },
        {
            Keys.NumPad3,
            Key.NumPad3
        },
        {
            Keys.NumPad4,
            Key.NumPad4
        },
        {
            Keys.NumPad5,
            Key.NumPad5
        },
        {
            Keys.NumPad6,
            Key.NumPad6
        },
        {
            Keys.NumPad7,
            Key.NumPad7
        },
        {
            Keys.NumPad8,
            Key.NumPad8
        },
        {
            Keys.NumPad9,
            Key.NumPad9
        },
        {
            Keys.Multiply,
            Key.Multiply
        },
        {
            Keys.Add,
            Key.Add
        },
        {
            Keys.Separator,
            Key.Separator
        },
        {
            Keys.Subtract,
            Key.Subtract
        },
        {
            Keys.Decimal,
            Key.Decimal
        },
        {
            Keys.Divide,
            Key.Divide
        },
        {
            Keys.F1,
            Key.F1
        },
        {
            Keys.F2,
            Key.F2
        },
        {
            Keys.F3,
            Key.F3
        },
        {
            Keys.F4,
            Key.F4
        },
        {
            Keys.F5,
            Key.F5
        },
        {
            Keys.F6,
            Key.F6
        },
        {
            Keys.F7,
            Key.F7
        },
        {
            Keys.F8,
            Key.F8
        },
        {
            Keys.F9,
            Key.F9
        },
        {
            Keys.F10,
            Key.F10
        },
        {
            Keys.F11,
            Key.F11
        },
        {
            Keys.F12,
            Key.F12
        },
        {
            Keys.F13,
            Key.F13
        },
        {
            Keys.F14,
            Key.F14
        },
        {
            Keys.F15,
            Key.F15
        },
        {
            Keys.F16,
            Key.F16
        },
        {
            Keys.F17,
            Key.F17
        },
        {
            Keys.F18,
            Key.F18
        },
        {
            Keys.F19,
            Key.F19
        },
        {
            Keys.F20,
            Key.F20
        },
        {
            Keys.F21,
            Key.F21
        },
        {
            Keys.F22,
            Key.F22
        },
        {
            Keys.F23,
            Key.F23
        },
        {
            Keys.F24,
            Key.F24
        },
        {
            Keys.NumLock,
            Key.NumLock
        },
        {
            Keys.Scroll,
            Key.Scroll
        },
        {
            Keys.LShiftKey,
            Key.LeftShift
        },
        {
            Keys.RShiftKey,
            Key.RightShift
        },
        {
            Keys.LControlKey,
            Key.LeftCtrl
        },
        {
            Keys.RControlKey,
            Key.RightCtrl
        },
        {
            Keys.LMenu,
            Key.LeftAlt
        },
        {
            Keys.RMenu,
            Key.RightAlt
        },
        {
            Keys.BrowserBack,
            Key.BrowserBack
        },
        {
            Keys.BrowserForward,
            Key.BrowserForward
        },
        {
            Keys.BrowserRefresh,
            Key.BrowserRefresh
        },
        {
            Keys.BrowserStop,
            Key.BrowserStop
        },
        {
            Keys.BrowserSearch,
            Key.BrowserSearch
        },
        {
            Keys.BrowserFavorites,
            Key.BrowserFavorites
        },
        {
            Keys.BrowserHome,
            Key.BrowserHome
        },
        {
            Keys.VolumeMute,
            Key.VolumeMute
        },
        {
            Keys.VolumeDown,
            Key.VolumeDown
        },
        {
            Keys.VolumeUp,
            Key.VolumeDown
        },
        {
            Keys.MediaNextTrack,
            Key.MediaNextTrack
        },
        {
            Keys.MediaPreviousTrack,
            Key.MediaPreviousTrack
        },
        {
            Keys.MediaStop,
            Key.MediaStop
        },
        {
            Keys.MediaPlayPause,
            Key.MediaPlayPause
        },
        {
            Keys.LaunchMail,
            Key.LaunchMail
        },
        {
            Keys.SelectMedia,
            Key.SelectMedia
        },
        {
            Keys.LaunchApplication1,
            Key.LaunchApplication1
        },
        {
            Keys.LaunchApplication2,
            Key.LaunchApplication2
        },
        {
            Keys.OemSemicolon,
            Key.OemSemicolon
        },
        {
            Keys.Oemplus,
            Key.OemPlus
        },
        {
            Keys.Oemcomma,
            Key.OemComma
        },
        {
            Keys.OemMinus,
            Key.OemMinus
        },
        {
            Keys.OemPeriod,
            Key.OemPeriod
        },
        {
            Keys.OemQuestion,
            Key.OemQuestion
        },
        {
            Keys.Oemtilde,
            Key.OemTilde
        },
        {
            Keys.OemOpenBrackets,
            Key.OemOpenBrackets
        },
        {
            Keys.OemPipe,
            Key.OemPipe
        },
        {
            Keys.OemCloseBrackets,
            Key.OemCloseBrackets
        },
        {
            Keys.OemQuotes,
            Key.OemQuotes
        },
        {
            Keys.Oem8,
            Key.Oem8
        },
        {
            Keys.OemBackslash,
            Key.OemBackslash
        },
        {
            Keys.Attn,
            Key.DbeNoRoman
        },
        {
            Keys.Crsel,
            Key.CrSel
        },
        {
            Keys.Exsel,
            Key.ExSel
        },
        {
            Keys.EraseEof,
            Key.EraseEof
        },
        {
            Keys.Play,
            Key.Play
        },
        {
            Keys.Zoom,
            Key.DbeNoCodeInput
        },
        {
            Keys.NoName,
            Key.NoName
        },
        {
            Keys.Pa1,
            Key.DbeEnterDialogConversionMode
        },
        {
            Keys.OemClear,
            Key.OemClear
        }
    };

        private static Keys[] IgnoredSpecialKeys = new Keys[7]
        {
        Keys.Alt,
        Keys.LMenu,
        Keys.RMenu,
        Keys.LControlKey,
        Keys.RControlKey,
        Keys.LShiftKey,
        Keys.RShiftKey
        };

        private static Keys[] SpecialKeys = new Keys[24]
        {
        Keys.Tab,
        Keys.Escape,
        Keys.Return,
        Keys.Space,
        Keys.Back,
        Keys.Delete,
        Keys.Home,
        Keys.End,
        Keys.Insert,
        Keys.Pause,
        Keys.Prior,
        Keys.Next,
        Keys.Snapshot,
        Keys.Scroll,
        Keys.Left,
        Keys.Up,
        Keys.Right,
        Keys.Down,
        Keys.LMenu,
        Keys.RMenu,
        Keys.LControlKey,
        Keys.RControlKey,
        Keys.LShiftKey,
        Keys.RShiftKey
        };

        private static nint NativeHook = IntPtr.Zero;

        private const int VK_CONTROL = 17;

        private const int VK_MENU = 18;

        private const int VK_SHIFT = 16;

        private const int VK_LWIN = 91;

        private const int VK_RWIN = 92;

        private const int WH_KEYBOARD_LL = 13;

        private const int WM_KEYDOWN = 256;

        private const int WM_SYSKEYDOWN = 260;

        internal static bool IsListening => NativeHook != IntPtr.Zero;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern nint SetWindowsHookEx(int idHook, LowLevelEventProc lpfn, nint hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(nint hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern nint GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern int MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int ToUnicode(uint virtualKeyCode, uint scanCode, byte[] keyboardState, [Out][MarshalAs(UnmanagedType.LPWStr)] StringBuilder receivingBuffer, int bufferSize, uint flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern short GetKeyState(int keyCode);

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
                    NativeHook = SetWindowsHookEx(13, KeyboardHookCallback, GetModuleHandle(mainModule.ModuleName), 0u);
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

        private static int? GetSpecialKey(int virtualKey)
        {
            if ((virtualKey >= 166 && virtualKey <= 183) || (virtualKey >= 91 && virtualKey <= 95) || (virtualKey >= 112 && virtualKey <= 135) || SpecialKeys.Contains((Keys)virtualKey))
            {
                return virtualKey;
            }
            return null;
        }

        private static nint KeyboardHookCallback(int nCode, nint wParam, nint lParam)
        {
            if (nCode >= 0 && (wParam == 256 || wParam == 260))
            {
                int num = Marshal.ReadInt32(lParam);
                bool flag = (GetKeyState(18) & 0x8000) != 0;
                bool flag2 = (GetKeyState(17) & 0x8000) != 0;
                bool flag3 = (GetKeyState(16) & 0x8000) != 0;
                bool flag4 = (GetKeyState(91) & 0x8000) != 0 || (GetKeyState(92) & 0x8000) != 0;
                byte[] array = new byte[256];
                if (GetKeyboardState(array))
                {
                    StringBuilder stringBuilder = new StringBuilder(array.Length);
                    uint scanCode = (uint)((MapVirtualKey((uint)num, 0u) << 16) | 1);
                    int num2 = ToUnicode((uint)num, scanCode, array, stringBuilder, stringBuilder.Capacity, 0u);
                    bool value = !flag && !flag2 && !flag3 && !flag4;
                    string text = ((num2 > 0) ? stringBuilder.ToString() : string.Empty);
                    int? specialKey = GetSpecialKey(num);
                    int? num3 = specialKey;
                    if (num3.HasValue)
                    {
                        num3 = (int)VirtualKeyCodeToAvaloniaKey(num3.Value);
                    }
                    KeyboardEvent keyboardEvent = new KeyboardEvent(new KeyboardEventModifiers(flag, flag2, flag3, flag4), text, (int)VirtualKeyCodeToAvaloniaKey(num), num3, SystemService.Instance.CurrentTimestamp);
                    
                    if (specialKey.HasValue && IgnoredSpecialKeys.Contains((Keys)specialKey.Value))
                    {

                    }
                    else
                    {
                        EventAggregator.Instance.Publish(new KeyboardCaptureEvent(keyboardEvent));
                    }
                }
            }
            return CallNextHookEx(NativeHook, nCode, wParam, lParam);
        }

        private static Key VirtualKeyCodeToAvaloniaKey(int virtualKeyCode)
        {
            if (WindowsToAvalonia.TryGetValue((Keys)virtualKeyCode, out var value))
            {
                return value;
            }
            return Key.None;
        }
    }
}


