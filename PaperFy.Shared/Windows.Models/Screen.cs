using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using PaperFy.Shared.Common.Extensions;


namespace PaperFy.Shared.Windows.Models
{
    public class Screen
    {
        private static readonly object SyncObject = new object();

        private static List<Screen> screens = new List<Screen>();

        public Rectangle Bounds => Native.Bounds.ToPaperFy();

        public string Name => $"Screen #{screens.IndexOf(this) + 1}";

        public float Scaling => (float)Native.Scaling;

        internal Avalonia.Platform.Screen Native { get; private set; }

        public static Screen[] Screens
        {
            get
            {
                lock (SyncObject)
                {
                    if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime classicDesktopStyleApplicationLifetime)
                    {
                        Screens screens = classicDesktopStyleApplicationLifetime.MainWindow?.Screens;
                        if (screens != null)
                        {
                            if (Screen.screens.Count != screens.ScreenCount || !Screen.screens.Select((Screen i) => i.Native).SequenceEqual(screens.All))
                            {
                                Screen.screens.Clear();
                                Screen.screens.AddRange(screens.All.Select((Avalonia.Platform.Screen i) => new Screen(i)));
                            }
                            else
                            {
                                for (int j = 0; j < Screen.screens.Count; j++)
                                {
                                    if (Screen.screens[j].Bounds != screens.All[j].Bounds.ToPaperFy())
                                    {
                                        Screen.screens.Clear();
                                        Screen.screens.AddRange(screens.All.Select((Avalonia.Platform.Screen i) => new Screen(i)));
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    return Screen.screens.ToArray();
                }
            }
        }

        public Screen(Avalonia.Platform.Screen screen)
        {
            Native = screen;
        }

        public IReadOnlyDictionary<string, object> GetTelemetryInformation()
        {
            return new Dictionary<string, object>
        {
            {
                "Bounds",
                Native.Bounds.ToString()
            },
            { "DeviceName", Name },
            { "Primary", Native.IsPrimary },
            {
                "WorkingArea",
                Native.WorkingArea.ToString()
            },
            { "dpr", Scaling }
        };
        }

        public static Screen FromPoint(Point point)
        {
            Screen[] array = Screens;
            foreach (Screen screen in array)
            {
                if (screen.Bounds.Contains(point))
                {
                    return screen;
                }
            }
            return null;
        }

        public static IReadOnlyDictionary<string, object>[] GetTelemetryInformationForAllScreens()
        {
            try
            {
                List<IReadOnlyDictionary<string, object>> list = new List<IReadOnlyDictionary<string, object>>();
                Screen[] array = Screens;
                foreach (Screen screen in array)
                {
                    list.Add(screen.GetTelemetryInformation());
                }
                return list.ToArray();
            }
            catch (Exception exception)
            {
                //TODO:Handle Exception here
            }
            return Array.Empty<Dictionary<string, object>>();
        }
    }
}


