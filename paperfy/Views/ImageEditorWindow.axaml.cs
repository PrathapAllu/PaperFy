using Avalonia.Controls;
using Avalonia.Interactivity;
using Paperfy.ViewModels;
using PaperFy.Shared.AppManager;
using System.Linq;

namespace Paperfy.Views;

public partial class ImageEditorWindow : Window
{
    public ImageEditorWindow()
    {
        InitializeComponent();
    }

    private void DeleteImage_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is byte[] imageToDelete)
        {
            var viewModel = DataContext as ImageEditorViewModel;

            // Remove from data structure
            ApplicationManager.IimageProcessor.RemoveImage(imageToDelete);

            // Remove from UI
            viewModel?.Images.Remove(imageToDelete);

            // Update selection
            if (viewModel?.SelectedImage == imageToDelete)
            {
                viewModel.SelectedImage = viewModel.Images.FirstOrDefault();
            }
        }
    }
}