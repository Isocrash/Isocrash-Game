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

        public Vector2D ClipPlanes { get; set; } = new Vector2D(0D, 1E+3);
        public Colour32 ClearColour { get; set; } = new Colour32(149, 192, 232, 255);
        public double Precision { get; set; } = 1E-3;
        public double FieldOfView { get; set; } = 70D;


        public Bitmap RenderImage { get; private set; }

        public Bitmap Render()
        {
           // Vector2I resolution = Graphics.GetRenderResolution();

            RenderImage = Raymarcher.Rendering.Renderer.Bake(this);

            return RenderImage;
        }


        protected internal override void OnCameraRender()
        {
            Render();
        }

        public override void Destroy()
        {
            Cameras.Remove(this);
            base.Destroy();
        }
    }
}
