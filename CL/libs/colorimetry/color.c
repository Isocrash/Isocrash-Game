#include <color.h>
#include <raymath.h>

bool rm_color_equals(color c1, color c2)
{
    return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b && c1.a == c2.a;
}

color rm_color_createFromRGBA(float r, float g, float b, float a)
{
    return (color){ r, g, b, a };
}

color rm_color_createFromKnown(knownColor c)
{
    switch (c)
    {
        case black:
            return (color){ 0.0F, 0.0F, 0.0F, 1.0F };
        case blue:
            return (color){ 0.0F, 0.0F, 1.0F, 1.0F };
        case green:
            return (color){ 0.0F, 1.0F, 0.0F, 1.0F };
        case red:
            return (color){ 1.0F, 0.0F, 0.0F, 1.0F };
        default:
            return (color){ 1.0F, 1.0F, 1.0F, 1.0F };
    }
}

color rm_color_lerp(color c1, color c2, float t)
{
    return (color)
    {
        rm_math_lerp(c1.r, c2.r, t),
        rm_math_lerp(c1.g, c2.g, t),
        rm_math_lerp(c1.b, c2.b, t),
        rm_math_lerp(c1.a, c2.a, t)
    };
}

