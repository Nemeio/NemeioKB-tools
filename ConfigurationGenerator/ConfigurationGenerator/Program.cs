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

        static void generateImageKey(KeyboardLayoutRenderer renderer, Size size, Font font, string imgPath, string cfgPath, string scriptPath)
        {
            if (!Directory.Exists(cfgPath))
            {
                Directory.CreateDirectory(cfgPath);
            }
            if (!Directory.Exists(scriptPath))
            {
                Directory.CreateDirectory(scriptPath);
            }

            var imgName = System.IO.Path.GetFileNameWithoutExtension(imgPath);

            bool bDarkBackground = imgName.Contains("blanc");

            var lyt = new Layout(
                    size,
                    Point.Zero,
                    true
                );            

            var key = new Key(
                    size,
                    Point.Zero,
                    bDarkBackground,
                    Disposition.OnlyOneSymbol,
                    font
                );

            var subkey = new Key(
                    size,
                    Point.Zero,
                    bDarkBackground,
                    Disposition.OnlyOneSymbol,
                    font);

            subkey.Displayer = new ImageDisplayer(subkey, imgPath);
            subkey.Modifier = Modifier.None;

            key.AddComponent(subkey);
            lyt.AddComponent(key);

            var bmp = renderer.Render(lyt);
            string fileName = string.Format("{0}\\{1}_rendering.png", cfgPath, imgName);
            saveBitmap(bmp, fileName);

            var wallpapper = renderer.ConvertToWallpapper(bmp);
            var compress = renderer.CompressImage(wallpapper);
            File.WriteAllBytes(string.Format("{0}\\{1}.wallpapper.gz", cfgPath, imgName), compress);

            var guid = Guid.NewGuid().ToString();
            var json = string.Format(@"{{""id"":""{0}"",""default"":""0"",""hid"":""1"",""mac"":""0"",""specialSequences"":{{""winSAS"":[[72,75,16],[75,72,16],[72,77,16],[77,72,16],[78,75,16],[75,78,16],[78,77,16],[77,78,16],[74,51]]}}}}", guid);
            System.IO.File.WriteAllText(string.Format("{0}\\{1}.json", cfgPath, imgName), json);

            var scriptSend = string.Format(@"set CONFIGNAME={0}

call ""%~dp0/../scripts/dl_config.bat""", imgName);
            System.IO.File.WriteAllText(string.Format("{0}\\send_{1}_config.bat", scriptPath, imgName), scriptSend);

            var scriptApply = string.Format(@"call ""%~dp0/../com_var.bat""

""%~dp0/../InstallTool.exe"" APPLYCONFIG {0}

          pause", guid);
            System.IO.File.WriteAllText(string.Format("{0}\\apply_{1}_config.bat", scriptPath, imgName), scriptApply);
        }

        static void Main(string[] args)
        {
            var size = new Size(1496, 624);
            var defaultFont = new Font("Arial", false, false, false, _defaultFontSize);
            var _keyboardLayoutRenderer = new KeyboardLayoutRenderer();
            var imgPath = args[0];
            var cfgPath = args[1];
            var scriptPath = args[2];

            generateImageKey(_keyboardLayoutRenderer, size, defaultFont, imgPath, cfgPath, scriptPath);
        }
    }
}
