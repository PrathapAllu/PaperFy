using PdfSharp.Fonts;
using System;
using System.IO;
using System.Linq;

namespace Paperfy.Services
{
    public class SimpleFontResolver : IFontResolver
    {
        public byte[] GetFont(string faceName)
        {
            // Try system fonts directory
            var fontsDir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
            var fontFiles = new[] { "arial.ttf", "calibri.ttf", "tahoma.ttf", "segoeui.ttf" };

            foreach (var fontFile in fontFiles)
            {
                var fontPath = Path.Combine(fontsDir, fontFile);
                if (File.Exists(fontPath))
                    return File.ReadAllBytes(fontPath);
            }

            // Fallback to any available .ttf file
            var anyFont = Directory.GetFiles(fontsDir, "*.ttf").FirstOrDefault();
            return anyFont != null ? File.ReadAllBytes(anyFont) : null;
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            return new FontResolverInfo(familyName);
        }
    }
}