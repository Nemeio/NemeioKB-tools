using System.Drawing;
using Nemeio.LayoutGen.Extensions;
using SkiaSharp;

namespace Nemeio.LayoutGen.Models
{
    public class Layout : Component
    {
        public bool IsDarkBackground { get; } = false;

        public SKColor BackgroundColor => IsDarkBackground ? SKColors.Black : SKColors.White;

        public Layout(Size size, Point pos, bool darkBackground) : base(size, pos)
        {
            IsDarkBackground = darkBackground;
        }

        public override void Render(SKCanvas canvas)
        {
            canvas.DrawFilledRectangle(
                Position.X,
                Position.Y,
                Size.Width,
                Size.Height,
                BackgroundColor
            );
            foreach (var cpnt in Childrens)
            {
                cpnt.Render(canvas);
            }
        }
    }
}
