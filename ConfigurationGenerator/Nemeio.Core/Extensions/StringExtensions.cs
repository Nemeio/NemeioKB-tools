using System.Collections.Generic;
using System.IO;

namespace Nemeio.Core.Extensions
{
    public static class StringExtensions
    {
        public static Stream GetStream(this string str)
        {
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);
            streamWriter.Write(str);
            streamWriter.Flush();
            stream.Position = 0;
            return stream;
        }

        public static string LeftOf(this string s, string c)
        {
            string ret = s;

            int idx = s.IndexOf(c);

            if (idx != -1)
            {
                ret = s.Substring(0, idx);
            }

            return ret;
        }

        public static string RightOf(this string s, string c)
        {
            string ret = string.Empty;
            int idx = s.IndexOf(c);
            
            if (idx != -1)
            {
                ret = s.Substring(idx + 1);
            }

            return ret;
        }

        public static string RightOfRightMostOf(this string src, string s)
        {
            string ret = src;
            int idx = src.IndexOf(s);
            int idx2 = idx;

            while (idx2 != -1)
            {
                idx2 = src.IndexOf(s, idx + s.Length);

                if (idx2 != -1)
                {
                    idx = idx2;
                }
            }

            if (idx != -1)
            {
                ret = src.Substring(idx + s.Length, src.Length - (idx + s.Length));
            }

            return ret;
        }
    }
}
