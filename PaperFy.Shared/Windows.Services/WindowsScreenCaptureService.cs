using PaperFy.Shared.Windows.Models;
using PaperFy.Shared.Windows.Service;
using PaperFy.Shared.Windows.Utilities;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using Rectangle = PaperFy.Shared.Windows.Models.Rectangle; // Resolve ambiguity

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

        protected override Bitmap CaptureScreen(Screen screen, bool excludeTaskbar = false)
        {
            Rectangle captureBounds;

            // Check if taskbar exclusion is enabled
            if (excludeTaskbar)
            {
                captureBounds = TaskbarUtilities.CalculateExcludeTaskbarBounds(screen);
            }
            else
            {
                captureBounds = screen.Bounds;
            }

            Bitmap bitmap = new Bitmap(captureBounds.Width, captureBounds.Height, PixelFormat.Format24bppRgb);
            try
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(
                        captureBounds.X,
                        captureBounds.Y,
                        0,
                        0,
                        new System.Drawing.Size(captureBounds.Width, captureBounds.Height),
                        CopyPixelOperation.SourceCopy
                    );
                }
                return bitmap;
            }
            catch (Win32Exception ex)
            {
                // Handle exception - maybe log it
                return null;
            }
        }
    }
}