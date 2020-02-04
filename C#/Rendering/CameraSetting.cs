using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hybridizer.Runtime.CUDAImports;

namespace Raymarcher.Rendering
{
    public struct CameraSetting
    {
        public float3 position;
        public float4 rotation;

        public float4 clearColour;
        public float2 resolution;
        public int2 fov;
        public float farClipDistance;

        public float precision;
    }
}
