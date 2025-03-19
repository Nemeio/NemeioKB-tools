using System.Drawing;
using System.IO;
using SkiaSharp;

namespace Nemeio.LayoutGen.Models.Loader
{
    public class PngLoader : IImageLoader
    {
        public SKBitmap LoadImage(string filePath, Size size)
        {
            SKBitmap bmp = SKBitmap.Decode(filePath);

            return ResizeImageIfNeeded(bmp, size);
        }

        public SKBitmap LoadImage(Stream stream, Size size)
        {
            SKBitmap bmp = SKBitmap.Decode(stream);

            return ResizeImageIfNeeded(bmp, size);
        }

        private SKBitmap ResizeImageIfNeeded(SKBitmap bmp, Size size)
        {
            if (bmp.Width == size.Width && bmp.Height == size.Height)
            {
                return bmp;
            }

            var scaled = bmp.Resize(
                new SKImageInfo((int)size.Width, (int)size.Height),
                SKBitmapResizeMethod.Lanczos3
            );

            return scaled;
        }
    }
}
