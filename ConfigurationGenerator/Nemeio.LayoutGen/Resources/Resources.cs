using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Nemeio.LayoutGen.Resources
{
    public class Resources
    {
        public static Stream GetResourceStream(string name)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            return asm.GetManifestResourceStream("Nemeio.LayoutGen.Resources.Images." + name);
        }
    }
}
