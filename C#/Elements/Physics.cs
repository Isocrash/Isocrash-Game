using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raymarcher
{
    class Physics : Module
    {
        public static Vector3D Gravity { get; set; } = new Vector3D(0D, -9.807D, 0D);

        public double Mass { get; set; } = 1D;

        public bool AttractedByGlobalGravity = false;

        public Vector3D Velocity { get; set; } = Vector3D.Null;

        
        protected internal override void FixedUpdate()
        {
            //Gravity
            if(AttractedByGlobalGravity)
            {
                Velocity += Gravity * Time.FixedDeltaTime;
            }

            
            this.Malleable.Translate(Velocity * Time.FixedDeltaTime);
        }
    }
}
