using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Paperfy.Services;
using PaperFy.Shared.AppManager;
using PaperFy.Shared.Windows.Models;
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
                // Save current annotation before switching
                if (_selectedImage != null && !string.IsNullOrWhiteSpace(_selectedImageAnnotation))
                {
                    _imageAnnotations[_selectedImage] = _selectedImageAnnotation;
                }

                this.RaiseAndSetIfChanged(ref _selectedImage, value);
                this.RaisePropertyChanged(nameof(SelectedImageStatusText));
                this.RaisePropertyChanged(nameof(HasSelectedImage));
                this.RaisePropertyChanged(nameof(ShowPlaceholder));

                if (value != null)
                {
                    if (_imageAnnotations.TryGetValue(value, out var annotation))
                    {
                        SelectedImageAnnotation = annotation;
                    }
                    else if (_clickDescriptions.TryGetValue(value, out var clickDescription))
                    {
                        SelectedImageAnnotation = clickDescription;
                        _imageAnnotations[value] = clickDescription; // Store it
                    }
                    else
                    {
                        SelectedImageAnnotation = string.Empty;
                    }
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
            var markedImages = ApplicationManager.IimageProcessor.GetMarkedImages();
            var imageList = markedImages.ToList();
            Images = new ObservableCollection<byte[]>(markedImages.Select(x => x.image));

            InitializeClickDescriptions(imageList);
        }

        private void InitializeClickDescriptions(List<(byte[] image, Point clickPoint)> markedImages)
        {
            _clickDescriptions.Clear();

            for (int i = 0; i < markedImages.Count; i++)
            {
                var (image, clickPoint) = markedImages[i];

                var description = GenerateDefaultDescription(i + 1, clickPoint);
                _clickDescriptions[image] = description;
            }
        }

        private string GenerateDefaultDescription(int imageNumber, Point clickPoint)
        {
            return $"Step {imageNumber}: Click at position ({clickPoint.X}, {clickPoint.Y})";
        }

        public void UpdateClickDescription(byte[] image, string description)
        {
            if (image != null && !string.IsNullOrWhiteSpace(description))
            {
                _clickDescriptions[image] = description;

                if (_selectedImage != null && _selectedImage.SequenceEqual(image))
                {
                    if (string.IsNullOrWhiteSpace(_selectedImageAnnotation) ||
                        _selectedImageAnnotation == _clickDescriptions.GetValueOrDefault(image))
                    {
                        SelectedImageAnnotation = description;
                    }
                }
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
    }
}