using PaperFy.Shared.Windows.Events;
using PaperFy.Shared.Windows.Services;
using System.Text.Json.Serialization;

namespace PaperFy.Shared.AppManager
{
    public class Settings
    {
        private const string Filename = "Persisted Data";

        private static bool isLoading = false;

        private bool captureLabels = true;
        private bool captureKeyboardEvents;
        private bool captureSpecialKeys = true;
        private bool dontIncludeTaskBar = false; // Add taskbar exclusion setting

        internal int markerSize = 10;
        internal string markerColor = "#FF0000";

        public static Settings Instance { get; private set; } = new Settings();

        [JsonPropertyName("capture_keyboard_events")]
        public bool CaptureKeyboardEvents
        {
            get
            {
                return captureKeyboardEvents;
            }
            set
            {
                if (captureKeyboardEvents != value)
                {
                    captureKeyboardEvents = value;
                    if (!isLoading)
                    {
                        EventAggregator.Instance.Publish(new SettingChangedEvent("CaptureKeyboardEvents", CaptureKeyboardEvents));
                    }
                }
            }
        }

        [JsonPropertyName("capture_labels")]
        public bool CaptureLabels
        {
            get
            {
                return captureLabels;
            }
            set
            {
                if (captureLabels != value)
                {
                    captureLabels = value;
                    if (!isLoading)
                    {
                        EventAggregator.Instance.Publish(new SettingChangedEvent("CaptureLabels", CaptureLabels));
                    }
                }
            }
        }

        [JsonPropertyName("capture_special_keys")]
        public bool CaptureSpecialKeys
        {
            get
            {
                return captureSpecialKeys;
            }
            set
            {
                if (captureSpecialKeys != value)
                {
                    captureSpecialKeys = value;
                    if (!isLoading)
                    {
                        EventAggregator.Instance.Publish(new SettingChangedEvent("CaptureSpecialKeys", CaptureSpecialKeys));
                    }
                }
            }
        }

        [JsonPropertyName("dont_include_taskbar")]
        public bool DontIncludeTaskBar
        {
            get
            {
                return dontIncludeTaskBar;
            }
            set
            {
                if (dontIncludeTaskBar != value)
                {
                    dontIncludeTaskBar = value;
                    if (!isLoading)
                    {
                        EventAggregator.Instance.Publish(new SettingChangedEvent("DontIncludeTaskBar", DontIncludeTaskBar));
                    }
                }
            }
        }

        [JsonPropertyName("marker_size")]
        public int MarkerSize
        {
            get
            {
                return markerSize;
            }
            set
            {
                if (markerSize != value)
                {
                    markerSize = value;
                    if (!isLoading)
                    {
                        EventAggregator.Instance.Publish(new SettingChangedEvent("MarkerSize", MarkerSize));
                    }
                }
            }
        }

        [JsonPropertyName("marker_color")]
        public string MarkerColor
        {
            get
            {
                return markerColor;
            }
            set
            {
                if (markerColor != value)
                {
                    markerColor = value;
                    if (!isLoading)
                    {
                        EventAggregator.Instance.Publish(new SettingChangedEvent("MarkerColor", MarkerColor));
                    }
                }
            }
        }

        protected string SettingsFilePath => Path.Combine(SystemService.Instance.LocalApplicationDataPath, "Persisted Data");

        [JsonConstructor]
        private Settings()
        {
        }
    }
}