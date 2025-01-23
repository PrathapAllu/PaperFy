using PaperFy.Shared.Windows.Service;
using Paperfy.ViewModels;
using Paperfy.Views;
using ReactiveUI;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using Paperfy;
using System;

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

    internal IClassicDesktopStyleApplicationLifetime DesktopLifetime => Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
    internal MainWindow MainWindow => DesktopLifetime?.MainWindow as MainWindow;

    public CaptureControlsViewModel CaptureControlsViewModel { get; }

    public MainViewModel(ApplicationStateService applicationStateService, string[] arguments)
    {
        _captureView = new CaptureControlsView();
        _settingsView = new SettingsView();
        _updatesView = new CheckForUpdatesView();
        _aboutView = new AboutView();

        CaptureControlsViewModel = new CaptureControlsViewModel(this);

        NavigateCommand = ReactiveCommand.Create<string>(SwitchView);
        CurrentView = _captureView;
    }

    private void SwitchView(string view) => CurrentView = view switch
    {
        "capture" => _captureView,
        "settings" => _settingsView,
        "updates" => _updatesView,
        "about" => _aboutView,
        _ => _captureView
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