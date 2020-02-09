#ifndef _RAYMARCHER_COLOR_H_
#define _RAYMARCHER_COLOR_H_

typedef struct colorf
{
    float r;
    float g;
    float b;
    float a;
} color;

typedef enum colors
{
    Black, 
    Blue,
    Green,
    Red,
    White
} KnownColor;

bool rm_color_equals(color c1, color c2);
color rm_color_createFromRGBA(float r, float g, float b, float a);
color rm_color_createFromKnown(KnownColor c);
color rm_color_lerp(color c1, color c2, float t);

#endif



