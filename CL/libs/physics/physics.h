#ifndef _RAYMARCHER_PHYSICS_H_
#define _RAYMARCHER_PHYSICS_H_

#include <vector3.h>
#include <redef.h>

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

bool rm_ray_equals(ray r1, ray r2);
ray rm_ray_create(vector3 origin, vector3 direction);
hit rm_hit_create(vector3 location, vector3 normal);

#endif

