using System.Drawing;

namespace Nemeio.LayoutGen.Models.Alignment.Text
{
    public class DefaultAlignment : IAlignment
    {
        private const int DefaultMargin = 5;

        public Point Calc(Key key, int index, Size size)
        {
            float x = 0;
            float y = 0;

            if (key.Modifier == Modifier.None || key.Modifier == Modifier.AltGr)
            {
                y = key.Parent.Size.Height / 2;
            }

            if (key.Modifier == Modifier.Both || key.Modifier == Modifier.AltGr)
            {
                x = key.Parent.Size.Width / 2;
            }

            var sizeOfCell = key.Parent.Size.ToSKSize();
            sizeOfCell.Height = sizeOfCell.Height / 2;
            sizeOfCell.Width = sizeOfCell.Width / 2;

            var topMargin = (sizeOfCell.Height - size.Height) / 2;
            var leftMargin = (sizeOfCell.Width - size.Width) / 2;

            x += leftMargin;
            y += topMargin;

            var withMargin = ApplyMargin(x, y, key.Modifier);

            return withMargin;
        }

        public bool CanDisplayKey(Modifier modifier)
        {
            return true;
        }

        private Point ApplyMargin(float x, float y, Modifier modifier)
        {
            if (modifier == Modifier.Shift)
            {
                x += DefaultMargin;
                y += DefaultMargin;
            }
            else if (modifier == Modifier.Both)
            {
                x -= DefaultMargin;
                y += DefaultMargin;
            }
            else if (modifier == Modifier.None)
            {
                x += DefaultMargin;
                y -= DefaultMargin;
            }
            else
            {
                x -= DefaultMargin;
                y -= DefaultMargin;
            }

            return new Point(x, y);
        }
    }
}
