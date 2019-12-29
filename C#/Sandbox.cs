using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raymarcher
{
    public class Sandbox
    {
        internal static Sandbox Instance { get; set; }
        internal static List<Element> LoadedElements { get; set; } = new List<Element>();
        internal static List<Element> NextToSpawn { get; set; } = new List<Element>();

        public static List<Camera> Cameras { get; set; } = new List<Camera>();
        public Sandbox()
        {
            if (Instance != null) Instance = null;

            Camera.Main = new Camera() { Name = "Main Camera" };


            Light.Main = new Light() { Name = "Main Light" };
            /*Sphere.Main =*/// new Sphere() { Name = "Main Sphere", Position = Vector3D.Forward, Scale = Vector3D.Positive};
            //new Cube() { Name = "Cube", Position = new Vector3D(-1, 1, 1) * 1D };
            //new Sphere() { Name = "Secondary Sphere", Position = Vector3D.Positive * 1D };
            //new Sphere() { Name = "Third Sphere", Position = new Vector3D(-1, -1, 1) * 1D };
            //new Sphere() { Name = "Fourth Sphere", Position = new Vector3D(1, -1, 1) * 1D };

            Instance = this;
        }

        [EngineInitializer(3)]
        public static void Initialize()
        {
            new Sandbox();

            Log.Print("Sandbox Initialized");
        }
    }
}
