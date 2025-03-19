using System.Drawing;
using SkiaSharp;

namespace Nemeio.LayoutGen.Models.Displayer
{
    public interface IDisplayer
    {
        string Value { get; }

        Key Key { get; }

        void Render(SKCanvas canvas);
    }
}
