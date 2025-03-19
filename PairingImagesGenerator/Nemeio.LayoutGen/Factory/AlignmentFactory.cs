using Nemeio.LayoutGen.Models;
using Nemeio.LayoutGen.Models.Alignment;
using Nemeio.LayoutGen.Models.Alignment.Text;

namespace Nemeio.LayoutGen.Factory
{
    public class AlignmentFactory
    {
        public IAlignment CreateAlignment(Key key)
        {
            switch (key.Disposition)
            {
                case Disposition.FourSymbol:
                    return new DefaultAlignment();
                case Disposition.TwoSymbolHorizontal:
                    return new HorizontalAligment();
                case Disposition.TwoSymbolVertical:
                    return new VerticalAligment();
                case Disposition.OnlyOneSymbol:
                    return new HorizontalVerticalAlignement();
                default:
                    return null;
            }
        }
    }
}
