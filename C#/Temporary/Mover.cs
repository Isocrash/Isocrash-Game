using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Raymarcher
{
    public class Mover : Element
    {
        public static double Speed = 10D;

        public static Vector2D MouseRotation = Vector2D.Null;
        public static double MouseSensivity = 10D;

        public override void Update()
        {
            if (Input.Triggering(System.Windows.Input.Key.G))
            {
                PlanetaryPhysics.DoGravity = !PlanetaryPhysics.DoGravity;
            }
        }
        public override void FixedUpdate()
        {
            MouseRotation += (Vector2D)Input.CursorMovement * MouseSensivity * Time.FixedDeltaTime;

            if(MouseRotation.x < 0)
            {
                MouseRotation.x = MouseRotation.x + 360;
            }
            if (MouseRotation.x > 360)
            {
                MouseRotation.x = MouseRotation.x - 360;
            }
            if (MouseRotation.y < -90)
            {
                MouseRotation.y = -90;
            }
            if (MouseRotation.y > 90)
            {
                MouseRotation.y = 90;
            }

            Camera.Main.Rotation = EQuaternion.FromEuler(MouseRotation.y, MouseRotation.x, 0);

            //Log.Print("Mouse rotation: " + MouseRotation);

            Vector3D camPos = Camera.Main.Position;

            Vector3D dir = Vector3D.Null;

            double factor = 1;

            double zoomSensivity = 100F;

            if(Input.Triggering(Key.Escape))
            {
                Entry.ExecuteOnMainThread(() =>
                    GameWindow.Instance.Close()
                );
            }

            if(Input.Pressed(Key.LeftShift))
            {
                factor = 0.1D;
            }

            if (Input.Pressed(Key.B))
            {
                Camera.Main.FieldOfView += Time.FixedDeltaTime * zoomSensivity * factor;
            }

            if (Input.Pressed(Key.N))
            {
                Camera.Main.FieldOfView -= Time.FixedDeltaTime * zoomSensivity * factor;
            }

            if (Input.Pressed(Key.Z))
            {
                dir.z += 1 * factor;
                //dir.z += 1;
            }

            if(Input.Pressed(Key.S))
            {
                dir.z -= 1 * factor;
                //dir.z -= 1;
            }

            if (Input.Pressed(Key.Q))
            {
                dir.x -= 1;
            }

            if (Input.Pressed(Key.D))
            {
                dir.x += 1;
            }

            if (Input.Pressed(Key.Space))
            {
                dir.y += 1;
            }

            if (Input.Pressed(Key.LeftCtrl))
            {
                dir.y -= 1;
            }

            camPos += dir * Speed * Time.FixedDeltaTime * factor;

            Camera.Main.Position = camPos;
        }

        [EngineInitializer(10000)]
        public static void Init()
        {
            new Mover();
        }

    }
}
