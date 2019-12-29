using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Hybridizer.Runtime.CUDAImports;
using System.Diagnostics;

namespace Raymarcher
{
    public class Camera : Element
    {
        //public static List<Camera> Cameras { get; set; } = new List<Camera>();
        public static Camera Main { get; set; }
        public Vector2D ClipPlanes { get; set; } = new Vector2D(0D, 1000D);
        public double Size { get; set; } = 5D;
        public double FieldOfView = 70D;
        public Colour ClearColour { get; set; } = new Colour(149, 192, 232);//new Colour(49, 77, 121);
        public Processors RenderMode { get; set; } = Processors.CUDA;
        public double Precision { get; set; } = .001D;

        public Camera() : base()
        {
            Sandbox.Cameras.Add(this);
        }

        public Bitmap Render
        {
            get
            {
                return render;
            }
        }
        private Bitmap render;

        Stopwatch sw = new Stopwatch();
        public Bitmap RenderImage()
        {
            sw.Reset();
            sw.Start();
            Vector2I res = Graphics.GetRenderResolution();

            byte[] pixels = null;

            switch (RenderMode)
            {
                case Processors.NativeCPU:
                    pixels = CPURender();
                    break;

                case Processors.CUDA:
                    {
                        try
                        {

                            //pixels = GPUCamera.RenderCPU();

                            pixels = GPUCamera.Render(this);
                            //pixels = new byte[res.x * res.y * 3];

                        }
                        catch(Exception e)
                        {
                            Log.Print(e.Message + "\n" + e.StackTrace);
                        }
                        //pixels = new byte[];
                    }
                    break;
                default:
                    pixels = CPURender();
                    break;
            }

            
            render = Imaging.RawToImage(pixels, res.x, res.y);

            sw.Stop();
            Log.Print("Render time of camera: " + sw.ElapsedMilliseconds + "ms");
            return render;
        }

        public bool Hit(Vector3D point, Vector3D direction, out HitInfos hit)
        {
            Vector3D probe = point;

            double rayDst = 0D;
            double maxDst = this.ClipPlanes.y;
            double minDst = this.ClipPlanes.y;

            hit = new HitInfos();

            while(rayDst < maxDst)
            {
                double dst = Element.ClosestDistance(probe);

                if (dst < minDst)
                {
                    minDst = dst;
                    hit.ClosestPoint = probe;
                    hit.ClosestDistance = minDst;
                }

                if (dst < this.Precision)
                {
                    hit.Point = probe;
                    hit.Normal = ((Primitive)Element.Closest(probe)).GetNormal(probe);
                    return true;
                }

                probe += direction * dst;
                rayDst += dst;
            }

            return false;
        }  
        private byte[] CPURender()
        {
            //Log.Print("CPURender()");
            Vector2I resolution = Graphics.GetRenderResolution();
            byte[] image = new byte[resolution.x * resolution.y * 3];
            //Log.Print("Starting rendering " + (resolution.x * resolution.y) + " pixels");
            Parallel.For(0, resolution.x * resolution.y, i =>
            //{
            //for (int i = 0; i < resolution.x * resolution.y; i++)
            {
                //x and y coordinates
                int x = i % resolution.x;
                int y = i / resolution.x;
                byte[] pix = this.RenderPixel(x, resolution.y - y);
                image[i * 3] = pix[0];
                image[i * 3 + 1] = pix[1];
                image[i * 3 + 2] = pix[2];
                //Log.Print($"Pixel {i} done");
            //}
            });
            return image;
        }

        //X and Y pixels in screen pixel, not world.
        internal byte[] RenderPixel(int x, int y)
        {
            
            Vector2I RenderResolution = Graphics.GetRenderResolution();

            byte[] bytes = this.ClearColour.ToByte();
            byte[] clear = bytes;
            byte[] lightColour = Light.Main.Colour.ToByte();

            #region Frustrum calculations
            double hFOV = this.FieldOfView;
            double vFOV = (57.2958D * 2D) * Math.Atan(Math.Tan((hFOV * 0.0174533D) / 2D) * ((double)RenderResolution.y / RenderResolution.x));



            double pctX = (double)x / RenderResolution.x;
            double pctY = (double)y / RenderResolution.y;


            
            double yAngle = hFOV * pctX - (hFOV / 2D);
            double xAngle = vFOV * pctY - (vFOV / 2D);

            //Vector3D direction = Vector3D.RotateAroundY(Vector3D.Forward, Mover.MouseRotation.x);
            Vector3D direction = Vector3D.RotateAroundY(Vector3D.Forward, yAngle);
            direction = Vector3D.RotateAroundX(direction, -xAngle/* - Mover.MouseRotation.y*/);

            #endregion

            //Frustrum Size
            //Vector2D frustrumHeight = new Vector2D(2.0D * ClipPlanes.x * Math.Tan(this.FieldOfView * 0.5f * 0.01745D), 2.0D * ClipPlanes.y * Math.Tan(this.FieldOfView * 0.5f * 0.01745D));
            //Vector2D frustrumWidth = frustrumHeight / Graphiques.Ratio;
            Vector3D SunAngle = new Vector3D(1, 1, 0);

            if (Hit(this.Position, direction, out HitInfos hit))
            {
                //0 => Sun Dir
                //180 => Opposite
                double angle = Vector3D.Angle(hit.Normal, SunAngle);

                //Shadow
                if (angle < 90)
                {
                    double sunLight = Math.Sin((angle + 90) * 0.0174533D);
                    //Shadow
                    if(!Hit(hit.Point + hit.Normal * Precision, SunAngle, out HitInfos shadowHit))
                    {
                        double light = Math.Sin((angle + 90) * 0.0174533D);
                        bytes = new byte[3] { (byte)(light * 255), (byte)(light * 255), (byte)(light * 255) };
                    }

                    else
                    {
                        bytes = new byte[3] { 0, 0, 0 };
                    }
                        
                }
                else
                {
                    bytes = new byte[3] { 0, 0, 0 };
                }
                    

                /*double reflexionPower = 0.5;
                byte[] reflexionColor;
                //Reflexion
                //If hiting something, if not return clear
                if(Hit(hit.Point + hit.Normal * this.Precision, hit.Normal, out HitInfos reflexionHit))
                {
                    reflexionColor = new byte[3] { 255, 255, 255 };
                }
                else
                {
                    reflexionColor = clear;
                }

                bytes = new byte[3] { (byte)(Map(bytes[0] + reflexionPower * reflexionColor[0], 0, 255 + 255 * reflexionPower, 0, 255)), (byte)(Map(bytes[1] + reflexionPower * reflexionColor[1], 0, 255 + 255 * reflexionPower, 0, 255)), (byte)(Map(bytes[2] + reflexionPower * reflexionColor[2], 0, 255 + 255 * reflexionPower, 0, 255)) };
                */
            }

            else
            {
                //TEST ATMOSPHERE
                /*if(hit.ClosestDistance < 0.025D)
                {
                    if(!Hit(hit.ClosestPoint, SunAngle, out HitInfos atmoshadow))
                    {
                        double pct = 1 - (hit.ClosestDistance / 0.025D);
                        bytes = new byte[3] { (byte)(pct * 255), (byte)(234 * pct), (byte)(158 * pct) };
                    }
                }*/
            }

            return bytes;
        }

        /// <summary>
        /// Screen pixel referential to world coordinate TODO: Rotation
        /// </summary>
        public Vector3D ScreenToWorldPoint(double x, double y)
        {
            Vector3D pos = this.Position;
            Vector2I res = Graphics.GetRenderResolution();
            Vector2D percent = new Vector2D(x / res.x, y / res.y);

            percent *= Size;
            percent -= Size / 2D;
            percent += new Vector2D(pos.x, pos.y);
            percent.x *= Graphics.Ratio;

            return new Vector3D(percent.x, percent.y, ClipPlanes.x + this.Position.z);
        }

        public Vector3D ScreenToWorldPoint(int x, int y)
        {
            return ScreenToWorldPoint((double)x, (double)y);
        }

        public Vector3D ScreenToWorldPoint(Vector2D screenCoordinates)
        {
            return ScreenToWorldPoint((double)screenCoordinates.x, (double)screenCoordinates.y);
        }
        public Vector3D ScreenToWorldPoint(Vector2I screenCoordinates)
        {
            return ScreenToWorldPoint((double)screenCoordinates.x, (double)screenCoordinates.y);
        }

        public double Map(double value, double currentMin, double currentMax, double newMin, double newMax)
        {
            return newMin + (value - currentMin) * (newMax - newMin) / (currentMax - currentMin);
        }
    }
}

public struct HitInfos
{
    public Vector3D Point;
    public Vector3D ClosestPoint;
    public double ClosestDistance;
    public Vector3D Normal;

    internal HitInfos(Vector3D point, Vector3D normal, Vector3D closest, double closestDistance)
    {
        Point = point;
        Normal = normal;
        ClosestPoint = closest;
        ClosestDistance = closestDistance;
    }
}
