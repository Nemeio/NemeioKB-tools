using Nemeio.LayoutGen.Factory;
using SkiaSharp;
using System.Diagnostics;
using System.Drawing;

namespace Nemeio.LayoutGen.Models.Displayer
{
    public abstract class BaseDisplayer : IDisplayer
    {
        private readonly string _value;

        private readonly Key _key;

        private AlignmentFactory _factory;

        protected BaseDisplayer(Key key, string val)
        {
            _key = key;
            _value = val;

            _factory = new AlignmentFactory();
        }

        public string Value => _value;

        public Key Key => _key;

        public abstract void Render(SKCanvas canvas);

        protected int SubkeyIndex => Key.Parent.Childrens.IndexOf(Key);

        protected Point DrawPoint(Size itemSize)
        {
            Point pts = null;

            var keyParent = Key.Parent as Key;

            if (keyParent != null)
            {
                var alignement = _factory.CreateAlignment(keyParent);
                if (alignement != null)
                {
                    if (!alignement.CanDisplayKey(Key.Modifier))
                    {
                        return pts;
                    }

                    pts = alignement.Calc(Key, SubkeyIndex, itemSize);

                    if (pts != null)
                    {
                        pts = Key.Position + pts;
                    }
                }
            }
            else
            {
                pts = Key.Position;
            }

            return pts;
        }

        protected SKColor BackgroundColor
        {
            get
            {
                var keyParent = Key.Parent as Key;
                if (keyParent != null)
                {
                    return keyParent.IsDarkBackground ? SKColors.Black : SKColors.White;
                }

                return SKColors.White;
            }
        }

        protected SKColor ForegroundColor
        {
            get
            {
                var keyParent = Key.Parent as Key;
                if (keyParent != null)
                {
                    return keyParent.IsDarkBackground ? SKColors.White : SKColors.Black;
                }

                return SKColors.Black;
            }
        }
    }
}
