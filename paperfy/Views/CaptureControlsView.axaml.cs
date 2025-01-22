using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Paperfy.Views;

public partial class CaptureControlsView : UserControl
{
    public CaptureControlsView()
    {
        InitializeComponent();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var window = new Window
        {
            Content = new ScreenShotEditorView(),
            // Optional: Set window properties
            Width = 800,
            Height = 600,
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };

        window.Show();
    }
}