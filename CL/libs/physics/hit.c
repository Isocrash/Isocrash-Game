#include <physics.h>

hit rm_hit_create(vector3 location, vector3 normal)
{
    return (hit){ 1, location, normal };
}

