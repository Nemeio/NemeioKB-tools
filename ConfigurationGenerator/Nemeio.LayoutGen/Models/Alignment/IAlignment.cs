using System.Drawing;

namespace Nemeio.LayoutGen.Models.Alignment
{
    public interface IAlignment
    {
        Point Calc(Key key, int index, Size size);

        bool CanDisplayKey(Modifier modifier);
    }
}
