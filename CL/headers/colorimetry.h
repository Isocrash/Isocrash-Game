#ifndef _RAYMARCHER_COLORIMETRY_H_
#define _RAYMARCHER_COLORIMETRY_H_

#include <raymath.h>

typedef struct
{
    byte r;
    byte g;
    byte b;
    byte a;
} colour32;

typedef struct
{
    float r;
    float g;
    float b;
    float a;
} colour128;

colour32 c_colour32(byte r, byte g, byte b, byte a);
colour128 c_colour128(float r, float g, float b, float a);
colour128 clerp(colour128 a, colour128 b, float t);

#endif /*#ifndef _RAYMARCHER_COLORIMETRY_H_*/