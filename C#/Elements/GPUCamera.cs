using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hybridizer.Runtime.CUDAImports;
using System.Diagnostics;
using System.IO;
using System.Numerics;

namespace Voxine
{
    public class GPUCamera
    {
        public Vector3D Position;
        public Vector2D ClipPlanes;
        public double FOV;
        public Vector2I Resolution;
        public byte[] ClearColour;
        public Vector3D SunDirection;
        public double Precision;
        public Quaternion Rotation;

        public GPUCamera() { }
        public GPUCamera(Camera cam)
        {
            this.Position = cam.Position;
            this.ClipPlanes = cam.ClipPlanes;
            this.FOV = cam.FieldOfView;
            this.Resolution = Graphics.GetRenderResolution();
            this.ClearColour = cam.ClearColour.ToByte();
            this.SunDirection = new Vector3D(1, 1, 0);
            this.Precision = cam.Precision;
            this.Rotation = cam.Rotation;

        }

        public struct GPUHitInfos
        {
            public bool HasHit;
            public Vector3D Normal;
            public Vector3D Point;
            public Colour Colour;
            //public byte[] Colour;
        }

        public class SceneInfos
        {
            public Vector3D Normal;
        }

        public static byte[] Render(Camera cam)
        {
            Vector2I resolution = Graphics.GetRenderResolution();

            byte[] render = new byte[resolution.x * resolution.y * 3];

            List<GPUSphere> spheres = new List<GPUSphere>();
            foreach (Element e in Sandbox.LoadedElements)
            {
                Sphere s = e as Sphere;

                if (s != null) spheres.Add(new GPUSphere(s.Position, s.Scale.z / 2D, s.Colour));
            }
            int n = spheres.Count;
            Stopwatch sw = new Stopwatch();

            sw.Start();
            wrapped.GPUPixels(
                render,
                spheres.ToArray(),
                n,
                new GPUCamera(cam)
                );
            sw.Stop();
            Log.Print("GPU Render time: " + sw.ElapsedMilliseconds + "ms ");
            return render;
        }
        public class GPUSphere
        {
            public Vector3D position;
            public double radius;
            public Colour colour;
            public GPUSphere(Vector3D pos, double rad, Colour col)
            {
                radius = rad;
                position = pos;
                colour = col;
            }
        }

        [EntryPoint]
        public static void GPUPixels(byte[] pixels, GPUSphere[] spheres, int n, GPUCamera camera)
        {
            byte[] clear = camera.ClearColour;
            Parallel.For(0, camera.Resolution.x * camera.Resolution.y, i =>
            {
            int xpct = i % camera.Resolution.x;
            int ypct = camera.Resolution.y - (i / camera.Resolution.x);

            GPUHitInfos infos =
                Hit(camera.Position,
                GetDir(camera, new Vector2I() { x = xpct, y = ypct }), spheres, n, camera);

                if (infos.HasHit)
                {
                    pixels[i * 3] = 0;
                    pixels[i * 3 + 1] = 0;
                    pixels[i * 3 + 2] = 0;

                    //double angle = Vector3D.Angle(infos.Normal, camera.SunDirection); //Math.Acos(1);//Angle(infos.Normal, new Vector3D() { x = 1, y = 1, z = 0 });

                    //Angle
                    double angle = Angle(infos.Normal, camera.SunDirection);

                    if (angle < 90D)
                    {
                        GPUHitInfos shadow =
                        Hit(infos.Point + infos.Normal * camera.Precision, camera.SunDirection, spheres, n, camera);
                        
                        if(!shadow.HasHit)
                        {
                            double light = Math.Sin((angle + 90) * 0.0174533D);

                            pixels[i * 3] = (byte)(light * infos.Colour[0]);
                            pixels[i * 3 + 1] = (byte)(light * infos.Colour[1]);
                            pixels[i * 3 + 2] = (byte)(light * infos.Colour[2]);
                        }
                        
                    }

                    else
                    {
                        pixels[i * 3] = 0;
                        pixels[i * 3 + 1] = 0;
                        pixels[i * 3 + 2] = 0;
                    }
                }

                else
                {
                    pixels[i * 3] = camera.ClearColour[0];
                    pixels[i * 3 + 1] = camera.ClearColour[1];
                    pixels[i * 3 + 2] = camera.ClearColour[2];
                }
            });
        }
        [Kernel]
        public static double Angle(Vector3D v1, Vector3D v2)
        {
            return Math.Acos(Scalar(v1, v2) / (Length(v1) * Length(v2))) * 180D / Math.PI;
        }
        [Kernel]
        public static float Angle(float3 v1, float3 v2)
        {
            return (float)Math.Acos(Scalar(v1, v2) / (Length(v1) * Length(v2))) * 180F / (float)Math.PI;
        }
        [Kernel]
        public static double Scalar(Vector3D u, Vector3D v)
        {
            return u.x * v.x + u.y * v.y + u.z * v.z;
        }
        [Kernel]
        public static float Scalar(float3 u, float3 v)
        {
            return u.x * v.x + u.y * v.y + u.z * v.z;
        }

        [Kernel]
        public static Vector3D GetDir(GPUCamera cam, Vector2I pixel)
        {
            double hFOV = cam.FOV;

            double aspectRatio = (double)cam.Resolution.x / cam.Resolution.y;
            double vFOV = hFOV * (1D/aspectRatio);

            double pctX = (float)pixel.x / cam.Resolution.x;
            double pctY = (float)pixel.y / cam.Resolution.y;

            double yAngle = hFOV * pctX - (hFOV / 2F);
            double xAngle = vFOV * pctY - (vFOV / 2F);

            //Cam direction
            //Vector3D direction = cam.Rotation * Vector3D.Forward;

            Vector3D direction = EQuaternion.FromEuler(0, yAngle, 0) * Vector3D.Forward;
            return EQuaternion.FromEuler(-xAngle, 0, 0) * direction;
            //Vector3D direction = RotateAroundY(new Vector3D { x = 0, y = 0, z = 1F }, yAngle);
            //return RotateAroundX(direction, -xAngle);
        }
        #region Math Utility
        [Kernel]
        public static double Acos(double a)
        {
            return 1D / HybMath.Cos(a);
        }

        [Kernel]
        public static float Acos(float a)
        {
            return 1F / HybMath.Cos(a);
        }

        [Kernel]
        public static double Tan(double a)
        {
            return HybMath.Sin(a) / HybMath.Cos(a);
        }
        [Kernel]
        public static double Atan(double a)
        {
            return 1 / Tan(a);//a / HybMath.Sqrt(1 + a * a);
        }

        [Kernel]
        public static Vector3D RotateAroundX(Vector3D dir, double a)
        {
            a *= 0.0174533D;

            double rx = dir.x;
            double ry = dir.y * HybMath.Cos(a) - dir.z * HybMath.Sin(a);
            double rz = dir.y * HybMath.Sin(a) + dir.z * HybMath.Cos(a);

            return new Vector3D() { x = rx, y = ry, z = rz };
        }

        [Kernel]
        public static Vector3D RotateAroundY(Vector3D dir, double a)
        {
            a *= 0.0174533F;

            double rx = dir.x * HybMath.Cos(a) + dir.z * HybMath.Sin(a);
            double ry = dir.y;
            double rz = -dir.x * HybMath.Sin(a) + dir.z * HybMath.Cos(a);

            return new Vector3D() { x = rx, y = ry, z = rz };
        }
        #endregion

        [Kernel]
        public static GPUHitInfos Hit(Vector3D point, Vector3D dir, GPUSphere[] spheres, int n, GPUCamera camera)
        {
            Vector3D probe = point;

            double rayDst = 0D;
            double maxDst = camera.ClipPlanes.y;

            GPUHitInfos hit = new GPUHitInfos() { HasHit = false };

            while (rayDst < maxDst)
            {
                //double dst = Element.ClosestDistance(probe);
                double dst = ClosestDistance(probe, spheres, n);

                if (dst < camera.Precision)
                {
                    hit.Point = probe;
                    int closest = GetClosestIndex(probe, spheres, n);
                    hit.Colour = spheres[closest].colour;
                    hit.Normal = GetSphereNormal(probe, spheres[closest].position);//GetSphereNormal(probe, GetClosestPosition(probe, spheres, n));//new Vector3D() { x= 0, y= 0, z = 1});//GetClosestPosition(probe, spheres, n));
                    hit.HasHit = true;

                    return hit;
                }

                probe += dir * dst;
                rayDst += dst;
            }

            

            return hit;
        }

        [Kernel]
        public static int GetClosestIndex(Vector3D point, GPUSphere[] spheres, int n)
        {
            if (n == 0) return 0;
            int closestIndex = 0;
            double closestDistance = 100F;

            for (int i = 0; i < n; i++)
            {
                double dst = DistanceToSphere(point, spheres[i].position, spheres[i].radius);

                if(dst < closestDistance)
                {
                    closestDistance = dst;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }

        [Kernel]
        public static Vector3D GetSphereNormal(Vector3D point, Vector3D spherePos)
        {
            //Converts sphere as unit sphere
            return Direction(point - spherePos);
        }

        [Kernel]
        public static Vector3D Direction(Vector3D v)
        {
            double l = Length(v);
            return new Vector3D() { x = v.x / l, y = v.y / l, z = v.z / l };
        }

        [Kernel]
        public static double ClosestDistance(Vector3D pos, GPUSphere[] spheres, int n)
        {
            double minDist = 100F;

            for (int i = 0; i < n; i++)
            {
                double dst = DistanceToSphere(pos, spheres[i].position, spheres[i].radius);

                if (dst < minDist) minDist = dst;
            }

            return minDist;
        }

        [Kernel]
        public static double Length(Vector3D v)
        {
            return HybMath.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
        }
        [Kernel]
        public static double Length(float3 v)
        {
            return HybMath.Sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
        }


        [Kernel]
        public static double DistanceToSphere(Vector3D p, Vector3D centre, double radius)
        {
            return Length(new Vector3D() { x = centre.x - p.x, y = centre.y - p.y, z = centre.z - p.z }) - radius;
        }

        private static dynamic wrapped;

        [EngineInitializer(321)]
        public static void Initialize()
        {
            cudaDeviceProp prop;
            cuda.GetDeviceProperties(out prop, 0);

            string s = "";
            foreach (var item in prop.name)
            {
                s += item;
            }
            Log.InstantPrint(
                cuda.IsCudaAvailable()
                );

            //BLOCK SIZE: ALWAYS MULTIPLE OF 32
            //ex: 32 * 1 * 1, 32 * x * y...
            int props = prop.multiProcessorCount;
            HybRunner runner = HybRunner.Cuda("Raymarcher_CUDA.dll").SetDistrib(props, 512);//128,128,32,32,1, 0);//8, 8, 16, 16, 1, 0);
            wrapped = runner.Wrap(new GPUCamera());
        }
    }
}
