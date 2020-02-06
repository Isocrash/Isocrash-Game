using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 

namespace Raymarcher
{
    public static class RayMath
    {
        #region Constants
        public const double DegreeToRadian = 0.01745D;
        public const double RadianToDegree = 57.2958D;
        #endregion

        #region Clamp
         
        public static double Clamp(double value, double min, double max)
        {
            if (double.IsNaN(value)) return double.NaN;

            if (value < min) return min;
            if (value > max) return max;

            return value;
        }

        public static byte Clamp(byte value, byte min, byte max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
         
        public static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        #endregion

        #region Lerp
        public static Colour256 Lerp(Colour256 a, Colour256 b, double t)
        {
            t = Clamp(t, 0.0D, 1.0D);

            return new Colour256(
                a.R + (b.R - a.R) * t,
                a.G + (b.G - a.G) * t,
                a.B + (b.B - a.B) * t,
                a.A + (b.A - a.A) * t
                );
        }

         
        public static Colour32 Lerp(Colour32 a, Colour32 b, double t)
        {
            t = Clamp(t, 0.0D, 1.0D);

            return new Colour32(
                (byte)(a.R + (b.R - a.R) * t),
                (byte)(a.G + (b.G - a.G) * t),
                (byte)(a.B + (b.B - a.B) * t),
                (byte)(a.A + (b.A - a.A) * t)
                );

            //return Lerp((Colour256)a, (Colour256)b, t);
        }
        #endregion

        #region Remap
        public static double Remap(double value, double oldLow, double oldHigh, double newLow, double newHigh)
        {
            return newLow + (value - oldLow) * (newHigh - newLow) / (oldHigh - oldLow);
        }
        #endregion
    }
}



