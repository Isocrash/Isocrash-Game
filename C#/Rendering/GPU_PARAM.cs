using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raymarcher.Rendering
{
    public struct GPU_PARAM
    {
        public int X_RESOLUTION;
        public int Y_RESOLUTION;

        public float TIME;
    }

    public struct PIXEL
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;
    }
}
