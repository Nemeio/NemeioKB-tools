using Nemeio.LayoutGen.Models;
using Nemeio.LayoutGen.Models.Displayer;
using System;
using SkiaSharp;
using System.Reflection;
using System.IO;

namespace PairingImagesGenerator
{
    class Program
    {
        private const int _defaultFontSize = 32;
        private const int digitheight = 32;
        private const int digitwidth = 24;

        static void saveBitmap(SKBitmap bmp, string name)
        {
            var dir = Directory.GetCurrentDirectory();

            // create an image and then get the PNG (or any other) encoded data
            using (var image = SKImage.FromBitmap(bmp))
            using (var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100))
            {
                // save the data to a stream
                var path = name;
                using (var stream = File.OpenWrite(path))
                {
                    data.SaveTo(stream);
                }
            }
        }

        static void generateImageKey(KeyboardLayoutRenderer renderer, Size size, Font font, string svgFileName, string keyText)
        {
            var lyt = new Layout(
                    size,
                    Point.Zero,
                    false
                );

            var key = new Key(
                    size,
                    Point.Zero,
                    false,
                    Disposition.OnlyOneSymbol,
                    font
                );

            var subkey = new Key(
                    Size.Zero,
                    Point.Zero,
                    false,
                    Disposition.OnlyOneSymbol,
                    font);

            subkey.Displayer = new ImageDisplayer(subkey, svgFileName);
            subkey.Modifier = Modifier.None;
            key.AddComponent(subkey);

            lyt.AddComponent(key);
            var bmp = renderer.Render(lyt);
            string fileName = string.Format("key{0}.png", keyText);
            saveBitmap(bmp, fileName);
            var wallpapper = renderer.ConvertToWallpapper(bmp);
            displaySimpleKeyBuffer(wallpapper, keyText);
        }

        static void displaySimpleKeyBuffer(byte[] buffer, string keyText)
        {
            Console.Write("const uint8_t BLEPasskeyDisplayer::key{0}[KEY_RSP_BUFFER_SIZE] = {{", keyText);
            foreach (var wpbyte in buffer)
            {
                Console.Write("0x{0:X2}, ", wpbyte);
            }
            Console.WriteLine("};");
        }

        static void displayDigitsBufferStart()
        {
            Console.Write("const uint8_t BLEPasskeyDisplayer::keyNum[NB_DIGITS][KEY_DIGITS_BUFFER_SIZE]  = {\r\n");
        }

        static void displayDigitsBufferDigit(byte[] buffer)
        {
            Console.Write("{");
            foreach (var wpbyte in buffer)
            {
                Console.Write("0x{0:X2}, ", wpbyte);
            }
            Console.WriteLine("},");
        }

        static void displayDigitsBufferEnd()
        {
            Console.Write("};\r\n");
        }

        static void Main(string[] args)
        {
            var size = new Size(digitwidth, digitheight);

            var lyt = new Layout(
                    size,
                    Point.Zero,
                    false
                );

            var _keyboardLayoutRenderer = new KeyboardLayoutRenderer();
            var defaultFont = new Font("Arial", false, false, false, _defaultFontSize);

            displayDigitsBufferStart();

            for (int i = 0; i < 10; i++)
            {
                var keyText = i.ToString();

                var key = new Key(
                    size,
                    Point.Zero,
                    false,
                    Disposition.OnlyOneSymbol,
                    defaultFont
                );

                var subkey = new Key(
                    Size.Zero,
                    Point.Zero,
                    false,
                    Disposition.OnlyOneSymbol,
                    defaultFont);

                subkey.Displayer = new TextDisplayer(subkey, keyText);
                subkey.Modifier = Modifier.None;
                key.AddComponent(subkey);

                lyt.AddComponent(key);
                var bmp = _keyboardLayoutRenderer.Render(lyt);
                string fileName = string.Format("key{0}.png", keyText);
                saveBitmap(bmp, fileName);
                var wallpapper = _keyboardLayoutRenderer.ConvertToWallpapper(bmp);
                displayDigitsBufferDigit(wallpapper);
            }
            displayDigitsBufferEnd();

            var okSize = new Size(32, 32);
            var nokSize = new Size(32, 32);
            generateImageKey(_keyboardLayoutRenderer, okSize, defaultFont, "ok.svg", "OK");
            generateImageKey(_keyboardLayoutRenderer, nokSize, defaultFont, "ko.svg", "NOK");
        }
    }
}
