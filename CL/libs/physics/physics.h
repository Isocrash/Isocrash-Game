#ifndef _RAYMARCHER_PHYSICS_H_
#define _RAYMARCHER_PHYSICS_H_

#include <vector3.h>
#include <redef.h>

typedef struct rayv3
{
    vector3 origin;
    vector3 direction;
} ray;

bool rm_ray_equals(ray r1, ray r2);
ray rm_ray_create(vector3 origin, vector3 direction);

#endif

