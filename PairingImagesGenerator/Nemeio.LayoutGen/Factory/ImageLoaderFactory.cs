using System.IO;
using Nemeio.LayoutGen.Exceptions;
using Nemeio.LayoutGen.Models.Loader;

namespace Nemeio.LayoutGen.Factory
{
    public class ImageLoaderFactory
    {
        public IImageLoader CreateImageLoader(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            switch (extension)
            {
                case ".svg": return new SvgLoader();
                case ".png": return new PngLoader();
                default:
                    throw new ImageFormatNotSupportedException(extension);
            }
        }
    }
}
