using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hybridizer.Runtime.CUDAImports;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Windows.Forms;


namespace Raymarcher
{
    public class GPUCamera
    {
        #region old1
        public Vector3D Position;
        public Vector2D ClipPlanes;
        public double FOV;
        public Vector2I Resolution;
        public byte[] ClearColour;
        public Vector3D SunDirection;
        public double Precision;
        public Quaternion Rotation;
        public Vector3D Dir;

        public GPUCamera() { }
        public GPUCamera(Camera cam)
        {
            this.Position = cam.Malleable.Position;
            this.ClipPlanes = cam.ClipPlanes;
            this.FOV = cam.FieldOfView;
            this.Resolution = Graphics.GetRenderResolution();
            this.ClearColour = cam.ClearColour.ToByte();
            this.SunDirection = new Vector3D(1, 1, 0);
            this.Precision = cam.Precision;
            this.Rotation = cam.Malleable.Rotation;
            Dir = new Vector3D();

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

        #endregion

        public static byte[] Render(Camera cam)
        {
            Vector2I resolution = Graphics.GetRenderResolution();

            byte[] render = new byte[resolution.x * resolution.y * 3];

            List<GPUSphere> spheres = new List<GPUSphere>();
            foreach (Element e in Element._LoadedElements)
            {
                Sphere s = e as Sphere;

                if (s != null) spheres.Add(new GPUSphere(s.Position, s.Scale.z / 2D, s.Colour));
            }
            int n = spheres.Count;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            wrapped.GPUOpti(
                render,
                spheres.ToArray(),
                n,
                new GPUCamera(cam)
                );
            sw.Stop();

            cuda.DeviceSynchronize();
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


        public struct Pixel
        {
            public byte this[int index]
            {
                get
                {
                    switch(index)
                    {
                        case 0: return r;
                        case 1: return g;
                        case 2: return b;
                        default: return 0;
                    }
                }

                set
                {
                    if (index == 0) r = value;
                    else if (index == 1) g = value;
                    else if (index == 2) b = value;
                }
            }

            public byte r;
            public byte g;
            public byte b;

            public Pixel(byte r, byte g, byte b)
            {
                this.r = r;
                this.g = g;
                this.b = b;
            }

            public Pixel(byte[] rgb)
            {
                r = rgb[0];
                g = rgb[1];
                b = rgb[2];
            }
        }

        [EntryPoint]
        public static void GPUOpti(byte[] pixels, GPUSphere[] spheres, int n, GPUCamera camera)
        {
            for (int y = threadIdx.y + blockIdx.y * blockDim.y; y < camera.Resolution.y; y += blockDim.y * gridDim.y)
            {
                for (int x = threadIdx.x + blockIdx.x * blockDim.x; x < camera.Resolution.x; x += blockDim.x * gridDim.x)
                {
                    int index = x + camera.Resolution.x * y;

                    Pixel pix = new Pixel(camera.ClearColour);

                    GPUHitInfos infos =
                        Hit(camera.Position,
                        GetDir(camera, new Vector2I() { x = x, y = y }), spheres, n, camera);

                    if (infos.HasHit)
                    {
                        //Angle
                        double angle = Angle(infos.Normal, camera.SunDirection);

                        if (angle < 90D)
                        {
                            GPUHitInfos shadow =
                            Hit(infos.Point + infos.Normal * camera.Precision, camera.SunDirection, spheres, n, camera);

                            if (!shadow.HasHit)
                            {
                                double light = Math.Sin((angle + 90) * 0.0174533D);

                                pix[0] = (byte)(light * infos.Colour[0]);
                                pix[1] = (byte)(light * infos.Colour[1]);
                                pix[2] = (byte)(light * infos.Colour[2]);
                            }

                            else
                            {
                                pix[0] = 0;
                                pix[1] = 0;
                                pix[2] = 0;
                            }

                        }

                        else
                        {
                            pix[0] = 0;
                            pix[1] = 0;
                            pix[2] = 0;
                        }
                    }

                    pixels[index * 3] = pix[0];
                    pixels[index * 3 + 1] = pix[1];
                    pixels[index * 3 + 2] = pix[2];
                }
            }
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
            // https://www.scratchapixel.com/lessons/3d-basic-rendering/ray-tracing-generating-camera-rays/generating-camera-rays?http://www.scratchapixel.com/lessons/3d-basic-rendering/3d-viewing-pinhole-camera/how-pinhole-camera-works-part-1

            // Original pixel position in Raster space (Unnormalized screen space)

            // Normalized Device Coordinates of pixel
            Vector2D PixelNDC = new Vector2D();
            PixelNDC.x = (pixel.x + 0.5D) / cam.Resolution.x;
            PixelNDC.y = (pixel.y + 0.5D) / cam.Resolution.y;

            Vector2D PixelScreen = new Vector2D();
            PixelScreen.x = 2D * PixelNDC.x - 0.5D;
            PixelScreen.y = 2D * PixelNDC.y - 0.5D;

            double ImageAspectRatio = (double)cam.Resolution.x / cam.Resolution.y;

            Vector2D PixelCamera = new Vector2D();
            PixelCamera.x = (2D * PixelScreen.x - 1D) * ImageAspectRatio * Tan(cam.FOV / 2D * Math.PI / 180D);
            PixelCamera.y = (1D - 2D * PixelScreen.y) * Tan(cam.FOV / 2D * Math.PI / 180D);

            // Position of pixel relative to camera
            Vector3D PcameraSpace = new Vector3D(PixelCamera.x, PixelCamera.y, 1D);

            return (cam.Rotation * PcameraSpace).Direction;

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
        #region folder
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
                    hit.Normal = GetSphereNormal(probe, spheres[closest].position);
                    hit.HasHit = true;

                    return hit;
                }

                probe += dir * dst;
                rayDst += dst;
            }

            

            return hit;
        }

        [Kernel]
        public static double DistanceToBox(Vector3D point, Vector3D centre, Vector3D size)
        {
            Vector3D o = Vector3D.Abs(point - centre) - size;
            double ud = Length(Vector3D.Max(o, Vector3D.Null));
            double n = Max(Max(Min(o.x, 0), Min(o.y,0)), Min(o.z, 0));

            return ud + n;
        }

        [Kernel]
        public static double Min(double a, double b)
        {
            if (a < b)
            {
                return a;
            }

            return b;
        }

        [Kernel]
        public static double Max(double a, double b)
        {
            if(a > b)
            {
                return a;
            }

            return b;
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
        public static double Length(Vector2D v)
        {
            return HybMath.Sqrt(v.x * v.x + v.y * v.y);
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
        public static double DistanceToSphere(Vector3D p, Vector3D c, double radius)
        {
            return HybMath.Sqrt((c.x - p.x) * (c.x - p.x) + (c.y - p.y) * (c.y - p.y) + (c.z - p.z) * (c.z - p.z)) - radius;
        }
        #endregion

        private static dynamic wrapped;

        [EngineInitializer(321)]
        public static void Initialize()
        {
            cudaDeviceProp prop;
            cuda.GetDeviceProperties(out prop, 0);

            if(!cuda.IsCudaAvailable())
            {
                DialogResult dr = MessageBox.Show("CUDA is not available, abording launch.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if(dr == DialogResult.OK)
                {
                    Application.Exit();
                }
            }

            //BLOCK SIZE: ALWAYS MULTIPLE OF 32
            //ex: 32 * 1 * 1, 32 * x * y...
            int props = prop.multiProcessorCount;

            dim3 threadsPerBlock = new dim3(8,8,1);
            dim3 numBlocks = new dim3(1024 / threadsPerBlock.x, 1024 / threadsPerBlock.y, 1);
            HybRunner runner = HybRunner.Cuda("Raymarcher_CUDA.dll").SetDistrib(numBlocks, threadsPerBlock);
            wrapped = runner.Wrap(new GPUCamera());
        }

        private static GPUWritter GPUOutput;
    }

    public class GPUWritter : TextWriter
    {
        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(string value)
        {
            Log.InstantPrint(value);
            //base.Write(value);
        }
    }
}
