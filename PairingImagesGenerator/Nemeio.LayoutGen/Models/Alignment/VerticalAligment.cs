using System.Drawing;

namespace Nemeio.LayoutGen.Models.Alignment.Text
{
    public class VerticalAligment : IAlignment
    {
        private int DefaultMargin = 5;

        public Point Calc(Key key, int index, Size size)
        {
            float x = 0;
            float y = 0;

            if (key.Modifier != Modifier.Shift)
            {
                y = key.Parent.Size.Height / 2;
            }

            var sizeOfCell = key.Parent.Size.ToSKSize();
            sizeOfCell.Height = sizeOfCell.Height / 2;

            var topMargin = (sizeOfCell.Height - size.Height) / 2;
            var leftMargin = (sizeOfCell.Width - size.Width) / 2;

            x = leftMargin;
            y += topMargin;

            return ApplyMargin(x, y, key.Modifier);
        }

        private Point ApplyMargin(float x, float y, Modifier modifier)
        {
            if (modifier != Modifier.Shift)
            {
                y += DefaultMargin;
            }
            else
            {
                y -= DefaultMargin;
            }

            return new Point(x, y);
        }

        public bool CanDisplayKey(Modifier modifier)
        {
            return modifier == Modifier.None || modifier == Modifier.Shift;
        }
    }
}
