using System.ComponentModel;

namespace Paperfy.Models
{
    public class LocalSettings : INotifyPropertyChanged
    {
        private bool _isVoiceRecordingOn;
        private bool _isDontIncludeTaskBar;
        private bool _isShiftClickCapture;
        private string _markerColor;
        private bool _isAppDocumenting = false;

        public bool IsVoiceRecordingOn
        {
            get => _isVoiceRecordingOn;
            set
            {
                if (_isVoiceRecordingOn != value)
                {
                    _isVoiceRecordingOn = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsDontIncludeTaskBar
        {
            get => _isDontIncludeTaskBar;
            set
            {
                if (_isDontIncludeTaskBar != value)
                {
                    _isDontIncludeTaskBar = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsShiftClickCapture
        {
            get => _isShiftClickCapture;
            set
            {
                if (_isShiftClickCapture != value)
                {
                    _isShiftClickCapture = value;
                    OnPropertyChanged();
                }
            }
        }

        public static LocalSettings Instance { get; private set; } = new LocalSettings();

        public string markerColor
        {
            get => _markerColor;
            set
            {
                if (_markerColor != value)
                {
                    _markerColor = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsAppDocumenting
        {
            get => _isAppDocumenting;
            set
            {
                if (_isAppDocumenting != value)
                {
                    _isAppDocumenting = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}