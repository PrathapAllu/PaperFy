using PaperFy.Shared.AppManager;
using ReactiveUI;
using System.Windows.Input;

namespace Paperfy.ViewModels;

public class PreDocumentingViewModel : ParentViewModel
{
    private readonly MainViewModel _mainViewModel;
    private string _documentName = string.Empty;
    private bool _enableVoice;
    private bool _enableKeyboard;
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

    public bool EnableKeyboard
    {
        get => _enableKeyboard;
        set => this.RaiseAndSetIfChanged(ref _enableKeyboard, value);
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
        ImportPicturesCommand = ReactiveCommand.Create(ImportPictures);
        StartRecordingCommand = ReactiveCommand.Create(StartRecording);
        CancelRecordCommand = ReactiveCommand.Create(() =>
        {
            parent.SwitchView("capture");
        });
    }

    private void ImportPictures()
    {
        // Picture import logic
    }

    private void StartRecording()
    {
        base.Parent.Minimize();
        ApplicationManager.DocumenterService?.StartDocumenting(false);
    }
}