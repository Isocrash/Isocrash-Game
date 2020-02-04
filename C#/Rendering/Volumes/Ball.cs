using Hybridizer.Runtime.CUDAImports;
using rm = Raymarcher.Rendering.RMath;

namespace Raymarcher.Rendering
{
    public sealed class Ball : Volume
    {
        public override float Distance(float3 point)
        {
            //TODO: take in count the scale
            return
                (position.x - point.x) * (position.x - point.x) +
                (position.y - point.y) * (position.y - point.y) +
                (position.z - point.z) * (position.z - point.z) -
                scale.z * scale.z;
        }

        public override float3 NormalToSurface(float3 point)
        {
            return rm.Normalize(point - position);
        }
    }
}
