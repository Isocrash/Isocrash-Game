#include <color.h>
#include <raymath.h>

bool rm_color_equals(color b, color c2)
{
    return b.r == c2.r && b.g == c2.g && b.b == c2.b && b.a == c2.a;
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

color rm_color_multiply(color c, float m)
{
    return rm_color_createFromRGBA(c.r * m, c.g * m, c.b * m, c.a);
}

color rm_color_lerp(color a, color b, float t)
{
    return (color)
    {
        rm_math_lerp(a.r, b.r, t),
        rm_math_lerp(a.g, b.g, t),
        rm_math_lerp(a.b, b.b, t),
        rm_math_lerp(a.a, b.a, t)
    };
}

color rm_color_combine(color a, color b)
{
    return rm_color_createFromRGBA(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);
}


color rm_color_combineAlpha(color a, color b)
{
    float aAB = (1.0F - a.a) * b.a + a.a;

    return rm_color_createFromRGBA(
        ((1.0F - a.a) * b.a * b.r + a.a * a.r) / aAB,
        ((1.0F - a.a) * b.a * b.g + a.a * a.g) / aAB,
        ((1.0F - a.a) * b.a * b.b + a.a * a.b) / aAB,
        aAB
    );
}

color rm_color_add(color a, color b)
{
    return rm_color_createFromRGBA(
        rm_math_min(a.r + b.r, 1.0F),
        rm_math_min(a.g + b.g, 1.0F),
        rm_math_min(a.b + b.b, 1.0F),
        1.0F
    );
}



