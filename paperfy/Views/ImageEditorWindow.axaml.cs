using Avalonia.Controls;
using Avalonia.Interactivity;
using Paperfy.Models;
using Paperfy.ViewModels;
using PaperFy.Shared.AppManager;
using System;
using System.Linq;

namespace Paperfy.Views;

public partial class ImageEditorWindow : Window
{
    public ImageEditorWindow()
    {
        InitializeComponent();

        // Handle window closing to reset application state
        Closing += OnWindowClosing;
    }

    private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        try
        {
            // Reset application state when window closes
            LocalSettings.Instance.IsAppDocumenting = false;
            ApplicationManager.IimageProcessor?.ClearImages();

            System.Diagnostics.Debug.WriteLine("Application state reset on ImageEditor window close");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error resetting state on window close: {ex.Message}");
        }
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