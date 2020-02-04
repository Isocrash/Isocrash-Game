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
using System.Drawing;


namespace Raymarcher
{
    public class GPUCamera
    {
        #region old1
        public Vector3D Position; //ok - camsetting
        public Vector2D ClipPlanes; //ok mais pas near - camsetting
        public double FOV; //ok - camsetting
        public Vector2I Resolution; //ok - camsetting
        public Colour32 ClearColour; //ok - camsetting
        public Vector3D SunDirection; //ok - lightsetting
        public Colour32 SunColour; //ok - lightsetting
        public double Precision; //ok - camsetting
        public Quaternion Rotation; //ok - camsetting
        public Vector3D Dir; // inutile

        public GPUCamera() { }

        public GPUCamera(Camera cam)
        {
            this.Position = cam.Malleable.Position;
            this.ClipPlanes = cam.ClipPlanes;
            this.FOV = cam.FieldOfView;
            this.Resolution = Graphics.GetRenderResolution();
            this.ClearColour = cam.ClearColour;
            this.SunDirection = Light.Main.Malleable.Forward;
            this.Precision = cam.Precision;
            this.Rotation = cam.Malleable.Rotation;
            this.SunColour = Light.Main.Colour;
            Dir = new Vector3D();

        }

        public struct GPUHitInfos
        {
            public bool HasHit;
            public Vector3D Normal;
            public Vector3D Point;
            public Colour32 Colour;
            public double Closest;
            public int Iterances;
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

                if (s != null) spheres.Add(new GPUSphere(s.Position, s.Scale, s.Colour));
            }
            int n = spheres.Count;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (Graphics.Processor == Graphics.RenderProc.CUDA)
                wrapped.GPUOpti(render, spheres.ToArray(), n, new GPUCamera(cam));
            else GPUOpti1D(render, spheres.ToArray(), n, new GPUCamera(cam));
            sw.Stop();

            cuda.DeviceSynchronize();

            string proc = "CPU";
            if (Graphics.Processor == Graphics.RenderProc.CUDA) proc = "CUDA";

            if (Graphics.Processor == Graphics.RenderProc.CUDA)
            {
                Log.Print("GPU Render time: " + sw.ElapsedMilliseconds + "ms (" + Math.Round((1D / sw.ElapsedMilliseconds) * 1000D, 0) + " FPS)");
            }

            else
            {
                Log.Print("CPU Render time: " + Math.Round(sw.Elapsed.TotalMilliseconds, 3) + "ms " +
                    "(" + Math.Round((1D / sw.Elapsed.TotalMilliseconds) * 1000D, 0) + " FPS, " +
                    resolution.x + "x" + resolution.y + ")");
            }

            string fpstxt = Math.Round((1D / sw.Elapsed.TotalMilliseconds) * 1000D, 0) + " FPS [" + proc + "]";
            Entry.ExecuteOnMainThread(() => { GameWindow.Instance.lbFPS.Text = fpstxt; });

            return render;
        }
        public class GPUSphere
        {
            public Vector3D position;
            public Vector3D scale;
            public Colour32 colour;
            public GPUSphere(Vector3D pos, Vector3D sca, Colour32 col)
            {
                scale = sca;
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
                    switch (index)
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

            public static implicit operator Pixel(byte[] bytes)
            {
                return new Pixel(bytes[0], bytes[1], bytes[2]);
            }

            public static implicit operator Pixel(Colour32 col)
            {
                return new Pixel(col.B, col.G, col.R);
            }

            public static implicit operator Pixel(Color col)
            {
                return new Pixel(col.B, col.G, col.R);
            }
        }

        [EntryPoint]
        public static void GPUOpti(byte[] pixels, GPUSphere[] spheres, int n, GPUCamera camera)
        {
            Colour256 sunCol = camera.SunColour;

            Colour32 SunDayColour = new Colour32(255, 255, 255, 255);
            Colour32 SunSetColour = new Colour32(255, 85, 0, 255);

            //Vector2I Secteurs = new Vector2I(10, 10);


            for (int y = threadIdx.y + blockIdx.y * blockDim.y; y < camera.Resolution.y; y += blockDim.y * gridDim.y)
            {
                for (int x = threadIdx.x + blockIdx.x * blockDim.x; x < camera.Resolution.x; x += blockDim.x * gridDim.x)
                {
                    int i = x + camera.Resolution.x * y;

                    Colour32 pix = new Colour32(camera.ClearColour);

                    //Skybox
                    Vector3D normal = GetDir(camera, new Vector2I() { x = x, y = y });
                    Vector3D sunDir = camera.SunDirection;

                    double SunAltitude = 1D;
                    bool negative = sunDir.Y < 0D;
                    SunAltitude = Math.Abs(sunDir.Y);

                    pix = SkyboxColour(normal, sunDir);

                    if (Vector3D.DistanceSquared(normal, sunDir) < 0.0025D && normal.Y > 0D)
                    {
                        if (negative)
                        {
                            pix = SunSetColour;
                        }

                        else
                        {
                            pix = RayMath.Lerp(SunSetColour, SunDayColour, Math.Sqrt(SunAltitude * 2D));
                        }
                    }

                    GPUHitInfos infos = Hit(camera.Position, normal, spheres, n, camera);

                    if (infos.HasHit)
                    {
                        double angle = Vector3D.Angle(infos.Normal, sunDir);
                        double lightIntensity = Math.Cos(-angle * RayMath.DegreeToRadian);

                        //Get Ambient by skybox
                        Colour256 skyCol = SkyboxColour(infos.Normal, sunDir);
                        double ambientIntensity = 0.3D;

                        pix = Colour32.Combine(skyCol * ambientIntensity, sunCol * lightIntensity);
                    }

                    //pix = sect;
                    pixels[i * 3] = pix[2];
                    pixels[i * 3 + 1] = pix[1];
                    pixels[i * 3 + 2] = pix[0];
                }
            }
        }



        public static void GPUOpti1D(byte[] pixels, GPUSphere[] spheres, int n, GPUCamera camera)
        {
            Colour256 sunCol = camera.SunColour;
            Colour32 SunDayColour = new Colour32(255, 255, 255, 255);
            Colour32 SunSetColour = new Colour32(255, 85, 0, 255);

            Parallel.For(0, camera.Resolution.y, y =>
            {
                for (int x = 0; x < camera.Resolution.x; x++)
                {
                    int i = x + camera.Resolution.x * y;


                    Colour32 pix = new Colour32(camera.ClearColour);

                    //Skybox
                    Vector3D normal = GetDir(camera, new Vector2I() { x = x, y = y });
                    Vector3D sunDir = camera.SunDirection;

                    double SunAltitude = 1D;
                    bool negative = sunDir.Y < 0D;
                    SunAltitude = Math.Abs(sunDir.Y);

                    pix = SkyboxColour(normal, sunDir);

                    if (Vector3D.DistanceSquared(normal, sunDir) < 0.0025D && normal.Y > 0D)
                    {
                        if (negative)
                        {
                            pix = SunSetColour;
                        }

                        else
                        {
                            pix = RayMath.Lerp(SunSetColour, SunDayColour, Math.Sqrt(SunAltitude * 2D));
                        }
                    }

                    GPUHitInfos infos = Hit(camera.Position, normal, spheres, n, camera);

                    if (infos.HasHit)
                    {
                        double angle = Vector3D.Angle(infos.Normal, sunDir);
                        double lightIntensity = Math.Cos(-angle * RayMath.DegreeToRadian);

                        //Get Ambient by skybox
                        Colour256 skyCol = SkyboxColour(infos.Normal, sunDir);
                        double ambientIntensity = 0.2D;

                        pix = Colour32.Combine(skyCol * ambientIntensity, sunCol * lightIntensity);
                    }

                    pixels[i * 3] = pix[2];
                    pixels[i * 3 + 1] = pix[1];
                    pixels[i * 3 + 2] = pix[0];
                }
            });
        }

        [Kernel]
        public static Colour32 SkyboxColour(Vector3D direction, Vector3D sunDirection)
        {

            Colour32 HorizonColourDay = new Colour32(210, 235, 252, 255);
            Colour32 HorizonColourSunset = new Colour32(255, 124, 0, 255);
            Colour32 NightColour = new Colour32(0, 0, 0, 255);
            Colour32 HighAtmosphereColour = new Colour32(61, 107, 179, 255);
            Colour32 GroundAtmosphereColour = new Colour32(150, 137, 116, 255);

            direction.Normalize();

            Colour32 pix = new Colour32(0, 0, 0, 255);

            if (direction == Vector3D.Null) return pix;

            double SunAltitude = 1D;

            bool negative = sunDirection.Y < 0;
            SunAltitude = Math.Abs(sunDirection.Y);

            Colour32 DenseAtmosphereColour = RayMath.Lerp(HorizonColourSunset, HorizonColourDay, Math.Sqrt(SunAltitude));
            Colour32 highAtmosphereColour = RayMath.Lerp(NightColour, HighAtmosphereColour, Math.Pow(SunAltitude, 0.6));
            Colour32 groundAtmosphereColour = RayMath.Lerp(NightColour, GroundAtmosphereColour, Math.Pow(SunAltitude, 0.6));

            if (negative)
            {
                DenseAtmosphereColour = RayMath.Lerp(HorizonColourSunset, NightColour, Math.Sqrt(SunAltitude));
                highAtmosphereColour = NightColour;
                groundAtmosphereColour = NightColour;
            }

            if (direction.Y > 0.0F)
            {
                pix = RayMath.Lerp(DenseAtmosphereColour, highAtmosphereColour, Math.Pow(direction.Y, 0.8));
            }
            else
            {
                pix = RayMath.Lerp(DenseAtmosphereColour, groundAtmosphereColour, Math.Pow(Math.Abs(direction.Y), 0.25));
            }

            return pix;
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

            /*Vector2D PixelNDC = new Vector2D(
            (pixel.x + 0.5D) / cam.Resolution.x,
            (pixel.y + 0.5D) / cam.Resolution.y
            );

            Vector2D PixelScreen = new Vector2D(
            2D * PixelNDC.x - 0.5D,
             2D * PixelNDC.y - 0.5D
                );*/

            double pixX = 2D * ((pixel.x + 0.5D) / cam.Resolution.x) - 0.5D;
            double pixY = 2D * ((pixel.y + 0.5D) / cam.Resolution.y) - 0.5D;

            double ImageAspectRatio = (double)cam.Resolution.x / cam.Resolution.y;

            Vector2D PixelCamera = new Vector2D(
            (2D * pixX - 1D) * ImageAspectRatio * Tan(cam.FOV / 2D * RayMath.DegreeToRadian),
            (1D - 2D * pixY) * Tan(cam.FOV / 2D * RayMath.DegreeToRadian)
            );

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

            GPUHitInfos hit = new GPUHitInfos() { HasHit = false, Closest = double.PositiveInfinity, Iterances = 0 };

            //ClosestInfos ci = new ClosestInfos();

            while (rayDst < maxDst)
            {
                hit.Iterances++;
                //double dst = Element.ClosestDistance(probe);
                //double dst = ClosestDistance(probe, spheres, n);
                ClosestInfos ci = Closest(probe, spheres, n, camera);

                if (ci.Distance < hit.Closest)
                {
                    hit.Closest = ci.Distance;
                }

                if (ci.Distance < camera.Precision)
                {
                    hit.Point = probe;
                    int i = ci.Index;//GetClosestIndex(probe, spheres, n);
                    hit.Colour = new Colour32(255, 255, 255, 255);//spheres[i].colour;
                    hit.Normal = GetSphereNormal(probe, spheres[i].position, spheres[i].scale);
                    hit.HasHit = true;

                    return hit;
                }

                probe += dir * ci.Distance;
                rayDst += ci.Distance;
            }



            return hit;
        }

        [Kernel]
        public static ClosestInfos Closest(Vector3D point, GPUSphere[] spheres, int n, GPUCamera cam)
        {
            ClosestInfos infos = new ClosestInfos();
            infos.Distance = double.MaxValue;

            double dst = double.MaxValue;
            for (int i = 0; i < n; i++)
            {
                dst = DistanceToSphereSQRT(point, spheres[i].position, spheres[i].scale.x);//DistanceToSphereSQRT(point, spheres[i].position, spheres[i].radius);

                if (dst < infos.Distance)
                {
                    infos.Distance = dst;
                    infos.Index = i;
                }

                //if (dst < cam.Precision) return infos;
            }
            //infos.Distance = HybMath.Sqrt(infos.Distance);

            return infos;
        }

        public struct ClosestInfos
        {
            public double Distance;
            public int Index;
        }

        [Kernel]
        public static double DistanceToBox(Vector3D point, Vector3D c, Vector3D size)
        {
            return
                Max(0, Math.Abs(point.x - c.x) - size.x / 2D) * Max(0, Math.Abs(point.x - c.x) - size.x / 2D) +
                Max(0, Math.Abs(point.y - c.y) - size.y / 2D) * Max(0, Math.Abs(point.y - c.y) - size.y / 2D) +
                Max(0, Math.Abs(point.z - c.z) - size.z / 2D) * Max(0, Math.Abs(point.z - c.z) - size.z / 2D);
        }

        [Kernel]
        public static double DistanceToBoxSQRT(Vector3D point, Vector3D centre, Vector3D size)
        {
            return HybMath.Sqrt(DistanceToBox(point, centre, size));
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
            if (a > b)
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
                double dst = DistanceToSphereSQRT(point, spheres[i].position, spheres[i].scale.z);

                if (dst < closestDistance)
                {
                    closestDistance = dst;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }

        [Kernel]
        public static Vector3D GetSphereNormal(Vector3D point, Vector3D spherePos, Vector3D scale)
        {
            //Converts sphere as unit sphere
            return Direction(point - spherePos);
        }

        [Kernel]
        public static Vector3D GetCubeNormal(Vector3D point, Vector3D cubePos, Vector3D size)
        {
            Vector3D relPos = point - cubePos;

            Vector3D dir = Vector3D.Null;

            if (relPos.y > size.y / 2D) return Vector3D.Up;
            if (relPos.y < -size.y / 2D) return Vector3D.Down;

            if (relPos.x > size.x / 2D) return Vector3D.Right;
            if (relPos.x < -size.x / 2D) return Vector3D.Left;



            if (relPos.z > size.z / 2D) return Vector3D.Forward;
            if (relPos.z < -size.z / 2D) return Vector3D.Backward;

            return dir.Normalize();

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
            double minDist = double.MaxValue;

            for (int i = 0; i < n; i++)
            {
                double dst = DistanceToSphere(pos, spheres[i].position, spheres[i].scale.z);

                if (dst < minDist) minDist = dst;
            }

            return Math.Sqrt(minDist);
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
            return (c.x - p.x) * (c.x - p.x) + (c.y - p.y) * (c.y - p.y) + (c.z - p.z) * (c.z - p.z) - radius * radius;
        }
        [Kernel]
        public static double DistanceToSphereSQRT(Vector3D p, Vector3D c, double radius)
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

            try
            {
                if (cuda.IsCudaAvailable())
                {
                    int props = prop.multiProcessorCount;

                    //BLOCK SIZE: ALWAYS MULTIPLE OF 32
                    //ex: 32 * 1 * 1, 32 * x * y...
                    dim3 threadsPerBlock = new dim3(16, 16, 1);//8, 8, 1);
                    dim3 numBlocks = new dim3(1024 / threadsPerBlock.x, 1024 / threadsPerBlock.y, 1);
                    HybRunner runner = HybRunner.Cuda("Raymarcher_CUDA.dll").SetDistrib(numBlocks, threadsPerBlock);
                    wrapped = runner.Wrap(new GPUCamera());

                    Graphics.Processor = Graphics.RenderProc.CUDA;
                }

                else
                {
                    //MessageBox.Show("CUDA is not available, running on CPU.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            catch//(Exception e)
            {
                //Log.Print(e);
                //MessageBox.Show("CUDA Libraries are not available, running on CPU.\n" + e, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

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
