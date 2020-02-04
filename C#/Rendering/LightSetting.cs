using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hybridizer.Runtime.CUDAImports;

namespace Raymarcher.Rendering
{
    public struct LightSetting
    {
        public float3 mainLightDir;
        public float4 mainLightColour;
    }
}