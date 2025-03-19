using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nemeio.LayoutGen.Extensions
{
    public static class SKBitmapExtensions
    {
        public static SKBitmap FlipHorizontal(this SKBitmap bitmap)
        {
            var rotated = new SKBitmap(bitmap.Width, bitmap.Height);
            using (var surface = new SKCanvas(rotated))
            {
                surface.Translate(bitmap.Width, 0);
                surface.Scale(-1, 1, 0, 0);
                surface.DrawBitmap(bitmap, 0, 0);
            }
            return rotated;
        }

        public static SKData ToPng(this SKBitmap bitmap)
        {
            SKImage png = SKImage.FromBitmap(bitmap);

            return png.Encode(SKEncodedImageFormat.Png, 100);
        }
    }
}
