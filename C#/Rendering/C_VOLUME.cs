using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raymarcher.Rendering
{
    public struct C_VOLUME
    {
        public C_VECTOR3 position;
        public C_QUATERNION rotation;
        public C_VECTOR3 scale;
        public volumeType type;

        public C_VOLUME(Primitive p)
        {
            position = new C_VECTOR3(p.Position);
            rotation = new C_QUATERNION(p.Rotation);
            scale = new C_VECTOR3(p.Scale);

            if(p as Sphere != null)
            {
                type = volumeType.ball;
            }

            else
            {
                type = volumeType.box;
            }

        }
    }

    public enum volumeType
    {
        none = 0,
        ball = 1,
        box = 2
    }
}
