using Paperfy.Models;
using PaperFy.Shared.AppManager;
using PaperFy.Shared.Windows.Events;
using PaperFy.Shared.Windows.Services;
using ReactiveUI;

namespace Paperfy.ViewModels
{
    namespace Paperfy.ViewModels
    {
        public class SettingsViewModel : ViewModelBase
        {
            private bool _dontIncludeTaskBar;

            public bool DontIncludeTaskBar
            {
                get => _dontIncludeTaskBar;
                set
                {
                    this.RaiseAndSetIfChanged(ref _dontIncludeTaskBar, value);
                    Settings.Instance.DontIncludeTaskBar = value;
                    LocalSettings.Instance.IsDontIncludeTaskBar = value;
                }
            }

            public SettingsViewModel()
            {
                // Initialize from Settings
                _dontIncludeTaskBar = Settings.Instance.DontIncludeTaskBar;

                // Subscribe to changes
                EventAggregator.Instance.Subscribe<SettingChangedEvent>(OnSettingChanged);
            }

            private void OnSettingChanged(SettingChangedEvent settingChanged)
            {
                if (settingChanged.Name == "DontIncludeTaskBar")
                {
                    var newValue = (bool)settingChanged.Value;
                    if (_dontIncludeTaskBar != newValue)
                    {
                        _dontIncludeTaskBar = newValue;
                        this.RaisePropertyChanged(nameof(DontIncludeTaskBar));
                    }
                }
            }
        }
    }
}