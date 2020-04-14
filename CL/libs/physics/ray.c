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

ray3 rm_ray3_create(float3 origin, float3 direction)
{
    int sign[3];
    float3 invdir = 1.0F/direction;
    sign[0] = (invdir.x < 0.0F);
    sign[1] = (invdir.y < 0.0F);
    sign[2] = (invdir.z < 0.0F);
    return (ray3){origin, direction, invdir, sign};
}

