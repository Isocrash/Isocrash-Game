using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCL.Net;
using System.Numerics;

namespace Raymarcher.Rendering
{
    public struct C_CAMERA
    {
        public int2 resolution;
        public float fov;
        public C_VECTOR3 position;
        public C_QUATERNION rotation;

        public C_CAMERA(Camera camera)
        {
            Vector2I res = Graphics.RenderResolution;
            resolution = new int2(res.x, res.y);
            fov = (float)camera.FieldOfView;

            position = new C_VECTOR3(camera.Malleable.Position);
            rotation = new C_QUATERNION(camera.Malleable.Rotation);
        }
    }

    public struct C_VECTOR3
    {
        public float x, y, z;

        public C_VECTOR3(Vector3D v)
        {
            x = (float)v.x;
            y = (float)v.y;
            z = (float)v.z;
        }
    }

    public struct C_QUATERNION
    {
        public float x,y,z,w;

        public C_QUATERNION(Quaternion q)
        {
            x = q.X;
            y = q.Y;
            z = q.Z;
            w = q.W;
        }
    }
}
