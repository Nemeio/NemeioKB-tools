using Nemeio.LayoutGen.Models;

namespace Nemeio.LayoutGen.Factory
{
    public class ImageSizeFactory
    {
        private const int DefaultMargin = 0;

        public Size CreateImageSize(Key key)
        {
            var size = Size.Zero;

            switch (key.Disposition)
            {
                case Disposition.FourSymbol:
                    size = new Size(key.Size.Width / 2, key.Size.Height / 2);
                    break;
                case Disposition.TwoSymbolHorizontal:
                    size = new Size(key.Size.Height / 2);
                    break;
                case Disposition.TwoSymbolVertical:
                    size = new Size(key.Size.Width / 2);
                    break;
                case Disposition.OnlyOneSymbol:
                    size = key.Size;
                    break;
                default:
                    size = Size.Zero;
                    break;
            }

            return size - DefaultMargin;
        }
    }
}
