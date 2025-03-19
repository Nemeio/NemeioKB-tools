using System.Drawing;
using System.IO;
using SkiaSharp;

namespace Nemeio.LayoutGen.Models.Loader
{
    public interface IImageLoader
    {
        SKBitmap LoadImage(string filePath, Size size);

        SKBitmap LoadImage(Stream stream, Size size);
    }
}
