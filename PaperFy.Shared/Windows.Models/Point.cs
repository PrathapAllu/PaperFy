using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PaperFy.Shared.Windows.Models
{
    public struct Point : IEquatable<Point>
    {
        public static Point Empty { get; } = new Point(0, 0);

        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Point other)
        {
            return this == other;
        }

        public override readonly bool Equals([NotNullWhen(true)] object obj)
        {
            if (obj is Point)
            {
                return (Point)obj == this;
            }
            return false;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static double Distance(Point first, Point second)
        {
            return Math.Sqrt(Math.Pow(second.X - first.X, 2.0) + Math.Pow(second.Y - first.Y, 2.0));
        }

        public static bool operator ==(Point left, Point right)
        {
            if (left.X == right.X)
            {
                return left.Y == right.Y;
            }
            return false;
        }

        public static bool operator !=(Point left, Point right)
        {
            return !(left == right);
        }
    }
}


