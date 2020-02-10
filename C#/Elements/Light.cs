using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
namespace Raymarcher
{
    public class Light : Module
    {
        public static Light Main { get; set; }

        public Colour32 Colour { get; set; } = new Colour32(255, 255, 255, 0);

        private Vector3D RotationAxis = new Vector3D(-1, 0, 0).Normalize();
        private double Angle = 0;
        private double DegPerSec = 5;

        static Colour32 SunDayColour = new Colour32(255, 255, 255, 255);
        static Colour32 SunSetColour = new Colour32(255, 85, 0, 255);
        static Colour32 NightColour = new Colour32(0, 0, 0, 255);

        protected internal override void FixedUpdate()
        {
            //Angle += DegPerSec * Time.FixedDeltaTime;
            Quaternion rot = Quaternion.CreateFromAxisAngle((Vector3)RotationAxis, (float)(Angle * 0.0174533D));
            this.Malleable.Rotation = rot;

            Vector3D sunDir = this.Malleable.Forward;

            double SunAltitude = 1D;
            bool negative = sunDir.Y < 0;
            SunAltitude = Math.Abs(sunDir.Y);

            if (!negative)
                this.Colour = RayMath.Lerp(SunSetColour, SunDayColour, Math.Sqrt(SunAltitude * 2));
            else
                this.Colour = RayMath.Lerp(SunSetColour, NightColour, SunAltitude * 10);
        }
    }
}
