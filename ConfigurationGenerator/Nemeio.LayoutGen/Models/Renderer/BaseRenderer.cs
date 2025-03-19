using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nemeio.LayoutGen.Models.Renderer
{
    public abstract class BaseRenderer<T>
    {
        public abstract SKBitmap Render(T configLyt);
    }
}
