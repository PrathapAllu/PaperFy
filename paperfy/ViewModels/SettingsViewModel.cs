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
                get => Settings.Instance.DontIncludeTaskBar;
                set
                {
                    Settings.Instance.DontIncludeTaskBar = value;
                    this.RaisePropertyChanged();
                }
            }

            public SettingsViewModel()
            {
                // Initialize from Settings


                // Subscribe to changes
                EventAggregator.Instance.Subscribe<SettingChangedEvent>(OnSettingChanged);
            }

            private void OnSettingChanged(SettingChangedEvent settingChanged)
            {
                if (settingChanged.Name == "DontIncludeTaskBar")
                {
                    this.RaisePropertyChanged(nameof(DontIncludeTaskBar));
                }
            }
        }
    }
}