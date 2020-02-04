using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Hybridizer.Runtime.CUDAImports;

namespace Raymarcher.Rendering
{
    internal static class Renderer
    {
        [Kernel]
        private static void Render(byte[] pixels, GraphicalObject[] objects, RenderSetting settings)
        {
            /*for (int y = threadIdx.y + blockIdx.y * blockDim.y; y < camera.Resolution.y; y += blockDim.y * gridDim.y)
            {
                for (int x = threadIdx.x + blockIdx.x * blockDim.x; x < camera.Resolution.x; x += blockDim.x * gridDim.x)
                {

                }
            }*/
        }

        public static Bitmap Render(Camera camera)
        {
            
            return null;
        }
    }
}
