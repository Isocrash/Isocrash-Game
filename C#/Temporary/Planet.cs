using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raymarcher
{
    class Planet : Sphere
    {
        public double Speed = 0D;

        public double Mass = 1;

		public Planet() : base()
        {
            PlanetaryPhysics.planets.Add(this);

            Mass = this.Scale.z * this.Scale.z * this.Scale.z * 1E+9;
        }

        public override void Destroy()
        {
            PlanetaryPhysics.planets.Remove(this);

            base.Destroy();
        }

        public override void FixedUpdate()
        {
            List<Planet> toDelete = new List<Planet>();
            foreach (Planet planet in PlanetaryPhysics.planets)
            {
                
                if (planet == this) continue;

                double distance = Vector3D.Distance(this.Position, planet.Position);

                if (distance < this.Scale.Z / 2D)
                {
                    if (this.Mass >= planet.Mass)
                    {
                        this.Mass += planet.Mass;

                        double volume = (4D / 3D) * Math.PI * Math.Pow(this.Scale.z / 2D, 3D);
                        volume += (4D / 3D) * Math.PI * Math.Pow(planet.Scale.z / 2D, 3D);

                        double radius = Math.Pow(((3D * volume) / (4D * Math.PI)), 1D / 3D);
                        //Log.Print(radius);
                        this.Scale = Vector3D.Positive * radius * 2D;

                        toDelete.Add(planet);
                        //sphere.Destroy();
                    }
                }

                double force = PlanetaryPhysics.G * (this.Mass * planet.Mass / (distance * distance));

                Vector3D dir = planet.Position - this.Position;

                double acceleration = force / this.Mass;

                this.Position = this.Position + dir * acceleration * Time.FixedDeltaTime;
            }

            Sphere[] spheresToDelete = toDelete.ToArray();
            for (int i = 0; i < spheresToDelete.Length; i++)
            {
                spheresToDelete[i].Destroy();
            }
        }
    }
}
