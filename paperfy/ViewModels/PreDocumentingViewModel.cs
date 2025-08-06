using Paperfy.Models;
using PaperFy.Shared.AppManager;
using PaperFy.Shared.Windows.Events;
using PaperFy.Shared.Windows.Services;
using ReactiveUI;
using System.Windows.Input;

namespace Paperfy.ViewModels;

public class PreDocumentingViewModel : ParentViewModel
{
    private readonly MainViewModel _mainViewModel;
    private string _documentName = string.Empty;
    private bool _enableVoice;
    private bool _enableDontIncludeTaskBar;
    private bool _enableShiftClick;

    public string DocumentName
    {
        get => _documentName;
        set => this.RaiseAndSetIfChanged(ref _documentName, value);
    }

    public bool EnableVoice
    {
        get => _enableVoice;
        set => this.RaiseAndSetIfChanged(ref _enableVoice, value);
    }

    public bool EnableDontIncludeTaskBar
    {
        get => Settings.Instance.DontIncludeTaskBar;
        set
        {
            Settings.Instance.DontIncludeTaskBar = value;
            this.RaisePropertyChanged();
        }
    }

    public bool EnableShiftClick
    {
        get => _enableShiftClick;
        set => this.RaiseAndSetIfChanged(ref _enableShiftClick, value);
    }

    public ICommand ImportPicturesCommand { get; }
    public ICommand StartRecordingCommand { get; }

    public ICommand CancelRecordCommand { get; }

    public PreDocumentingViewModel(MainViewModel parent) : base(parent)
    {
        _mainViewModel = parent;

        _enableDontIncludeTaskBar = Settings.Instance.DontIncludeTaskBar;
        LocalSettings.Instance.IsDontIncludeTaskBar = Settings.Instance.DontIncludeTaskBar;

        EventAggregator.Instance.Subscribe<SettingChangedEvent>(OnSettingChanged);

        ImportPicturesCommand = ReactiveCommand.Create(ImportPictures);

        var canStartRecording = this.WhenAnyValue(x => x.DocumentName, name => !string.IsNullOrWhiteSpace(name));
        StartRecordingCommand = ReactiveCommand.Create(() =>
        {
            LocalSettings.Instance.IsAppDocumenting = true;
            base.Parent.Minimize();
            ApplicationManager.DocumenterService?.StartDocumenting(false);

            parent.SwitchView("capture");
        }, canStartRecording);

        CancelRecordCommand = ReactiveCommand.Create(() =>
        {
            parent.SwitchView("capture");
        });
    }

    private void OnSettingChanged(SettingChangedEvent settingChanged)
    {
        if (settingChanged.Name == "DontIncludeTaskBar")
        {
            var newValue = (bool)settingChanged.Value;
            if (_enableDontIncludeTaskBar != newValue)
            {
                _enableDontIncludeTaskBar = newValue;
                this.RaisePropertyChanged(nameof(EnableDontIncludeTaskBar));
                LocalSettings.Instance.IsDontIncludeTaskBar = newValue;
            }
        }
    }

    private void ImportPictures()
    {
        // Picture import logic
    }
}