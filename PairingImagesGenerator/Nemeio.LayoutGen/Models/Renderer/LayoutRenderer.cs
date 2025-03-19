using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp;

namespace Nemeio.LayoutGen.Models.Renderer
{
    public class LayoutRenderer : BaseRenderer<Layout>
    {
        public override SKBitmap Render(Layout lyt)
        {
            float width = lyt.Size.Width;
            float height = lyt.Size.Height;

            SKImageInfo infos = new SKImageInfo((int)width, (int)height);
            using (var surface = SKSurface.Create(infos))
            {
                lyt.Render(surface.Canvas);

                SKImage snap = surface.Snapshot();
                SKBitmap bitmap = SKBitmap.FromImage(snap);

                return bitmap;
            }
        }
    }
}
