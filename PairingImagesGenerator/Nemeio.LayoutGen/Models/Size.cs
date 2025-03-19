using System.Drawing;
using SkiaSharp;

namespace Nemeio.LayoutGen.Models
{
    public class Size
    {
        public Size()
        {

        }

        public Size(float squareWidth)
        {
            Width = squareWidth;
            Height = squareWidth;
        }

        public Size(float width, float height)
        {
            Width = width;
            Height = height;
        }

        public float Width { get; }

        public float Height { get; }

        public SKSize ToSKSize()
        {
            return new SKSize(Width, Height);
        }

        public static Size operator /(Size size, float nb)
        {
            return new Size(size.Width / nb, size.Height / nb);
        }

        public static Size operator -(Size size, float nb)
        {
            return new Size(size.Width - nb, size.Height - nb);
        }

        public static Size Zero => new Size(0, 0);
    }
}
