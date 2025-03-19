using System.Drawing;
using SkiaSharp;

namespace Nemeio.LayoutGen.Models
{
    public class Point
    {
        public Point()
        {

        }

        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; }

        public float Y { get; }

        public SKPoint ToSKPoint()
        {
            return new SKPoint(X, Y);
        }

        public static Point operator +(Point pt1, Point pt2)
        {
            var newPt = new Point(
                pt1.X + pt2.X,
                pt1.Y + pt2.Y
            );

            return newPt;
        }

        public static Point Zero => new Point(0, 0);

        public override bool Equals(object obj)
        {
            if (obj is Point pt)
            {
                return X == pt.X && Y == pt.Y;
            }

            return false;
        }
    }
}
