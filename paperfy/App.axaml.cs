using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Paperfy.ViewModels;
using Paperfy.Views;
using PaperFy.Shared.Windows.Service;

namespace Paperfy;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainViewModel = new MainViewModel(
                new ApplicationStateService(desktop),
                desktop.Args
            );

            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel
            };

            if (true)
            {
                //shellViewModel.Restore();
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}
