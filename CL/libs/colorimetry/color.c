#include <color.h>

bool rm_color_equals(color c1, color c2)
{
    return c1.r == c2.r && c1.g == c2.g && c1.b == c2.b && c1.a == c2.a;
}

color rm_color_createFromRGBA(float r, float g, float b, float a)
{
    return (color){ r, g, b, a };
}

color rm_color_createFromKnown(KnownColor c)
{
    switch (c)
    {
        case Black:
            return (color){ 0.0F, 0.0F, 0.0F, 1.0F };
        case Blue:
            return (color){ 0.0F, 0.0F, 1.0F, 1.0F };
        case Green:
            return (color){ 0.0F, 1.0F, 0.0F, 1.0F };
        case Red:
            return (color){ 1.0F, 0.0F, 0.0F, 1.0F };
        default:
            return (color){ 1.0F, 1.0F, 1.0F, 1.0F };
    }
}




