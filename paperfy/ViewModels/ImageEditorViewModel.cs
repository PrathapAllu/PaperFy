using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using PaperFy.Shared.AppManager;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Paperfy.ViewModels
{
    public class ImageEditorViewModel : ViewModelBase
    {
        private ObservableCollection<byte[]> _images;

        private byte[] _selectedImage;

        public byte[] SelectedImage
        {
            get => _selectedImage;
            set => this.RaiseAndSetIfChanged(ref _selectedImage, value);
        }

        public ICommand SelectImageCommand { get; }

        public ObservableCollection<byte[]> Images
        {
            get => _images;
            set => this.RaiseAndSetIfChanged(ref _images, value);
        }

        private string _selectedExportOption;

        public string SelectedExportOption
        {
            get => _selectedExportOption;
            set => this.RaiseAndSetIfChanged(ref _selectedExportOption, value);
        }

        public ICommand ExportSingleImageCommand { get; }
        public ICommand ExportToDocumentCommand { get; }

        public ImageEditorViewModel()
        {
            LoadImages();
            // Set first image as selected by default
            if (Images?.Any() == true)
                SelectedImage = Images.First();

            SelectImageCommand = ReactiveCommand.Create<byte[]>(image => SelectedImage = image);

            ExportSingleImageCommand = ReactiveCommand.Create(ExportSingleImage);
            ExportToDocumentCommand = ReactiveCommand.Create(ExportToDocument);
        }

        private void LoadImages()
        {
            var markedImages = ApplicationManager.IimageProcessor.GetMarkedImages();
            Images = new ObservableCollection<byte[]>(markedImages.Select(x => x.image));
        }

private async void ExportSingleImage()
    {
        if (SelectedImage == null)
        {
            // Show message to user that no image is selected
            return;
        }

        try
        {
            // Get window for file dialog
            var window = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.MainWindow
                : null;

            if (window == null) return;

            // Open save file dialog
            var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save Image As",
                DefaultExtension = "png",
                SuggestedFileName = "exported_image.png",
                FileTypeChoices = new[]
                {
                new FilePickerFileType("PNG Image") { Patterns = new[] { "*.png" } },
                new FilePickerFileType("JPEG Image") { Patterns = new[] { "*.jpg", "*.jpeg" } }
            }
            });

            if (file != null)
            {
                // Save the selected image
                await using var stream = await file.OpenWriteAsync();
                await stream.WriteAsync(SelectedImage);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving image: {ex.Message}");
        }
    }

    private async void ExportToDocument()
    {
        if (Images == null || !Images.Any())
        {
            return;
        }

        try
        {
            // Get window for file dialog
            var window = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                ? desktop.MainWindow
                : null;

            if (window == null) return;

            // Open save file dialog for PDF
            var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Export to PDF Document",
                DefaultExtension = "pdf",
                SuggestedFileName = "exported_images.pdf",
                FileTypeChoices = new[]
                {
                new FilePickerFileType("PDF Document") { Patterns = new[] { "*.pdf" } }
            }
            });

            if (file != null)
            {
                // Create PDF document
                var document = new PdfDocument();
                document.Info.Title = "Exported Images";

                foreach (var imageBytes in Images)
                {
                    // Create a new page for each image
                    var page = document.AddPage();
                    var gfx = XGraphics.FromPdfPage(page);

                    // Create image from bytes
                    using var imageStream = new MemoryStream(imageBytes);
                    var image = XImage.FromStream(imageStream);

                    // Calculate dimensions to fit page while maintaining aspect ratio
                    var pageWidth = page.Width;
                    var pageHeight = page.Height - 100; // Leave space for margins

                    var imageWidth = image.PixelWidth;
                    var imageHeight = image.PixelHeight;

                    var scaleX = pageWidth / imageWidth;
                    var scaleY = pageHeight / imageHeight;
                    var scale = Math.Min(scaleX, scaleY);

                    var scaledWidth = imageWidth * scale;
                    var scaledHeight = imageHeight * scale;

                    // Center the image
                    var x = (pageWidth - scaledWidth) / 2;
                    var y = 50; // Top margin

                    // Draw the image
                    gfx.DrawImage(image, x, y, scaledWidth, scaledHeight);

                    gfx.Dispose();
                }

                // Save PDF
                await using var stream = await file.OpenWriteAsync();
                document.Save(stream);
                document.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating PDF: {ex.Message}");
        }
    }
}
}
