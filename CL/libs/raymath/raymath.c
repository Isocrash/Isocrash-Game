#include <raymath.h>

float rm_math_map(float value, float oldLow, float oldHigh, float newLow, float newHigh)
{
    return newHigh + (value - oldLow) * (newHigh - newLow) / (oldHigh - oldLow);
}

float rm_math_copysignf(float x, float y)
{
    return y == 0 ? x : fabs(x) * (y/fabs(y));
}

double rm_math_dabs(double d)
{
    return d < 0.0 ? -d : d;
}

float rm_math_lerp(float a, float b, float t)
{
    return a + (b - a) * clamp(t, 0.0F, 1.0F);
}

float rm_math_max(float a, float b)
{
    return a < b ? b : a;
}

float rm_math_min(float a, float b)
{
    return a < b ? a : b;
}

int rm_math_flat_2d(int2 size, int2 coordinates)
{
    return coordinates.x + coordinates.y * size.x;
}

//Get 1D index of a 3D index
int rm_math_flat_3d(int3 size, int3 coordinates)
{
    return coordinates.x + size.x*coordinates.y + size.x*size.y*coordinates.z;
    //return size.x + coordinates.x * (coordinates.y + size.z * coordinates.z);
}

float3 rm_math_create_float3(float x, float y, float z)
{
    return (float3){x,y,z};
}

float rm_math_fswap(float *x, float *y)
{
    int temp = *x;
    *x = *y;
    *y = temp;
}




