using PaperFy.Shared.Windows.Models;
using PaperFy.Shared.Windows.Service;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;

namespace PaperFy.Shared.Windows.Services
{
    public class WindowsScreenCaptureService : ScreenCaptureService<Bitmap>
    {
        protected override byte[] EncodeNativeImage(Bitmap nativeImage)
        {
            if (nativeImage != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    nativeImage.Save(memoryStream, ImageFormat.Jpeg);
                    return memoryStream.ToArray();
                }
            }
            return null;
        }

        protected override Bitmap CaptureScreen(Screen screen)
        {
            Bitmap bitmap = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, PixelFormat.Format24bppRgb);
            try
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(screen.Bounds.Left, screen.Bounds.Top, 0, 0, new Size(screen.Bounds.Width, screen.Bounds.Height), CopyPixelOperation.SourceCopy);
                }
                return bitmap;
            }
            catch (Win32Exception ex)
            {

            }
            return null;
        }
    }
}


