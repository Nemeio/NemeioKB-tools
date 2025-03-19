using System.Drawing;

namespace Nemeio.LayoutGen.Models.Alignment.Text
{
    public class HorizontalVerticalAlignement : IAlignment
    {
        public Point Calc(Key key, int index, Size size)
        {
            var sizeOfCell = key.Parent.Size.ToSKSize();

            var topMargin = sizeOfCell.Height / 2 - size.Height / 2;
            var leftMargin = sizeOfCell.Width / 2 - size.Width / 2;

            return new Point(leftMargin, topMargin);
        }

        public bool CanDisplayKey(Modifier modifier)
        {
            return modifier == Modifier.None;
        }
    }
}
