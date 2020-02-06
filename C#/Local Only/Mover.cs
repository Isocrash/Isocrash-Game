using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Drawing;

namespace Raymarcher
{
    public class Mover : Module
    {
        public double Speed = 5D;

        public static double MouseSensivity = 10D;

        public static Vector2D MouseRotation = Vector2D.Null;
        public static double Roll;

        protected internal override void OnCreation()
        {
            Input.ShowCursor = false;
            this.Malleable.Position = Vector3D.Forward * 2D;
        }

        protected internal override void Update()
        {
            Log.Print(Camera.Main.Malleable.Rotation);

            if (Input.CursorMode == CursorLockMode.Locked)
                MouseRotation += (Vector2D)Input.CursorMovement * MouseSensivity * Time.DeltaTime;

            if (MouseRotation.x < 0)
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


            if (Input.Triggering(Key.B))
            {
                Camera.Main.FieldOfView -= 10;
            }
            if (Input.Triggering(Key.N))
            {
                Camera.Main.FieldOfView += 10;
            }

            if (Input.Triggering(Key.Enter))
            {
                Graphics.ResolutionLockMode =
                    (Graphics.ResolutionLockMode == Graphics.LockMode.Size ? Graphics.LockMode.None : Graphics.LockMode.Size);

                if (Graphics.ResolutionLockMode == Graphics.LockMode.Size)
                {
                    Size s = GameWindow.Instance.Render.Size;
                    //Graphics.RenderResolution = new Vector2I(s.Width / 8, s.Height / 8);

                    // Graphics.RenderSizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                }

                else
                {
                    //  Graphics.RenderSizeMode = System.Windows.Forms.PictureBoxSizeMode.Normal;
                }
            }


            Camera.Main.Malleable.Rotation = EQuaternion.FromEuler(MouseRotation.y, MouseRotation.x, Roll);
        }
        protected internal override void FixedUpdate()
        {
            Malleable m = Camera.Main.Malleable;

            Vector3D f = m.Forward * Time.FixedDeltaTime * Speed;
            Vector3D r = m.Right * Time.FixedDeltaTime * Speed;
            Vector3D u = m.Up * Time.FixedDeltaTime * Speed;

            if (Input.Pressed(Key.Z))
            {
                m.Translate(f);
            }
            if (Input.Pressed(Key.S))
            {
                m.Translate(-f);
            }

            if (Input.Pressed(Key.Q))
            {
                m.Translate(-r);
            }
            if (Input.Pressed(Key.D))
            {
                m.Translate(r);
            }

            if (Input.Pressed(Key.Space))
            {
                m.Translate(u);
            }
            if (Input.Pressed(Key.LeftCtrl))
            {
                m.Translate(-u);
            }

            if (Input.Pressed(Key.O))
            {
                Time.FixedScale -= 1D;
            }
            if (Input.Pressed(Key.P))
            {
                Time.FixedScale += 1D;
            }


            if (Input.Pressed(Key.Escape))
            {
                if (Input.CursorMode == CursorLockMode.Free)
                {
                    Input.CursorMode = CursorLockMode.Locked;
                    Input.ShowCursor = true;
                    //System.Windows.Forms.Application.Exit();
                    //GameWindow.Instance.Close();
                }

                else
                {
                    Input.CursorMode = CursorLockMode.Free;
                    Input.ShowCursor = false;
                }
            }
        }
    }
}
