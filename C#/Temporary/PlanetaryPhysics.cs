using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raymarcher
{
    class PlanetaryPhysics : Element
    {
        public static List<Sphere> planets = new List<Sphere>();
        public const double G = 6.67430E-11;
        public static double Gravity = 0.01D;
        public static bool DoGravity = false;

        [EngineInitializer(198)]
        public static void Init()
        {
            for (int i = 0; i < 20; i++)
            {
                Vector3D pos = RandomDirection();
                pos.z = 0;

                pos.Normalize();
                pos *= 1/EngineInitializer.r.NextDouble();

                new Planet()
                {
                    Position = pos,
                    Scale = Vector3D.Positive * 0.1D * (EngineInitializer.r.NextDouble()) + 0.025D,
                };
            }

            new PlanetaryPhysics();
        }

        public override void Update()
        {
            Log.Print(planets.Count);
        }

        public static Vector3D RandomDirection()
        {
            Random r = EngineInitializer.r;
            return new Vector3D(r.NextDouble() - 0.5D, r.NextDouble() - 0.5D, r.NextDouble() - 0.5D).Direction;
        }
    }
}
