using Nemeio.LayoutGen.Factory;
using System.Drawing;
using System.IO;
using Nemeio.LayoutGen.Extensions;
using SkiaSharp;

namespace Nemeio.LayoutGen.Models.Displayer
{
    public class ImageDisplayer : BaseDisplayer
    {
        private Stream _stream;

        public ImageDisplayer(Key key, string val) : base(key, val)
        {

        }

        public ImageDisplayer(Key key, string filePath, Stream val) : base(key, filePath)
        {
            _stream = val;
        }

        public override void Render(SKCanvas canvas)
        {
            var bitmap = LoadBitmap();
            if (bitmap != null)
            {
                var pts = DrawPoint(new Size(bitmap.Width, bitmap.Height));
                if (pts != null)
                {
                    using (var paint = new SKPaint())
                    {
                        paint.ColorFilter = SKColorFilter.CreateBlendMode(ForegroundColor, SKBlendMode.SrcIn);
                        canvas.DrawBitmap(
                            bitmap,
                            pts.ToSKPoint(),
                            paint
                        );
                    }
                }
            }
        }

        private SKBitmap LoadBitmap()
        {
            var keyParent = Key.Parent as Key;
            if (keyParent != null)
            {
                var imgSizeFactory = new ImageSizeFactory();
                var size = imgSizeFactory.CreateImageSize(keyParent);

                var loader = new ImageLoaderFactory().CreateImageLoader(Value);
                SKBitmap bitmap = null;

                if (_stream == null)
                {
                    bitmap = loader.LoadImage(Value, size);
                }
                else
                {
                    bitmap = loader.LoadImage(_stream, size);

                    _stream.Dispose();
                }

                return bitmap;
            }

            return null;
        }
    }
}
