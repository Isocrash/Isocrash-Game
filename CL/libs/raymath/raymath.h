#ifndef _RAYMARCHER_RAYMATH_H_
#define _RAYMARCHER_RAYMATH_H_


//#include<vector3.h>
//#include<quaternion.h>

#include <redef.h>


#define DEG_TO_RAD 0.01745F
#define RAD_TO_DEG 57.2958F
#define F_PI 3.142857F

float rm_math_map(float value, float oldLow, float oldHigh, float newLow, float newHigh);
float rm_math_copysignf(float x, float y);
double rm_math_dabs(double d);
float rm_math_lerp(float a, float b, float t);

#endif

