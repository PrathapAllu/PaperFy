using PaperFy.Shared.AppManager;
using PaperFy.Shared.Interface;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Point = PaperFy.Shared.Windows.Models.Point;

namespace PaperFy.Shared.Windows.Services
{
    public class ImageProcessor : IimageProcessor, IDisposable
    {
        private readonly List<CapturedImageData> capturedImages = new();

        public void ClearImages()
        {
            capturedImages.Clear();
        }

        public bool HasImages => capturedImages.Any();

        public IEnumerable<(byte[] image, Point clickPoint)> GetMarkedImages()
        {
            foreach (var imageData in capturedImages)
            {
                var markedImage = DrawEnhancedMarker(imageData.ImageBytes, imageData.ClickPoint);
                yield return (markedImage, imageData.ClickPoint);
            }
        }

        public void AddImage(byte[] image, Point clickPoint)
        {
            capturedImages.Add(new CapturedImageData
            {
                ImageBytes = image,
                ClickPoint = clickPoint,
                Timestamp = DateTime.UtcNow
            });
        }

        public void RemoveImage(byte[] imageToRemove)
        {
            var itemToRemove = capturedImages.FirstOrDefault(x => x.ImageBytes.SequenceEqual(imageToRemove));
            if (itemToRemove != null)
            {
                capturedImages.Remove(itemToRemove);
            }
        }

        private byte[] DrawEnhancedMarker(byte[] imageBytes, Point point)
        {
            using var ms = new MemoryStream(imageBytes);
            using var image = new Bitmap(ms);
            using var graphics = Graphics.FromImage(image);

            // Enable anti-aliasing for smooth circles
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.CompositingQuality = CompositingQuality.HighQuality;

            // Calculate optimal marker size based on image dimensions
            var optimalSize = CalculateOptimalMarkerSize(image.Width, image.Height);
            var color = ColorTranslator.FromHtml(Settings.Instance.MarkerColor);

            // Draw outer circle (slightly transparent)
            var outerSize = (int)(optimalSize * 1.4f);
            var outerColor = Color.FromArgb(50, color); // 30% opacity
            using (var outerBrush = new SolidBrush(outerColor))
            {
                graphics.FillEllipse(outerBrush,
                    point.X - outerSize / 2,
                    point.Y - outerSize / 2,
                    outerSize,
                    outerSize);
            }

            // Draw main circle (solid)
            using (var mainBrush = new SolidBrush(color))
            {
                graphics.FillEllipse(mainBrush,
                    point.X - optimalSize / 2,
                    point.Y - optimalSize / 2,
                    optimalSize,
                    optimalSize);
            }

            var innerSize = (int)(optimalSize * 0.6f);
            var highlightColor = Color.FromArgb(150, Color.White);
            using (var innerBrush = new SolidBrush(highlightColor))
            {
                graphics.FillEllipse(innerBrush,
                    point.X - innerSize / 2,
                    point.Y - innerSize / 2,
                    innerSize,
                    innerSize);
            }

            // Draw border for definition
            var borderPen = new Pen(Color.FromArgb(150, Color.DarkRed), 1.5f);
            graphics.DrawEllipse(borderPen,
                point.X - optimalSize / 2,
                point.Y - optimalSize / 2,
                optimalSize,
                optimalSize);

            using var resultStream = new MemoryStream();
            image.Save(resultStream, ImageFormat.Png);
            return resultStream.ToArray();
        }

        private int CalculateOptimalMarkerSize(int imageWidth, int imageHeight)
        {
            // Base size from settings
            var baseSize = Settings.Instance.MarkerSize;

            // Scale based on image resolution
            var avgDimension = (imageWidth + imageHeight) / 2.0f;
            var scaleFactor = avgDimension / 1000.0f; // Assume 1000px as baseline

            // Clamp scale factor to reasonable bounds
            scaleFactor = Math.Max(0.5f, Math.Min(2.0f, scaleFactor));

            var calculatedSize = (int)(baseSize * scaleFactor);

            return Math.Max(8, Math.Min(60, calculatedSize));
        }

        public void Dispose()
        {
            capturedImages.Clear();
        }

        private class CapturedImageData
        {
            public byte[] ImageBytes { get; set; }
            public Point ClickPoint { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}