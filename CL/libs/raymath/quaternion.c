#include <quaternion.h>
#include <vector3.h>

bool rm_quaternion_equals(quaternion q1, quaternion q2)
{
    return q1.x == q2.x && q1.y == q2.y && q1.z == q2.z && q1.w == q2.w;
}

quaternion rm_quaternion_create(float x, float y, float z, float w)
{
    return (quaternion){ x, y, z, w };
}

vector3 rm_quaternion_multiply(quaternion q, vector3 v)
{
    float x = q.x * 2.0F;
    float y = q.y * 2.0F;
    float z = q.z * 2.0F;

    float xx = q.x * x;
    float yy = q.y * y;
    float zz = q.z * z;

    float xy = q.x * y;
    float xz = q.x * z;

    float yz = q.y * z;

    float wx = q.w * x;
    float wy = q.w * y;
    float wz = q.w * z;

    return (vector3)
    {
        (1.0F - (yy + zz)) * v.x + (xy - wz) * v.y + (xz + wy) * v.z,
        (xy + wz) * v.x + (1.0F - (xx + zz)) * v.y + (yz - wx) * v.z,
        (xz - wy) * v.x + (yz + wx) * v.y + (1.0F - (xx + yy)) * v.z
    };
}

