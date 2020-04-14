#include <voxel.h>
#include <raymath.h>

voxel ic_voxel_create(int3 size, __global byte4 *colors)
{
    box3 box = rm_box3_create((float3)(0.0F, 0.0F, 0.0F), rm_quaternion_create(0.0F,0.0F,0.0F,1.0F), (float3)(size.x/2.0F,size.y/2.0F,size.z/2.0F));
    return (voxel){ size, colors, box};
}

byte4 ic_voxel_getColor(voxel *vox, int3 coordinates)
{
    return vox->colors[rm_math_flat_3d(vox->size, coordinates)];
}

void ic_voxel_setColor(byte4 color, voxel *vox, int3 coordinates)
{
    vox->colors[rm_math_flat_3d(vox->size, coordinates)] = color;
}

byte4 ic_voxel_ray(voxel v, float3 uv, float3 relativeAngle)
{
    int3 rayBlockPos = (int3)(uv * v.size);

    //return ic_voxel_getColor(&v, (int3)(8,8,0));

    //int3 volpixel;

    //volpixel.x = (int)(uv.x * v.size.x);
    //volpixel.y = (int)(uv.y * v.size.y);
    //volpixel.z = (int)(uv.z * v.size.y);

    //if(pointedX < v.size.x && pointedY < v.size.y && pointedZ < v.size.z)
    //{
        return ic_voxel_getColor(&v, rayBlockPos);
    //}
    //else
    //{
       // return (byte4){0,0,0,0};
    //}
}


