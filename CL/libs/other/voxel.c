#include <voxel.h>
#include <raymath.h>
#include <color.h>


voxel ic_voxel_create(int3 resolution, __global byte4 *colors, float3 scale)
{
    box3 box = rm_box3_create(
        (float3)(0.0F, 0.0F, 0.0F),
         rm_quaternion_create(0.0F,0.0F,0.0F,1.0F),
          (float3)(
              (resolution.x / 2.0F) * scale.x,
              (resolution.y / 2.0F) * scale.y,
              (resolution.z / 2.0F) * scale.z
              ));
    return (voxel){ resolution, colors, box};
}

voxel ic_voxel_create_unit(int3 resolution, __global byte4 *colors, float3 scale, quaternion rotation, float3 position)
{
    int max = resolution.x;
    if(resolution.y > max) max = resolution.y;
    if(resolution.z > max) max = resolution.z;

    box3 box = rm_box3_create(
        position, 
        rotation, 
        (float3)((((float)resolution.x / max) / 2.0F) * scale.x, (((float)resolution.y / max) / 2.0F) * scale.y, (((float)resolution.z / max) / 2.0F)*scale.z));
        
    return (voxel){ resolution, colors, box};
}

byte4 ic_voxel_getColor(voxel *vox, int3 coordinates)
{
    return vox->colors[rm_math_flat_3d(vox->resolution, coordinates)];
}

void ic_voxel_setColor(byte4 color, voxel *vox, int3 coordinates)
{
    vox->colors[rm_math_flat_3d(vox->resolution, coordinates)] = color;
}

color ic_voxel_ray(voxel v, ray3 r, float quality)
{
    const float basequality = 0.05F;

    float max = v.box.extents.x;
    if(v.box.extents.y > max) max = v.box.extents.y;
    if(v.box.extents.z > max) max = v.box.extents.z;

    float sampleResolution = basequality * (1.0F/quality) * max;

    float3 samplePos = r.origin;
    r.direction = normalize(r.direction);
    samplePos += r.direction * sampleResolution;

    color col = rm_color_createFromRGBA(0.0F,0.0F,0.0F,0.0F);
    byte4 previous = (byte4){0,0,0,0};

    while(
        samplePos.x >= -v.box.extents.x && samplePos.y >= -v.box.extents.y && samplePos.z >= -v.box.extents.z &&
        samplePos.x <= v.box.extents.x && samplePos.y <= v.box.extents.y && samplePos.z <= v.box.extents.z
        && col.a != 1.0F
    )
    {
        float3 localPos = samplePos + v.box.extents;

        //inverse y
        localPos.y = v.box.extents.y * 2.0F - localPos.y;
        
        localPos /= v.box.extents * 2.0F;

        float3 res = (float3)(v.resolution.x, v.resolution.y, v.resolution.z);
        localPos *= res;

        
        int3 coords = (int3)(floor(localPos.x), floor(localPos.y), floor(localPos.z));

        if(coords.x < 0) coords.x = 0;
        else if (coords.x > v.resolution.x - 1) coords.x = v.resolution.x - 1;
        
        if(coords.y < 0) coords.y = 0;
        else if (coords.y > v.resolution.y - 1) coords.y = v.resolution.y - 1;

        if(coords.z < 0) coords.z = 0;
        else if (coords.z > v.resolution.z - 1) coords.z = v.resolution.z - 1;

        
        byte4 sample = ic_voxel_getColor(&v, coords);
        
        if( !(previous.r == sample.r&&
            previous.g == sample.g&&
            previous.b == sample.b&&
            previous.a == sample.a)&&
            
            sample.a != 0)
        {
            if(col.r == 0.0F && col.g == 0.0F && col.b == 0.0F)
            {
                col = rm_color_createFromRGBA(sample.r / 255.0F, sample.g / 255.0F, sample.b / 255.0F, sample.a / 255.0F);
            }

            else
            {
                col = rm_color_combineAlpha(col,rm_color_createFromRGBA(sample.r / 255.0F, sample.g / 255.0F, sample.b / 255.0F, sample.a / 255.0F)); 
            }         
        }

        samplePos += r.direction * sampleResolution;
        previous = sample;
    }

    return col;
}


