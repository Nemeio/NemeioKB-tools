using System.Drawing;
using Nemeio.LayoutGen.Extensions;
using SkiaSharp;

namespace Nemeio.LayoutGen.Models.Displayer
{
    public class TextDisplayer : BaseDisplayer
    {
        public TextDisplayer(Key key, string val) : base(key, val)
        {
            
        }

        public override void Render(SKCanvas canvas)
        {
            var fontPaint = Key.Font.ToSKPaint();
            fontPaint.Style = SKPaintStyle.Fill;
            fontPaint.Color = ForegroundColor;

            SKRect textBounds = new SKRect();
            if (!string.IsNullOrEmpty(Value))
            {
                fontPaint.MeasureText(Value, ref textBounds);
            }

            var pts = DrawPoint(new Size(textBounds.Width, textBounds.Height));
            if (pts != null)
            {
                pts = new Point(
                    pts.X,
                    pts.Y + textBounds.Height
                );
                canvas.DrawText(Value,
                    pts.ToSKPoint(),
                    fontPaint
                );
            }
        }
    }
}
