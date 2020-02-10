using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raymarcher
{
    class PlanetSpawner : Module
    {
        public static Sphere p;
        protected internal override void OnCreation()
        {

            Sphere.Main = new Sphere() { Name = "Cube", Position = Vector3D.Null, Scale = Vector3D.Positive * 1D };

            //Body b = this.Malleable.AddModule<Body>();
            //b.Volume = new Rendering.Ball();
            //return;
            /*for (int z = 0; z < 3; z++)
            {
                for (int x = 0; x < 3; x++)
                {
                    new Sphere() { Name = "Cube", Position = new Vector3D(x, 0.0D, z), Scale = Vector3D.Positive * 1D };
                }
            }*/
            // p = new Sphere() { Name = "Cube", Position = new Vector3D(0.0D,0.0D,0.0D), Scale = Vector3D.Positive * 0.5D };
            // p.AddModule<Local_Only.MouvementAleatoire>();

            //Planet pla = p.AddModule<Planet>();

            //Physics phy = pla.Malleable.AddModule<Physics>();//p.Module<Physics>();

            //pla.Physic = phy;
            //phy.AttractedByGlobalGravity = false;
            //phy.Mass = Malleable.Scale.z * Malleable.Scale.z * Malleable.Scale.z * 1E+10;
            //phy.Freeze = true;

            /*new Sphere() { Position = Vector3D.Forward };

            new Sphere() { Position = Vector3D.Right * 3D };*/
        }
        protected internal override void Update()
        {
            return;
            if (Input.Triggering(System.Windows.Input.Key.Enter))
            {
                for (int i = 0; i < 25; i++)
                {

                    Vector3D pos = Planet.RandomDirection();
                    pos.z = 0;

                    pos.Normalize();
                    //pos *= 1 / EngineInitializer.r.NextDouble();

                    Sphere p = new Sphere() { Position = pos * ((EngineInitializer.r.NextDouble() * 1D) + 0.5D), Scale = Vector3D.Positive * 0.015D * (EngineInitializer.r.NextDouble()) + 0.01D };

                    Planet pla = p.AddModule<Planet>();

                    Physics phy = pla.Malleable.AddModule<Physics>();//p.Module<Physics>();

                    pla.Physic = phy;
                    phy.AttractedByGlobalGravity = false;
                    phy.Mass = Malleable.Scale.z * Malleable.Scale.z * Malleable.Scale.z * 1E+8;
                    phy.Velocity = EQuaternion.FromEuler(0, 0, 90D) * p.Position.Direction * -1.0D;
                }
            }
        }
    }

    class Planet : Module
    {
        public const double G = 6.674E-11;
        public static List<Planet> planets = new List<Planet>();

        public Physics Physic;
        public Planet() : base()
        {
            planets.Add(this);
        }

        public override void Destroy()
        {
            planets.Remove(this);

            base.Destroy();
        }

        protected internal override void OnCreation()
        {


            Physic = Malleable.AddModule<Physics>();
            Physic.AttractedByGlobalGravity = false;
            Physic.Mass = Malleable.Scale.z * Malleable.Scale.z * Malleable.Scale.z * 1E+9;
        }

        public static Vector3D RandomDirection()
        {
            Random r = EngineInitializer.r;
            return new Vector3D(r.NextDouble() - 0.5D, r.NextDouble() - 0.5D, r.NextDouble() - 0.5D).Direction;
        }

        protected internal override void FixedUpdate()
        {
            Physic.AttractedByGlobalGravity = false;

            List<Planet> toDelete = new List<Planet>();
            foreach (Planet planet in planets)
            {

                if (planet == this) continue;

                double distance = Vector3D.Distance(this.Malleable.Position, planet.Malleable.Position);

                if (distance < Malleable.Scale.Z / 2D)
                {
                    if (Physic.Mass >= planet.Physic.Mass)
                    {
                        Physic.Mass += planet.Physic.Mass;

                        double volume = (4D / 3D) * Math.PI * Math.Pow(this.Malleable.Scale.z / 2D, 3D);
                        volume += (4D / 3D) * Math.PI * Math.Pow(planet.Malleable.Scale.z / 2D, 3D);

                        double radius = Math.Pow(((3D * volume) / (4D * Math.PI)), 1D / 3D);
                        //Log.Print(radius);
                        this.Malleable.Scale = Vector3D.Positive * radius * 2D;

                        toDelete.Add(planet);
                        //sphere.Destroy();
                    }
                }

                double force = G * (this.Physic.Mass * planet.Physic.Mass / (distance * distance));

                Vector3D dir = planet.Malleable.Position - this.Malleable.Position;

                double acceleration = force / this.Physic.Mass;



                this.Physic.Velocity += dir * acceleration * Time.FixedDeltaTime;
            }

            Planet[] spheresToDelete = toDelete.ToArray();
            for (int i = 0; i < spheresToDelete.Length; i++)
            {
                spheresToDelete[i].Malleable.Destroy();
            }
        }
    }
}
