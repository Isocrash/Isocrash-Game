#include <vector3.h>

bool rm_vector3_equals(vector3 v1, vector3 v2)
{
    return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
}

vector3 rm_vector3_create(float x, float y, float z)
{
    return (vector3){ x, y, z };
}

vector3 rm_vector3_multiply(vector3 v, float m)
{
    return rm_vector3_create(v.x * m, v.y * m, v.z * m);
}

vector3 rm_vector3_divide(vector3 v, float d)
{
    return rm_vector3_create(v.x / d, v.y / d, v.z / d);
}

vector3 rm_vector3_add(vector3 v1, vector3 v2)
{
    return rm_vector3_create(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
}

vector3 rm_vector3_substract(vector3 v1, vector3 v2)
{
    return rm_vector3_create(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
}


float rm_vector3_length(vector3 v)
{
    return sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
}

vector3 rm_vector3_normalize(vector3 v)
{
    return rm_vector3_divide(v, rm_vector3_length(v));
}

float rm_vector3_squaredDistance(vector3 v1, vector3 v2)
{
    return
    (v2.x - v1.x) * (v2.x - v1.x) +
    (v2.y - v1.y) * (v2.y - v1.y) +
    (v2.z - v1.z) * (v2.z - v1.z);
}

float rm_vector3_distance(vector3 v1, vector3 v2)
{
    return sqrt(rm_vector3_squaredDistance(v1, v2));
}

//__constant vector3 RM_VECTOR3_UNIT = { 1.0F, 1.0F, 1.0F };
//__constant vector3 RM_VECTOR3_RIGHT = { 1.0F, 0.0F, 0.0F };
//__constant vector3 RM_VECTOR3_LEFT = { -1.0F, 0.0F, 0.0F };
//__constant vector3 RM_VECTOR3_UP = { 0.0F, 1.0F, 0.0F };
//__constant vector3 RM_VECTOR3_DOWN = { 0.0F, -1.0F, 0.0F };
//__constant vector3 RM_VECTOR3_FORWARD = { 0.0F, 0.0F, 1.0F };
//__constant vector3 RM_VECTOR3_BACKWARD = { 1.0F, -1.0F, 0.0F };


