#include <physics.h>
#include <vector3.h>

bool rm_ray_equals(ray r1, ray r2)
{
    return rm_vector3_equals(r1.origin, r2.origin) && rm_vector3_equals(r1.direction, r2.direction);
}

ray rm_ray_create(vector3 origin, vector3 direction)
{
    return (ray){ origin, direction };
}

