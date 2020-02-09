#ifndef _RAYMARCHER_RAYMATH_H_
#define _RAYMARCHER_RAYMATH_H_

#define DEG_TO_RAD 0.01745F
#define RAD_TO_DEG 57.2958F
#define F_PI 3.142857F
typedef unsigned char byte;

typedef struct
{
    float x;
    float y;
    float z;
    float w;
} quaternion;

typedef struct
{
    float x;
    float y;
    float z;
} vector3;

typedef struct
{
    vector3 origin;
    vector3 direction;
} ray;

enum shape
{
    Ball,
    Box
};


vector3 vdividef(vector3 v, float diviser);
vector3 vdirection(vector3 v);
float vlength(vector3 v);
vector3 vdividef(vector3 vector, float diviser);

float3 f3dividef(float3 v, float diviser);
float f3length(float3 v);
float3 f3direction(float3 v);

float3 qmultiplyf3(float3 v, quaternion q);
vector3 qmultiplyv(vector3 v, quaternion q);

vector3 toEuler(quaternion quat);
float copysignf(float x, float y);
double copysignd(double x, double y);
double dabs(double d);
float map(float value, float oldLow, float oldHigh, float newLow, float newHigh);


float vdistance(vector3 v1, vector3 v2);
float vdistancesqrt(vector3 v1, vector3 v2);

#endif /* _RAYMARCHER_RAYMATH_H_*/