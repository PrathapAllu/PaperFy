using PaperFy.Shared.Windows.Models;

namespace PaperFy.Shared.Interface
{
    public interface IimageProcessor
    {
        void AddImage(byte[] image, Point clickPoint, string description = "");
        void ClearImages();
        bool HasImages { get; }
        IEnumerable<(byte[] image, Point clickPoint, string description)> GetMarkedImages();
        void RemoveImage(byte[] image);
    }
}