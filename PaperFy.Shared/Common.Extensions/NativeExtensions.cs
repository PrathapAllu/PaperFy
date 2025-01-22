using Avalonia;
using PaperFy.Shared.Windows.Models;

namespace PaperFy.Shared.Common.Extensions
{
    public static class NativeExtensions
    {
        public static Rectangle ToPaperFy(this PixelRect rectangle)
        {
            return new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public static string ToDescription(this PixelRect rectangle)
        {
            return $"({rectangle.X}, {rectangle.Y}) {rectangle.Width} x {rectangle.Height}";
        }
    }
}
