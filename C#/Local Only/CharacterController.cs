using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raymarcher.Local_Only
{
    public class CharacterController : Module
    {
        private Physics _Phy;
        private Malleable _M;

        public static double MouseSensivity { get; set; } = 10D;

        public Vector2D MouseRotation = Vector2D.Null;

        protected internal override void OnCreation()
        {
            _M = this.Malleable;

            _Phy = _M.AddOrGetModule<Physics>();
            //_Phy.AttractedByGlobalGravity = false;
        }

        protected internal override void Update()
        {
            Rotation();
        }

        protected internal override void FixedUpdate()
        {
            Movement();
        }

        private void Movement()
        {







            if (Input.Triggering(System.Windows.Input.Key.Space))
            {
                _Phy.Velocity += Vector3D.Up * 10;
            }

            //if underground (because no physics yet lol)
            if (_M.Position.y <= 0.0F)
            {
                _M.Position = new Vector3D(_M.Position.x, 0.01F, _M.Position.z);

                _Phy.Velocity = new Vector3D(_Phy.Velocity.x, 0, _Phy.Velocity.z);
            }
        }


        private void Rotation()
        {
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

            _M.Rotation = EQuaternion.FromEuler(MouseRotation.y, MouseRotation.x, 0.0F);
        }
    }
}
