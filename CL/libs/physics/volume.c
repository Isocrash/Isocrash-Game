#include <volume.h>
#include <raymath.h>
//#include <quaternion.h>

bool rm_volume_equals(volume v1, volume v2)
{
    return 
        rm_vector3_equals(v1.position, v2.position) &&
        rm_quaternion_equals(v1.rotation, v2.rotation) &&
        rm_vector3_equals(v1.scale, v2.scale) &&
        v1.type == v2.type;
}

volume rm_volume_create(vector3 pos, quaternion rot, vector3 sca, volumeType type)
{
    return (volume){pos, rot, sca, type};
}

//TODO: take in count scale
float rm_volume_distanceToBall(volume v, vector3 p)
{
    vector3 vp = v.position;
    vector3 vs = v.scale;
    
    return
        (vp.x - p.x) * (vp.x - p.x) +
        (vp.y - p.y) * (vp.y - p.y) +
        (vp.z - p.z) * (vp.z - p.z) -
        vs.z * vs.z;
}

float rm_volume_squaredDistanceToBall(volume v, vector3 p)
{
    vector3 vp = v.position;
    vector3 vs = v.scale;
    
    return sqrt(
        (vp.x - p.x) * (vp.x - p.x) +
        (vp.y - p.y) * (vp.y - p.y) +
        (vp.z - p.z) * (vp.z - p.z)) - vs.z / 2.0F;
}

//TODO: take in count the rotation
float rm_volume_distanceToBox(volume v, vector3 p)
{
    vector3 vp = v.position;

    
    vector3 vs = rm_vector3_divide(v.scale, 2.0F);

    quaternion rot = v.rotation;

    p = rm_vector3_rotateAroundPivot(p, vp, rot);

    float x = rm_math_max(0.0F, fabs(p.x - vp.x) - vs.x);
    float y = rm_math_max(0.0F, fabs(p.y - vp.y) - vs.y);
    float z = rm_math_max(0.0F, fabs(p.z - vp.z) - vs.z);

    return x * x + y * y + z * z;
}

float rm_volume_distance(volume v, vector3 p)
{
    switch (v.type)
    {
        case box: return rm_volume_distanceToBox(v, p);
        case ball: return rm_volume_distanceToBall(v, p);
        
        default: return INFINITY;
    }
}

float rm_volume_squaredDistance(volume v, vector3 p)
{
    switch (v.type)
    {
        case box: return sqrt(rm_volume_distanceToBox(v, p));
        case ball: return rm_volume_squaredDistanceToBall(v, p);
        
        default: return INFINITY;
    }
}

//TODO: take in count scale
vector3 rm_volume_normalToBall(volume v, vector3 p)
{
    return rm_vector3_normalize(rm_vector3_substract(p, v.position));
}

//TODO: take in count the rotation
vector3 rm_volume_normalToBox(volume v, vector3 p)
{
    vector3 zero;
    //p = rm_vector3_rotateAroundPivot(p, v.position, v.rotation);
    
    vector3 rp = rm_vector3_substract(p, v.position);

    vector3 n = rm_vector3_create(1.0F, 0.0F, 0.0F);
    vector3 vs = rm_vector3_divide(v.scale, 2.0F);

    
    if (rp.y > vs.y)
    {
        n.x = 0.0F;
        n.y = 1.0F;
    }
    else if (rp.y < -vs.y)
    {
        n.x = 0.0F;
        n.y = -1.0F;
    }

    // Defaults
    /*else if (rp.x > vs.x)
    {
        n.x = 1.0F;
    }*/
    
    else if (rp.x < -vs.x)
    {
        n.x = -1.0F;
    }
    
    else if (rp.z > vs.z)
    {
        n.x = 0.0F;
        n.z = 1.0F;
    }

    else if (rp.z < -vs.z)
    {
        n.x = 0.0F;
        n.z = -1.0F;
    }

    return rm_vector3_normalize(n);
}

vector3 rm_volume_normal(volume v, vector3 p)
{
    switch (v.type)
    {
        case box: return rm_volume_normalToBox(v, p);
        case ball: return rm_volume_normalToBall(v, p);
        
        default: return (vector3){ 1.0F, 0.0F, 0.0F };
    }
}

