#ifndef _IC_VOXEL_H_
#define _IC_VOXEL_H_

#include <redef.h>
#include <physics.h>

typedef struct ic_voxel
{
    int3 resolution;
    __global byte4 *colors;
    box3 box;
} voxel;

voxel ic_voxel_create(int3 resolution, __global byte4 *colors, float3 scale);
voxel ic_voxel_create_unit(int3 resolution, __global byte4 *colors, float3 scale, quaternion rotation, float3 position);
byte4 ic_voxel_getColor(voxel *vox, int3 coordinates);
void ic_voxel_setColor(byte4 color, voxel *vox, int3 coordinates);
color ic_voxel_ray(voxel v, ray3 r, float quality);

#endif

