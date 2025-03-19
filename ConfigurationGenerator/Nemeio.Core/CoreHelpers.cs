using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nemeio.Core
{
    public static class CoreHelpers
    {
        public static List<T> AddChainable<T>(this List<T> list, T element)
        {
            list.Add(element);
            return list;
        }

        public static List<T> AddChainable<T>(this List<T> list, IEnumerable<T> elements)
        {
            list.AddRange(elements);
            return list;
        }

        public static async Task ForEachAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
        {
            foreach (var item in enumerable)
            {
                await action(item);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        public static byte[] Append(this byte[] source, byte[] toAppend)
        {
            var buffer = new byte[source.Length + toAppend.Length];
            Array.Copy(source, buffer, source.Length);
            Array.Copy(toAppend, 0, buffer, source.Length, toAppend.Length);
            return buffer;
        }

        static public string TraceBuffer(byte[] buffer, bool withText = false)
        {
            if (buffer.Length == 0)
            {
                return $"[Length=0]";
            }

            var strBytes = buffer.Select(l => l.ToString()).Aggregate((l1, l2) => $"{l1}, {l2}");
            var output = $"[Length={buffer.Length}]<{strBytes}>";

            if (withText)
            {
                var str = Encoding.UTF8.GetString(buffer);

                if (str.Any(char.IsLetterOrDigit))
                {
                    return output + $"{str}";
                }
            }

            return output;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}
