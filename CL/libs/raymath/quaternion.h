#ifndef _RAYMARCHER_QUATERNION_H_
#define _RAYMARCHER_QUATERNION_H_

#include <vector3.h>

typedef struct quaternionf
{
    float x;
    float y;
    float z;
    float w;
} quaternion;

bool rm_quaternion_equals(quaternion q1, quaternion q2);
quaternion rm_quaternion_create(float x, float y, float z, float w);
vector3 rm_quaternion_multiply(quaternion q, vector3 v);

#endif

