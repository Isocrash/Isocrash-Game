using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Raymarcher
{
    public class Mover : Module
    {
        public double Speed = 10D;
        protected internal override void FixedUpdate()
        {
            Vector3D dir = new Vector3D();


            if (Input.Pressed(Key.Z))
            {
                dir.z += 1D;
            }
            if (Input.Pressed(Key.S))
            {
                dir.z -= 1D;
            }

            if (Input.Pressed(Key.Q))
            {
                dir.x -= 1D;
            }
            if (Input.Pressed(Key.D))
            {
                dir.x += 1D;
            }

            if (Input.Pressed(Key.Space))
            {
                dir.y += 1D;
            }
            if (Input.Pressed(Key.LeftCtrl))
            {
                dir.y -= 1D;
            }


            this.Malleable.Position += dir * Speed * Time.FixedDeltaTime;
        }
    }
}
