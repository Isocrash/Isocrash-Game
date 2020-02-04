using System;
using System.Drawing;

namespace Raymarcher
{
    [Serializable]
    public struct Colour256
    {
        public const double MinValue = 0.0D;
        public const double MaxValue = 1.0D;

        public double R { get; private set; }
        public double G { get; private set; }
        public double B { get; private set; }
        public double A { get; private set; }

        public Colour256(double r, double g, double b, double a)
        {
            this.R = RayMath.Clamp(r, MinValue, MaxValue);
            this.G = RayMath.Clamp(g, MinValue, MaxValue);
            this.B = RayMath.Clamp(b, MinValue, MaxValue);
            this.A = RayMath.Clamp(a, MinValue, MaxValue);
        }

        public Colour256(Colour32 colour)
        {
            this.R = (double)colour.R / Colour32.MaxValue;
            this.G = (double)colour.G / Colour32.MaxValue;
            this.B = (double)colour.B / Colour32.MaxValue;
            this.A = (double)colour.A / Colour32.MaxValue;
        }

        public static implicit operator Colour256(Colour32 colour)
        {
            return new Colour256(colour);
        }

        public static implicit operator Colour256(Color colour)
        {
            return new Colour256(colour);
        }

        public static Colour256 operator *(Colour256 c, double v)
        {
            return new Colour256(c.R * v, c.G * v, c.B * v, c.A * v);
        }

        /*public override string ToString()
        {
            return $"Colour256({},{},{},{})";
        }*/
    }

    public struct Colour32
    {
        public const byte MinValue = Byte.MinValue;
        public const byte MaxValue = Byte.MaxValue;

        public byte R { get; private set; }
        public byte G { get; private set; }
        public byte B { get; private set; }
        public byte A { get; private set; }

        public byte this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0: return R;
                    case 1: return G;
                    case 2: return B;
                    case 3: return A;
                    default: return 0;
                }
            }
        }

        public Colour32(byte r, byte g, byte b, byte a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }
        public Colour32(int r, int g, int b, int a)
        {
            this.R = (byte)RayMath.Clamp(r, MinValue, MaxValue);
            this.G = (byte)RayMath.Clamp(g, MinValue, MaxValue);
            this.B = (byte)RayMath.Clamp(b, MinValue, MaxValue);
            this.A = (byte)RayMath.Clamp(a, MinValue, MaxValue);
        }
        public Colour32(Colour256 colour)
        {
            this.R = (byte)(colour.R * MaxValue);
            this.G = (byte)(colour.G * MaxValue);
            this.B = (byte)(colour.B * MaxValue);
            this.A = (byte)(colour.A * MaxValue);
        }

        public Colour32(Color colour)
        {
            this.R = colour.R;
            this.G = colour.G;
            this.B = colour.B;
            this.A = colour.A;
        }

        public static Colour32 Combine(Colour32 a, Colour32 b)
        {
            Colour256 c0 = a;
            Colour256 c1 = b;

            double a01 = (1D - c0.A) * c1.A + c0.A;

            return new Colour256(
                ((1D - c0.A) * c1.A * c1.R + c0.A * c0.R) / a01,
                ((1D - c0.A) * c1.A * c1.G + c0.A * c0.G) / a01,
                ((1D - c0.A) * c1.A * c1.B + c0.A * c0.B) / a01,
                a01
            );
        }

        public static implicit operator Colour32(Colour256 colour)
        {
            return new Colour32(colour);
        }

        public static implicit operator Colour32(Color colour)
        {
            return new Colour32(colour);
        }

        public static implicit operator byte[](Colour32 colour)
        {
            return new byte[3] { colour.A, colour.G, colour.R };
        }
    }

}
