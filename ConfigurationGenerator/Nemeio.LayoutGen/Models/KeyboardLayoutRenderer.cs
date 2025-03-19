using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.LayoutGen.Extensions;
using Nemeio.LayoutGen.Models.Renderer;
using SkiaSharp;

namespace Nemeio.LayoutGen.Models
{
    public class KeyboardLayoutRenderer
    {
        public KeyboardLayoutRenderer()
        {
            
        }

        public SKBitmap Render(ConfiguratorLayout configLyt)
        {
            return new JsonRenderer().Render(configLyt);
        }

        public SKBitmap Render(Layout lyt)
        {
            return new LayoutRenderer().Render(lyt);
        }

        public byte[] ConvertTo1Bpp(SKBitmap img)
        {
            const int bitPerPixel = 8;
            const int seuil = 140;
            const uint mask = 0x80;

            int width = img.Width;
            int height = img.Height;
            int index = width * height - 1;

            byte[] bytesArray = new byte[(width * height + bitPerPixel - 1) / bitPerPixel];

            for (int y = 0; y < height; y++) 
            {
                for (int x = 0; x < width; x++)
                {
                    SKColor pixel = img.GetPixel(x, y);
                    int value = (pixel.Red + pixel.Green + pixel.Blue) / 3;

                    if (value > seuil)
                    {
                        bytesArray[index / bitPerPixel] |= (byte) (mask >> (index % bitPerPixel));
                    }
                    else
                    {
                        bytesArray[index / bitPerPixel] &= (byte) ~(mask >> (index % bitPerPixel));
                    }

                    --index;
                }
            }

            return bytesArray;
        }

        public byte[] CompressImage(byte[] img)
        {   
            using (var compressedStream = new MemoryStream())
            using (GZipStream zipStream = new GZipStream(compressedStream, CompressionLevel.Optimal))
            {
                zipStream.Write(img, 0, img.Length);
                zipStream.Close();

                return compressedStream.ToArray();
            }
        }

        public byte[] ConvertToWallpapper(SKBitmap bmp)
        {
            var bpp = ConvertTo1Bpp(bmp);

            return bpp;
        }

        public void SaveToPng(SKBitmap bitmap, string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }

            using (var fileStream = new FileStream(filename, FileMode.CreateNew))
            {
                bitmap.ToPng().SaveTo(fileStream);
            }
        }
    }
}
