#ifndef _IC_VOXEL_H_
#define _IC_VOXEL_H_

#include <redef.h>
#include <physics.h>

typedef struct ic_voxel
{
    int3 size;
    __global byte4 *colors;
    box3 box;
} voxel;

voxel ic_voxel_create(int3 size, __global byte4 *colors);
byte4 ic_voxel_getColor(voxel *vox, int3 coordinates);
void ic_voxel_setColor(byte4 color, voxel *vox, int3 coordinates);
byte4 ic_voxel_ray(voxel v, float3 relativePoint, float3 relativeAngle);

#endif

