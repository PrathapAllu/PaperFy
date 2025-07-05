using PaperFy.Shared.Windows.Models;

namespace PaperFy.Shared.Interface
{
    public interface IimageProcessor
    {
        void AddImage(byte[] image, Point clickPoint);
        void ClearImages();
        bool HasImages { get; }
        IEnumerable<(byte[] image, Point clickPoint)> GetMarkedImages();
    }
}
