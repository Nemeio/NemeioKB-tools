using System.Linq;
using Nemeio.LayoutGen.Exceptions;
using SkiaSharp;

namespace Nemeio.LayoutGen.Models
{
    public class Font
    {
        public const string DEFAULT_FONT_NAME = "Arial";
        public const int DEFAULT_FONT_SIZE = 16;

        public string Name { get; }

        public bool IsBold { get; }

        public bool IsItalic { get; }

        public bool IsUnderline { get;  }

        public int Size { get; }

        public Font(string name, bool bold, bool italic, bool underline, int size)
        {
            Name = name;
            IsBold = bold;
            IsItalic = italic;
            IsUnderline = underline;
            Size = size;
        }

        public SKPaint ToSKPaint()
        {
            if (!FontExists(Name))
            {
                throw new FontNotFoundException(Name);
            }

            var paint = new SKPaint();

            SKTypefaceStyle style = SKTypefaceStyle.Normal;

            if (IsBold)
            {
                style |= SKTypefaceStyle.Bold;
            }

            if (IsItalic)
            {
                style |= SKTypefaceStyle.Italic;
            }

            paint.Typeface = SKTypeface.FromFamilyName(Name, style);
            paint.TextSize = Size;

            return paint;
        }

        private bool FontExists(string fontName)
        {
            var fontsInstalled = SKFontManager.Default.GetFontFamilies();

            return fontsInstalled.Where(x => x.Equals(fontName)).Count() >= 1;
        }

        public static Font Default
        {
            get
            {
                return new Font(DEFAULT_FONT_NAME, false, false, false, DEFAULT_FONT_SIZE);
            }
        }
    }
}
