using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Paperfy.Models;
using Paperfy.Services;
using PaperFy.Shared.AppManager;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using ReactiveUI;
using System;
using System.Collections.Generic;
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
        private string _selectedImageAnnotation = string.Empty;
        private string _statusMessage = "Ready to export";
        private Dictionary<byte[], string> _imageAnnotations = new();
        private string _documentName;
        private Dictionary<byte[], string> _clickDescriptions = new();

        public byte[] SelectedImage
        {
            get => _selectedImage;
            set
            {
                if (_selectedImage != null && !string.IsNullOrWhiteSpace(_selectedImageAnnotation))
                {
                    _imageAnnotations[_selectedImage] = _selectedImageAnnotation;
                }

                this.RaiseAndSetIfChanged(ref _selectedImage, value);
                this.RaisePropertyChanged(nameof(SelectedImageStatusText));
                this.RaisePropertyChanged(nameof(HasSelectedImage));
                this.RaisePropertyChanged(nameof(ShowPlaceholder));

                if (value != null && _imageAnnotations.TryGetValue(value, out var annotation))
                {
                    SelectedImageAnnotation = annotation;
                }
                else
                {
                    SelectedImageAnnotation = string.Empty;
                }
            }
        }

        public string SelectedImageStatusText => SelectedImage != null ? "Image selected" : "No image selected";
        public bool HasSelectedImage => SelectedImage != null;
        public bool ShowPlaceholder => SelectedImage == null;

        public string SelectedImageAnnotation
        {
            get => _selectedImageAnnotation;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedImageAnnotation, value);

                // Auto-save annotation
                if (_selectedImage != null)
                {
                    _imageAnnotations[_selectedImage] = value ?? string.Empty;
                }
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => this.RaiseAndSetIfChanged(ref _statusMessage, value);
        }

        public ICommand SelectImageCommand { get; }
        public ObservableCollection<byte[]> Images
        {
            get => _images;
            set => this.RaiseAndSetIfChanged(ref _images, value);
        }

        public ICommand ExportSingleImageCommand { get; }
        public ICommand ExportToDocumentCommand { get; }

        public ImageEditorViewModel(string documentName = "")
        {
            GlobalFontSettings.FontResolver = new SimpleFontResolver();

            _documentName = string.IsNullOrWhiteSpace(documentName) ? "exported_images" : documentName;

            LoadImages();

            if (Images?.Any() == true)
                SelectedImage = Images.First();

            SelectImageCommand = ReactiveCommand.Create<byte[]>(image => SelectedImage = image);
            ExportSingleImageCommand = ReactiveCommand.Create(ExportSingleImage);
            ExportToDocumentCommand = ReactiveCommand.Create(ExportToDocument);
        }

        private void LoadImages()
        {
            var markedImages = ApplicationManager.IimageProcessor.GetMarkedImages().ToList();
            Images = new ObservableCollection<byte[]>(markedImages.Select(x => x.image));

            _clickDescriptions.Clear();
            _imageAnnotations.Clear();

            foreach (var (image, clickPoint, description) in markedImages)
            {
                var finalDescription = !string.IsNullOrWhiteSpace(description)
                    ? description
                    : $"Click at ({clickPoint.X}, {clickPoint.Y})";

                _clickDescriptions[image] = finalDescription;
                _imageAnnotations[image] = finalDescription;
            }
        }

        private async void ExportSingleImage()
        {
            if (SelectedImage == null)
            {
                StatusMessage = "Please select an image to export";
                return;
            }

            try
            {
                StatusMessage = "Exporting image...";

                var window = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow : null;

                if (window == null) return;

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
                    await using var stream = await file.OpenWriteAsync();
                    await stream.WriteAsync(SelectedImage);
                    StatusMessage = "Image exported successfully";

                    // Reset application state after successful export
                    ResetApplicationState();
                }
                else
                {
                    StatusMessage = "Export cancelled";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Export failed: {ex.Message}";
            }
        }

        private async void ExportToDocument()
        {
            if (Images == null || !Images.Any())
            {
                StatusMessage = "No images to export";
                return;
            }

            try
            {
                StatusMessage = "Creating PDF document...";

                var window = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow : null;

                if (window == null) return;

                var file = await window.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Export to PDF Document",
                    DefaultExtension = "pdf",
                    SuggestedFileName = $"{_documentName}.pdf",
                    FileTypeChoices = new[]
                    {
                new FilePickerFileType("PDF Document") { Patterns = new[] { "*.pdf" } }
            }
                });

                if (file != null)
                {
                    var document = new PdfDocument();
                    document.Info.Title = "Exported Images";

                    var imagesPerPage = Settings.Instance.ImagesPerPage;
                    var imageList = Images.ToList();

                    for (int i = 0; i < imageList.Count; i += imagesPerPage)
                    {
                        var page = document.AddPage();
                        var gfx = XGraphics.FromPdfPage(page);

                        var pageWidth = page.Width;
                        var pageHeight = page.Height;
                        var margin = 50;

                        var imagesToProcess = imageList.Skip(i).Take(imagesPerPage).ToList();
                        var imageHeight = (pageHeight - (2 * margin) - ((imagesToProcess.Count - 1) * 20)) / imagesToProcess.Count;

                        for (int j = 0; j < imagesToProcess.Count; j++)
                        {
                            var imageBytes = imagesToProcess[j];

                            using var imageStream = new MemoryStream(imageBytes);
                            var image = XImage.FromStream(imageStream);

                            var availableWidth = pageWidth - (2 * margin);
                            var scaleX = availableWidth / image.PixelWidth;
                            var scaleY = imageHeight / image.PixelHeight;
                            var scale = Math.Min(scaleX, scaleY);

                            var scaledWidth = image.PixelWidth * scale;
                            var scaledHeight = image.PixelHeight * scale;

                            var x = (pageWidth - scaledWidth) / 2;
                            var y = margin + (j * (imageHeight + 20));

                            gfx.DrawImage(image, x, y, scaledWidth, scaledHeight);

                            if (_imageAnnotations.TryGetValue(imageBytes, out var annotation) &&
                                !string.IsNullOrWhiteSpace(annotation))
                            {
                                var font = new XFont("Arial", 10);
                                var textY = y + scaledHeight + 5;
                                var textRect = new XRect(margin, textY, pageWidth - (2 * margin), 15);
                                gfx.DrawString(annotation, font, XBrushes.Black, textRect, XStringFormats.TopLeft);
                            }
                        }

                        gfx.Dispose();
                    }

                    await using var stream = await file.OpenWriteAsync();
                    document.Save(stream);
                    document.Close();

                    StatusMessage = $"PDF exported successfully with {Images.Count} images";

                    // Reset application state after successful export
                    ResetApplicationState();
                }
                else
                {
                    StatusMessage = "Export cancelled";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"PDF export failed: {ex.Message}";
            }
        }

        private void ResetApplicationState()
        {
            try
            {
                // Reset local settings
                LocalSettings.Instance.IsAppDocumenting = false;

                // Clear captured images
                ApplicationManager.IimageProcessor?.ClearImages();

                // Reset documenter service state if available
                if (ApplicationManager.DocumenterService != null)
                {
                    // The service should already be stopped, but ensure it's in idle state
                    // This is handled in the DocumenterService.StopDocumenting method
                }

                System.Diagnostics.Debug.WriteLine("Application state reset successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error resetting application state: {ex.Message}");
            }
        }
    }
}