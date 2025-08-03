using PaperFy.Shared.AppManager;
using PaperFy.Shared.Interface;
using System.Drawing;
using System.Drawing.Imaging;
using Point = PaperFy.Shared.Windows.Models.Point;

namespace PaperFy.Shared.Windows.Services
{
    public class ImageProcessor : IimageProcessor, IDisposable
    {
        private readonly List<(byte[] image, Point clickPoint)> capturedImages = new();

        public void ClearImages()
        {
            capturedImages.Clear();
        }

        public bool HasImages => capturedImages.Any();

        public IEnumerable<(byte[] image, Point clickPoint)> GetMarkedImages()
        {
            foreach (var (image, point) in capturedImages)
            {
                var markedImage = DrawMarker(image, point);
                yield return (markedImage, point);
            }
        }

        private byte[] DrawMarker(byte[] imageBytes, Point point)
        {
            using var ms = new MemoryStream(imageBytes);
            using var image = new Bitmap(ms);
            using var graphics = Graphics.FromImage(image);

            var markerSize = Settings.Instance.markerSize;
            var color = ColorTranslator.FromHtml(Settings.Instance.markerColor);

            graphics.FillEllipse(new SolidBrush(color),
                point.X - markerSize / 2,
                point.Y - markerSize / 2,
                markerSize,
                markerSize);

            using var resultStream = new MemoryStream();
            image.Save(resultStream, ImageFormat.Png);
            return resultStream.ToArray();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void AddImage(byte[] image, Models.Point clickPoint)
        {
            capturedImages.Add((image, clickPoint));
        }

        public void RemoveImage(byte[] imageToRemove)
        {
            var itemToRemove = capturedImages.FirstOrDefault(x => x.image.SequenceEqual(imageToRemove));
            if (itemToRemove != default)
            {
                capturedImages.Remove(itemToRemove);
            }
        }
    }
}
