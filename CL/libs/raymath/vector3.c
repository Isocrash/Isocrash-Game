#include <vector3.h>
#include <raymath.h>

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

float rm_vector3_scalar(vector3 u, vector3 v)
{
    return u.x * v.x + u.y * v.y + u.z * v.z;
}

float rm_vector3_angle(vector3 v1, vector3 v2)
{
    return acos(rm_vector3_scalar(v1, v2) / (rm_vector3_length(v1) * rm_vector3_length(v2)));
}

float rm_vector3_angleDegree(vector3 v1, vector3 v2)
{
    return rm_vector3_angle(v1, v2) * (180.0F / F_PI);
}


//__constant vector3 RM_VECTOR3_UNIT = { 1.0F, 1.0F, 1.0F };
//__constant vector3 RM_VECTOR3_RIGHT = { 1.0F, 0.0F, 0.0F };
//__constant vector3 RM_VECTOR3_LEFT = { -1.0F, 0.0F, 0.0F };
//__constant vector3 RM_VECTOR3_UP = { 0.0F, 1.0F, 0.0F };
//__constant vector3 RM_VECTOR3_DOWN = { 0.0F, -1.0F, 0.0F };
//__constant vector3 RM_VECTOR3_FORWARD = { 0.0F, 0.0F, 1.0F };
//__constant vector3 RM_VECTOR3_BACKWARD = { 1.0F, -1.0F, 0.0F };


