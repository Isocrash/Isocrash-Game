#include <physics.h>
#include <raymath.h>
#include <redef.h>
#include <quaternion.h>
#include <vector3.h>

box3 rm_box3_create(float3 center, quaternion rotation, float3 extends)
{
    return (box3){center, rotation, extends};
}

//http://iquilezles.org/www/articles/boxfunctions/boxfunctions.htm
//https://www.shadertoy.com/view/ld23DV

//result x = distance, yzw: normal
bool rm_box3_oob(
    ray3 r, /*ray, world space*/
    box3 box, /*box, world space*/
    float4* result,
    float3* receivedRelDir,
    float3* receivedRelPoint
    )
{
    float3 ro = r.origin;
    float3 rd = r.direction;
    float3 rad = box.extents;


    //1. rotate ray origin around the box by the its opposed rotation
    vector3 boxEuler = rm_quaternion_toEuler(box.rotation);
    float3 opposedRotation = (float3)(180.0F) - (float3)(boxEuler.x, boxEuler.y, boxEuler.z);
    vector3 rotated = 
    rm_vector3_rotateAroundPivot(
        (vector3){ro.x, ro.y, ro.z}, 
        (vector3){box.center.x, box.center.y, box.center.z}, 
        rm_quaternion_inverse(box.rotation)
        );
    float3 roo = (float3)(rotated.x, rotated.y, rotated.z);

    //2. set the position to relative
    roo = roo - box.center;

    //3. rotate the ray (box rotation)
    rotated = 
    rm_vector3_rotateAroundPivot(
        (vector3){rd.x, rd.y, rd.z}, 
        (vector3){0.0F, 0.0F, 0.0F}, 
        rm_quaternion_inverse(box.rotation)
        );
    
    float3 rdd = (float3)(rotated.x, rotated.y, rotated.z);
    
    // ray-box intersection in box space
    float3 m = 1.0F/rdd;
    float3 n = m*roo;
    float3 k = fabs(m)*rad;
	
    float3 t1 = -n - k;
    float3 t2 = -n + k;

	float tN = max( max( t1.x, t1.y ), t1.z );
	float tF = min( min( t2.x, t2.y ), t2.z );


    if( tN > tF || tF < 0.0F)
    {
        *result = (float4)(-1.0F);
        return false;
    }
    else
    {
        float3 normal = -sign(rdd) * step(t1.yzx, t1.xyz) * step(t1.zxy,t1.xyz);
        //normal = rm_mat4_mult_float4(txi, (float4)(normal, 0.0F)).xyz;
        *receivedRelDir = rdd;
        *receivedRelPoint = (ro + rd * tN) - box.center;
        *result = (float4)(tN, normal);

        return true;
    }
    
}

//bool rm_box3_aabb()

