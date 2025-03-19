using Nemeio.LayoutGen.Extensions;
using SkiaSharp;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;
using System.Drawing;
using System.IO;

namespace Nemeio.LayoutGen.Models.Loader
{
    public class SvgLoader : IImageLoader
    {
        public SKBitmap LoadImage(string filePath, Size size)
        {
            var svg = new SKSvg(new SKSize(size.Width, size.Height));
            svg.Load(filePath);

            return Resize(svg, size);
        }

        public SKBitmap LoadImage(Stream stream, Size size)
        {
            var svg = new SKSvg(new SKSize(size.Width, size.Height));
            svg.Load(stream);

            return Resize(svg, size);
        }

        private SKBitmap Resize(SKSvg svg, Size size)
        {
            var bitmap = new SKBitmap((int)size.Width, (int)size.Height);
            bitmap.Erase(SKColors.Transparent);

            using (var paint = new SKPaint())
            {
                paint.ColorFilter = SKColorFilter.CreateBlendMode(SKColors.Black, SKBlendMode.SrcIn);

                var canvas = new SKCanvas(bitmap);
                canvas.Clear(SKColors.Transparent);
                canvas.DrawPicture(svg.Picture, paint);

                return bitmap;
            }
        }
    }
}
