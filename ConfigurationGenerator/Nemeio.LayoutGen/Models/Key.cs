using System.Drawing;
using Nemeio.LayoutGen.Extensions;
using Nemeio.LayoutGen.Models.Displayer;
using SkiaSharp;

namespace Nemeio.LayoutGen.Models
{
    public enum Disposition
    {
        None = 0,
        OnlyOneSymbol = 1,
        TwoSymbolHorizontal = 2,
        TwoSymbolVertical = 3,
        FourSymbol = 4
    }

    public enum Modifier
    {
        None = 0,
        Shift = 1,
        AltGr = 2,
        Both = 3
    }

    public class Key : Component
    {
        public Key() : base(Size.Zero, Point.Zero)
        {

        }

        public Key(Size size, Point pos, bool darBackground = false, Disposition disp = Disposition.None, Font fnt = null) : base(size, pos)
        {
            IsDarkBackground = darBackground;
            Disposition = disp;
            Font = fnt;
        }

        public bool IsDarkBackground { get; } = false;

        public SKColor BackgroundColor => IsDarkBackground ? SKColors.Black : SKColors.White ;

        public Disposition Disposition { get; }

        public Font Font { get; }

        public Modifier Modifier { get; set; }

        public override void Render(SKCanvas canvas)
        {
            if (Size.Equals(Size.Zero))
            {
                return;
            }
            canvas.DrawFilledRectangle(
                Position.X,
                Position.Y,
                Size.Width,
                Size.Height,
                BackgroundColor
            );
            Displayer?.Render(canvas);
            foreach (var item in Childrens)
            {
                item.Render(canvas);
            }
        }
    }
}
