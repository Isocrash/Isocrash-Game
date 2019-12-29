using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Hybridizer.Runtime.CUDAImports;

namespace Voxine
{
    public static class EQuaternion
    {
        private const double DegToRad = Math.PI/180D;

        [Kernel]
        public static Quaternion FromEuler(Vector3D angles)
        {
            return FromEuler(angles.x, angles.y, angles.z);
        }
        [Kernel]
        public static Quaternion FromEuler(double x, double y, double z)
        {
            //To radians
            x *= DegToRad;
            y *= DegToRad;
            z *= DegToRad;

            return Quaternion.CreateFromYawPitchRoll((float)y, (float)x, (float)z);
        }

        [Kernel]
        public static Vector3D ToEuler(this Quaternion q)
        {
            Vector3D angles;

            // roll (X-axis rotation)
            double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
            double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            angles.x = Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch (Y-axis rotation)
            double sinp = 2 * (q.W * q.Y - q.Z * q.X);
            if (Math.Abs(sinp) >= 1)
                angles.y = CopySign(Math.PI / 2, sinp); // use 90 degrees if out of range
            else
                angles.y = Math.Asin(sinp);

            // yaw (Z-axis rotation)
            double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            angles.z = Math.Atan2(siny_cosp, cosy_cosp);

            return angles * (180D / Math.PI);
        }

        public static Vector3D Direction(this Quaternion q)
        {
            Vector3D angles = q.ToEuler();

            Vector3D dir = new Vector3D();

            double xCos = Math.Sin(angles.x);
            double xSin = Math.Sin(angles.x);
            double yCos = Math.Cos(angles.y);
            double ySin = Math.Sin(angles.y);
            double zCos = Math.Cos(angles.z);
            double zSin = Math.Sin(angles.z);

            // x: yaw
            // y: pitch
            // z: roll

            dir.x = -xCos * ySin * zSin - xSin * zCos;
            dir.y = -xSin * ySin * zSin + xCos * zCos;
            dir.z = yCos * zSin;

            return dir.Normalize();
        }



        public static double CopySign(double x, double y)
        {
            //if x negative
            if(x < 0)
            {
                //if y negative
                if(y < 0)
                {
                    return x;
                }

                //else return positive x
                return x * -1;
            }

            //if y negative
            if(y < 0)
            {
                //return negative x
                return x * -1;
            }

            //else return positive x
            return x;
        }
    }
}
