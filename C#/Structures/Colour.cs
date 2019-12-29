using System;
using System.Drawing;

namespace Raymarcher
{
    [Serializable]
    public struct Colour
    {
        public byte this[int i]
        {
            get
            {
                return colour[i];
            }

            set
            {
                colour[i] = value;
            }
        }
        public byte R
        {
            get
            {
                return colour[0];
            }

            set
            {
                colour[0] = value;
            }
        }
        public byte G
        {
            get
            {
                return colour[1];
            }

            set
            {
                colour[1] = value;
            }
        }
        public byte B
        {
            get
            {
                return colour[2];
            }

            set
            {
                colour[2] = value;
            }
        }

        internal byte[] colour;
        
        public Colour(int r, int g, int b)
        {
            colour = new byte[3] { (byte)b, (byte)g, (byte)r };
        }
        public Colour(byte r, byte g, byte b)
        {
            colour = new byte[3] { b, g, r };
        }

        public byte[] ToByte()
        {
            return this.colour;
        }

        public static explicit operator Colour(Color col)
        {
            return new Colour(col.R, col.G, col.B);
        }
        public static explicit operator Color(Colour col)
        {
            return Color.FromArgb(col.colour[2], col.colour[1], col.colour[0]);
        }
    }
}
