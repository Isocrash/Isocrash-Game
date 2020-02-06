#include <raymath.h>

vector3 qmultiplyv(vector3 v, quaternion q)
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



    v.x = (1.0F - (yy + zz)) * v.x + (xy - wz) * v.y + (xz + wy) * v.z;
    v.y = (xy + wz) * v.x + (1.0F - (xx + zz)) * v.y + (yz - wx) * v.z,
    v.z = (xz - wy) * v.x + (yz + wx) * v.y + (1.0F - (xx + yy)) * v.z;

    return v;
}

float3 qmultiplyf3(float3 v, quaternion q)
{
    vector3 vect;
    vect.x = v.x;
    vect.y = v.y;
    vect.z = v.z;

    vector3 vect3 = qmultiplyv(vect, q);

    v.x = vect3.x;
    v.y = vect3.y;
    v.z = vect3.z;

    return v;
}

vector3 toEuler(quaternion q)
{
    vector3 result;

    // Roll (X)
    float sinr_cosp = 2.0F * (q.w * q.x + q.y * q.z);
    float cosr_cosp = 1.0F - 2.0F * (q.x * q.x + q.y * q.y);  
    result.x = atan2f(sinr_cosp, cosr_cosp) * (180.0F / F_PI);

    // Pitch (Y)
    float sinp = 2.0F * (q.w * q.y - q.z * q.x);
    if (fabs(sinp) >= 1.0F)
    {
        result.y = copysignf(M_PI_2, sinp) * (180.0F / F_PI);
    }
    else
    {
        result.y = asinf(sinp) * (180.0F / F_PI);
    }

    // Yaw (Z)
    float siny_cosp = 2.0F * (q.w * q.z + q.x * q.y);
    float cosy_cosp = 1.0F - 2.0F * (q.w * q.y + q.z * q.z);
    result.z = atan2f(siny_cosp, cosy_cosp) * (180.0F / F_PI);

    return result;
}


