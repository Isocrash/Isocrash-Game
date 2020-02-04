using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hybridizer.Runtime.CUDAImports;

namespace Raymarcher.Rendering
{
    public struct Ray
    {
        public float3 Origin;

        public float3 Direction;
    }
}
