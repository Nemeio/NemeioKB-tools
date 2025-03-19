using System.Drawing;

namespace Nemeio.LayoutGen.Models.Alignment.Text
{
    public class HorizontalAligment : IAlignment
    {
        public Point Calc(Key key, int index, Size size)
        {
            float x = 0;
            float y = 0;

            if (key.Modifier != Modifier.None)
            {
                x = key.Parent.Size.Width / 2;
            }

            var sizeOfCell = key.Parent.Size.ToSKSize();
            sizeOfCell.Width = sizeOfCell.Width / 2;

            var topMargin = (sizeOfCell.Height - size.Height) / 2;
            var leftMargin = (sizeOfCell.Width - size.Width) / 2;

            x += leftMargin;
            y = topMargin;

            return new Point(x, y);
        }

        public bool CanDisplayKey(Modifier modifier)
        {
            return modifier == Modifier.None || modifier == Modifier.Shift;
        }
    }
}
