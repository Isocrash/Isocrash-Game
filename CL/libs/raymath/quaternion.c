#include <quaternion.h>
#include <vector3.h>
#include <raymath.h>

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

vector3 rm_vector3_rotateAroundPivot(vector3 point, vector3 pivot, quaternion rotation)
{
    vector3 dir = rm_vector3_substract(point, pivot);
    dir = rm_quaternion_multiply(rotation, dir);
    return rm_vector3_add(dir, pivot);
}


vector3 rm_quaternion_toEuler(quaternion q)
{
    vector3 angles;

    // roll (X-axis rotation)
    float sinr_cosp = 2.0F * (q.w * q.x + q.y * q.z);
    float cosr_cosp = 1.0F - 2.0F * (q.x * q.x + q.y * q.y);
    angles.x = atan2(sinr_cosp, cosr_cosp);

    // pitch (Y-axis rotation)
    float sinp = 2.0F * (q.w * q.y - q.z * q.x);
    if (fabs(sinp) >= 1)
    {
        // use 90 degrees if out of range
        angles.y = rm_math_copysignf(F_PI / 2.0F, sinp);
    }
    else
    {
        angles.y = asin(sinp);
    }

    // yaw (Z-axis rotation)
    float siny_cosp = 2.0F * (q.w * q.z + q.x * q.y);
    float cosy_cosp = 1.0F - 2.0F * (q.y * q.y + q.z * q.z);
    angles.z = atan2(siny_cosp, cosy_cosp);

    //if(angles.x < 0.0F) angles.x = -angles.x;
    //if(angles.y < 0.0F) angles.y = -angles.y;
    if(angles.z < 0.0F) angles.z = -angles.z;

    return rm_vector3_multiply(angles , RAD_TO_DEG);
}


// Source : https://referencesource.microsoft.com/System.Numerics/System/Numerics/Quaternion.cs.html#062fb725b5ca1d1e
quaternion rm_quaternion_createFromEuler(float x, float y, float z)
{

    x = fmod(x, 360.0F);
    y = fmod(y, 360.0F);
    z = fmod(z, 360.0F);

    x *= DEG_TO_RAD;
    y *= DEG_TO_RAD;
    z *= DEG_TO_RAD;

    //  Roll first, about axis the object is facing, then
    //  pitch upward, then yaw to face into the new heading
    float sr, cr, sp, cp, sy, cy;

    float halfRoll = z * 0.5f;
    sr = sin(halfRoll);
    cr = cos(halfRoll);

    float halfPitch = x * 0.5f;
    sp = sin(halfPitch);
    cp = cos(halfPitch);

    float halfYaw = y * 0.5f;
    sy = sin(halfYaw);
    cy = cos(halfYaw);

    quaternion q;
    q.x = cy * sp * cr + sy * cp * sr;
    q.y = sy * cp * cr - cy * sp * sr;
    q.z = cy * cp * sr - sy * sp * cr;
    q.w = cr * cp * cr + sy * sp * sr;

    return q;

}

quaternion rm_quaternion_inverse(quaternion value)
{
    quaternion ans = rm_quaternion_create(0.0F,0.0F,0.0F,1.0F);
 
    float ls = value.x * value.x + value.y * value.y + value.z * value.z + value.w * value.w;
    float invNorm = 1.0f / ls;
 
    ans.x = -value.x * invNorm;
    ans.y = -value.y * invNorm;
    ans.z = -value.z * invNorm;
    ans.w = value.w * invNorm;
 
    return ans;
}

