#ifndef _RAYMARCHER_VECTOR3_H_
#define _RAYMARCHER_VECTOR3_H_

#include <redef.h>

typedef struct vector3f
{
    float x;
    float y;
    float z;
} vector3;


bool rm_vector3_equals(vector3 v1, vector3 v2);
vector3 rm_vector3_create(float x, float y, float z);
vector3 rm_vector3_multiply(vector3 v, float m);
vector3 rm_vector3_divide(vector3 v, float d);
vector3 rm_vector3_add(vector3 v1, vector3 v2);
vector3 rm_vector3_substract(vector3 v1, vector3 v2);
float rm_vector3_length(vector3 v);
vector3 rm_vector3_normalize(vector3 v);
float rm_vector3_squaredDistance(vector3 v1, vector3 v2);
float rm_vector3_distance(vector3 v1, vector3 v2);

//extern __constant vector3 RM_VECTOR3_UNIT;
//extern __constant vector3 RM_VECTOR3_RIGHT;
//extern __constant vector3 RM_VECTOR3_LEFT;
//extern __constant vector3 RM_VECTOR3_UP;
//extern __constant vector3 RM_VECTOR3_DOWN;
//extern __constant vector3 RM_VECTOR3_FORWARD;
//extern __constant vector3 RM_VECTOR3_BACKWARD;

#endif

