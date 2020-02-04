using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hybridizer.Runtime.CUDAImports;

namespace Raymarcher.Rendering
{
    public static class RMath
    {
        /// <summary>
        /// Stucks a value between a minimal and maximal value.
        /// </summary>
        [Kernel]
        public static float Clamp(float f, float min, float max)
        {
            return Max(min, Min(f, max));
        }

        /// <summary>
        /// Returns the biggest value between two values.
        /// </summary>
        [Kernel]
        public static float Max(float a, float b)
        {
            return b < a ? a : b;
        }

        /// <summary>
        /// Returns the smallest value between two values.
        /// </summary>
        [Kernel]
        public static float Min(float a, float b)
        {
            return b < a ? b : a;
        }

        /// <summary>
        /// Returns the SQUARED length of the vector, way faster to use than SqrtLength.
        /// </summary>
        [Kernel]
        public static float Length(float2 v)
        {
            return v.x * v.x + v.y * v.y;
        }

        /// <summary>
        /// Returns the true length of the vector
        /// </summary>
        [Kernel]
        public static float SqrtLength(float2 v)
        {
            return HybMath.Sqrt(v.x * v.x + v.y * v.y);
        }

        /// <summary>
        /// Returns the SQUARED length of the vector, way faster to use than SqrtLength.
        /// </summary>
        [Kernel]
        public static float Length(float3 v)
        {
            return v.x * v.x + v.y * v.y;
        }

        /// <summary>
        /// Returns the true length of the vector
        /// </summary>
        [Kernel]
        public static float SqrtLength(float3 v)
        {
            return HybMath.Sqrt(v.x * v.x + v.y * v.y);
        }

        /// <summary>
        /// <para>Return the normalized / direction vector.</para>
        /// <para>Defaults to 1,0 if any error.</para>
        /// </summary>
        [Kernel]
        public static float2 Normalize(float2 v)
        {
            float l = Length(v);

            if (l == 0)
            {
                v.x = 0;
                v.y = 1;
            }

            else
            {
                v.x /= l;
                v.y /= l;
            }

            return v;
        }
        
        /// <summary>
        /// <para>Return the normalized / direction vector.</para>
        /// <para>Defaults to 1,0,0 if any error.</para>
        /// </summary>
        [Kernel]
        public static float3 Normalize(float3 v)
        {
            float l = Length(v);

            if(l == 0)
            {
                v.x = 1;
                v.y = v.z = 0;
            }

            else
            {
                v.x /= l;
                v.y /= l;
                v.z /= l;
            }

            return v;
        }
    }
}
