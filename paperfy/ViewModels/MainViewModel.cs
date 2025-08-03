using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Paperfy.ViewModels;
using Paperfy.ViewModels.Paperfy.ViewModels;
using Paperfy.Views;
using PaperFy.Shared.Windows.Service;
using ReactiveUI;
using System;
using System.Windows.Input;

public class MainViewModel : ViewModelBase
{
    public string UserName { get; set; } = Environment.UserName;
    private object _currentView;
    public object CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    private readonly CaptureControlsView _captureView;
    private readonly SettingsView _settingsView;
    private readonly CheckForUpdatesView _updatesView;
    private readonly AboutView _aboutView;
    private readonly PreDocumentingView _preDocumentingView;

    public PreDocumentingViewModel PreDocumentingViewModel { get; }

    internal IClassicDesktopStyleApplicationLifetime DesktopLifetime => Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
    internal MainWindow MainWindow => DesktopLifetime?.MainWindow as MainWindow;

    public CaptureControlsViewModel CaptureControlsViewModel { get; }

    public MainViewModel(ApplicationStateService applicationStateService, string[] arguments)
    {
        _settingsView = new SettingsView();
        _settingsView.DataContext = new SettingsViewModel();

        _preDocumentingView = new PreDocumentingView();
        PreDocumentingViewModel = new PreDocumentingViewModel(this);
        _preDocumentingView.DataContext = PreDocumentingViewModel;

        _captureView = new CaptureControlsView();
        _settingsView = new SettingsView();
        _updatesView = new CheckForUpdatesView();
        _aboutView = new AboutView();

        CaptureControlsViewModel = new CaptureControlsViewModel(this);

        NavigateCommand = ReactiveCommand.Create<string>(SwitchView);
        CurrentView = _captureView;
    }

    public void SwitchView(string view) => CurrentView = view switch
    {
        "predocumenting" => _preDocumentingView,
        "capture" => _captureView,
        "settings" => _settingsView,
        "updates" => _updatesView,
        "about" => _aboutView,
        _ => _preDocumentingView
    };

    public ICommand NavigateCommand { get; }

    public void Minimize()
    {
        if (MainWindow != null)
        {
            MainWindow.WindowState = WindowState.Minimized;
        }
    }
}