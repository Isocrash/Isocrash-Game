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
    return d < 0.0 ? d * -1.0 : d;
}

float rm_math_lerp(float a, float b, float t)
{
    return a + (b - a) * clamp(t, 0.0F, 1.0F);
}

