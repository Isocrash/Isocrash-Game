#include <colorimetry.h>

float map(float value, float oldLow, float oldHigh, float newLow, float newHigh)
{
    return newHigh + (value - oldLow) * (newHigh - newLow) / (oldHigh - oldLow);
}

float copysignf(float x, float y)
{
    return y == 0 ? x : fabs(x) * (y/fabs(y));
}

double copysignd(double x, double y)
{
    return y == 0 ? x : dabs(x) * (y/dabs(y));
}

double dabs(double d)
{
    return d < 0.0 ? d * -1.0 : d;
}

float vlength(vector3 v)
{
    return sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
}

vector3 vdirection(vector3 v)
{
    return vdividef(v, vlength(v));
}

vector3 vdividef(vector3 v, float diviser)
{
    v.x = v.x / diviser;
    v.y = v.y / diviser;
    v.z = v.z / diviser;

    return v;
}

vector3 vrotateEuler(vector3 v, float x, float y, float z)
{
    x *= DEG_TO_RAD;
    y *= DEG_TO_RAD;
    z *= DEG_TO_RAD;

    //X Rotation
    v.x = v.x;
    v.y = v.y * cosf(x) - v.z * sinf(x);
    v.z = v.y * sinf(x) + v.z * cosf(x);

}

float3 f3dividef(float3 v, float diviser)
{
    v.x = v.x / diviser;
    v.y = v.y / diviser;
    v.z = v.z / diviser;

    return v;
}

float f3length(float3 v)
{
    return sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
}

float3 f3direction(float3 v)
{
    return f3dividef(v, f3length(v));
}

colour32 c_colour32(byte r, byte g, byte b, byte a)
{
    colour32 c;
    c.r = r;
    c.g = g;
    c.b = b;
    c.a = a;
    return c;
}

colour128 c_colour128(float r, float g, float b, float a)
{
    colour128 c;
    c.r = r;
    c.g = g;
    c.b = b;
    c.a = a;
    return c;
}

colour128 clerp(colour128 a, colour128 b, float t)
{
    t = clamp(t, 0.0F, 1.0F);

    colour128 c;
    c.r = a.r + (b.r - a.r) * t;
    c.g = a.g + (b.g - a.g) * t;
    c.b = a.b + (b.b - a.b) * t;
    c.a = a.a + (b.a - a.a) * t;

    return c;
}

float vdistance(vector3 v1, vector3 v2)
{
     return (v2.x - v1.x) * (v2.x - v1.x) + (v2.y - v1.y) * (v2.y - v1.y) + (v2.z - v1.z) * (v2.z - v1.z);
}

float vdistancesqrt(vector3 v1, vector3 v2)
{
    return sqrt(vdistance(v1, v2));
}