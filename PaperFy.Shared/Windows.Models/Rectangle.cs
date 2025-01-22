using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace PaperFy.Shared.Windows.Models
{
    public struct Rectangle : IEquatable<Rectangle>
    {
        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonIgnore]
        public readonly int Left => X;

        [JsonIgnore]
        public readonly int Top => Y;

        [JsonIgnore]
        public readonly int Right => X + Width;

        [JsonIgnore]
        public readonly int Bottom => Y + Height;

        public Rectangle()
        {
            int num2 = (Height = 0);
            int num4 = (Width = num2);
            int x = (Y = num4);
            X = x;
        }

        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public readonly bool Contains(int x, int y)
        {
            if (X <= x && x < X + Width && Y <= y)
            {
                return y < Y + Height;
            }
            return false;
        }

        public readonly bool Contains(Point pt)
        {
            return Contains(pt.X, pt.Y);
        }

        public readonly bool Contains(Rectangle rect)
        {
            if (X <= rect.X && rect.X + rect.Width <= X + Width && Y <= rect.Y)
            {
                return rect.Y + rect.Height <= Y + Height;
            }
            return false;
        }

        public override readonly bool Equals([NotNullWhen(true)] object obj)
        {
            if (obj is Rectangle)
            {
                return (Rectangle)obj == this;
            }
            return false;
        }

        public readonly bool Equals(Rectangle other)
        {
            return this == other;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(X, Y, Width, Height);
        }

        public static bool operator ==(Rectangle left, Rectangle right)
        {
            if (left.X == right.X && left.Y == right.Y && left.Width == right.Width)
            {
                return left.Height == right.Height;
            }
            return false;
        }

        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return !(left == right);
        }
    }
}


