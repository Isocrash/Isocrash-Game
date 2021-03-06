﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raymarcher
{
    public class Primitive : Malleable
    {
        public virtual double DistanceFromSurface(Vector3D point) { return 0D; }
        public virtual double DistanceFromSurfaceSquared(Vector3D point) { return 0D; }
        public virtual Vector3D GetNormal(Vector3D point) { return Vector3D.Up; }
    }
}
