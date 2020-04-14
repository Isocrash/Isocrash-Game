#ifndef _RAYMARCHER_RAYMATH_H_
#define _RAYMARCHER_RAYMATH_H_


//#include<vector3.h>
//#include<quaternion.h>

#include <redef.h>
#include <quaternion.h>


#define DEG_TO_RAD 0.01745F
#define RAD_TO_DEG 57.2958F
#define F_PI 3.142857F
#define F_EPSILON 1.401298E-45F

typedef struct mat4f
{
    float* data;
} mat4;

float rm_math_map(float value, float oldLow, float oldHigh, float newLow, float newHigh);
float rm_math_copysignf(float x, float y);
double rm_math_dabs(double d);
float rm_math_lerp(float a, float b, float t);
float rm_math_max(float a, float b);
float rm_math_min(float a, float b);
int rm_math_flat_2d(int2 size, int2 coordinates);
int rm_math_flat_3d(int3 size, int3 coordinates);
float rm_math_fswap(float *x, float *y);

mat4 rm_mat4_create(float4 column0, float4 column1, float4 column2, float4 column3);
float4 rm_mat4_mult_float4(mat4 lhs, float4 vector);

mat4 rm_mat4_mult_mat4(mat4 lhs, mat4 rhs);
mat4 rm_mat4_axisAngle(float3 v, float angle);
mat4 rm_mat4_translate(float3 v);
//mat4 rm_mat4_inverse(mat4 m);
mat4 rm_mat4_rotate(quaternion q);
mat4 rm_mat4_cofactor(mat4 m);
mat4 rm_mat4_scale(float3 scale, float3 center);
float rm_mat4_determinant(mat4 m);
mat4 rm_mat4_invert(mat4 mat);
mat4 rm_mat4_transpose(mat4 * src, mat4 * dst);

#endif

