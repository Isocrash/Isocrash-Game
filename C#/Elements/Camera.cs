using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Raymarcher
{
    public class Camera : Module
    {
        public Camera() : base()
        {
            Cameras.Add(this);
        }

        public static Camera Main { get; set; }
        internal static List<Camera> Cameras { get; set; } = new List<Camera>();

        public Vector2D ClipPlanes { get; set; } = new Vector2D(0D, 1000D);
        public Colour ClearColour { get; set; } = new Colour(149, 192, 232);
        public double Precision { get; set; } = 0.001D;
        public double FieldOfView { get; set; } = 50D;


        public Bitmap RenderImage { get; private set; }

        public Bitmap Render()
        {
            Vector2I resolution = Graphics.GetRenderResolution();

            byte[] pixels = GPUCamera.Render(this);

            RenderImage = Imaging.RawToImage(pixels, resolution.x, resolution.y);
            
            return RenderImage;
        }


        protected internal override void OnCameraRender()
        {
            //Log.Print(this.Malleable.Rotation.ToEuler());
            Render();
        }

        public override void Destroy()
        {
            Cameras.Remove(this);
            base.Destroy();
        }
    }
}
