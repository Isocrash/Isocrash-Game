#ifndef _RAYMARCHER_PHYSICS_H_
#define _RAYMARCHER_PHYSICS_H_

#include <vector3.h>
#include <redef.h>
#include <quaternion.h>

typedef struct ray3
{
    float3 origin;
    float3 direction;
    float3 invdirection;
    int *sign;
} ray3;

typedef struct rayv3
{
    vector3 origin;
    vector3 direction;
} ray;

typedef struct hitInfos
{
    bool hit;
    vector3 location;
    vector3 normal;
} hit;

typedef struct ic_box3
{
    float3 center;
    quaternion rotation;
    float3 extents;
} box3;

bool rm_ray_equals(ray r1, ray r2);
ray rm_ray_create(vector3 origin, vector3 direction);
hit rm_hit_create(vector3 location, vector3 normal);
box3 rm_box3_create(float3 center, quaternion rotation, float3 extends);
bool rm_box3_oob(ray3 r, box3 box, float4* result, float3* receivedRelDir, float3* receivedRelPoint);
ray3 rm_ray3_create(float3 origin, float3 direction);


#endif


