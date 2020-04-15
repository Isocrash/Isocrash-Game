#include <quaternion.h>
#include <vector3.h>
#include <color.h>
#include <physics.h>
#include <redef.h>
#include <volume.h>
#include <raymath.h>
#include <voxel.h>

typedef struct gpu_camera
{
    int2 resolution;
    float fov;
    vector3 position;
    quaternion rotation;
    vector3 mainDirection;
    float farClipPlane;
    float precision;
    volume volume1;

} camera;

bool rm_render_raymarch(__global volume* volumes, size_t n, ray r, camera cam, hit* h);
ray rm_render_rayDirection(camera cam, int2 pixel);
color rm_render_skyColour(vector3 dir, vector3 sunDir);

__kernel void rm_render_entry(__global camera* input, __global float* time, __global int3* modelSize, __global byte4* modelColors, __global byte* output)
{   
    //while(true)
    //{
    size_t i = get_global_id(0);

    camera cam = *input;
    
    //size_t objectAmount = *n;

    size_t x = i % cam.resolution.x;
    size_t y = i / cam.resolution.x;

    int2 pixel;
    pixel.x = x;
    pixel.y = y; 

    ray r = rm_render_rayDirection(cam, pixel);
    color col = rm_render_skyColour(r.direction, cam.mainDirection);

    float sunAltitude = 1.0F;
    bool negative = cam.mainDirection.y < 0.0F;
    sunAltitude = fabs(cam.mainDirection.y);

    const color SunDayColour = rm_color_createFromKnown(white);
    const color SunSetColour = rm_color_createFromRGBA(1.0F, 0.33F, 0.0F, 1.0F);

    if(rm_vector3_squaredDistance(r.direction, cam.mainDirection) < 0.0025F && r.direction.y > 0.0F)
    {
        if(negative)
        {
            col = SunSetColour;
        }

        else
        {
            col = rm_color_lerp(SunSetColour, SunDayColour, sqrt(sunAltitude * 2.0F));
        }
    }
    
    //box3 box = rm_box3_create((float3) {0.0, 0.0F, -5.0F}, rm_quaternion_createFromEuler(/**time * 20.0F*/0.0F, 0.0F, 0.0F), (float3)(0.4F,0.6F,0.8F));

    quaternion modelRotation = rm_quaternion_createFromEuler(/**time * 20.0F*/0.0F, 0.0F, 0.0F);

    float3 ray3origin = (float3)(r.origin.x, r.origin.y, r.origin.z);
    float3 ray3direction = (float3)(r.direction.x, r.direction.y, r.direction.z);

    ray3 viewDir = rm_ray3_create(ray3origin, ray3direction);

    voxel model = ic_voxel_create_unit(*modelSize, modelColors, 
    (float3)
    (1.0F),
    
    
    
    
     modelRotation, (float3)(0.0F, 0.0F, 5.0F));


    float4 res;
    float3 relDir;
    float3 relPoint;
    if(rm_box3_oob(viewDir, model.box, &res, &relDir, &relPoint))
    {
        /*float3 uv = (float3)(
            (relPoint.x + model.box.extents.x) / model.resolution.x,
            (relPoint.y + model.box.extents.y) / model.resolution.y,
            (relPoint.z + model.box.extents.z) / model.resolution.z
        );*/
        
        
        //(float3)((relPoint.x + model.size.x / 2.0F) / model.size.x, (relPoint.y + model.size.y / 2.0F) / model.size.y, (relPoint.z + model.size.z / 2.0F) / model.size.z);
        //uv.y = 1.0F - uv.y;
        //uv.z = 0.0F;

        ray3 relRay = rm_ray3_create(relPoint, relDir);
        color receivedColor = ic_voxel_ray(model, relRay, 5.0F);

        if(receivedColor.a != 0)
        {
            color unlit = rm_color_createFromRGBA(relDir.x, relDir.y, relDir.z, 0.0F);
            //color render = rm_color_createFromRGBA(receivedColor.r/255.0F, receivedColor.g/255.0F, receivedColor.b/255.0F,1.0F);//receivedColor.a/255.0F);
            col = rm_color_combineAlpha(receivedColor, col);
        }
        /*float distance = res.x;
        float3 hitPoint = viewDir.origin + viewDir.direction * distance;
        float3 normal = res.yzw;

        //col = rm_color_createFromRGBA(distance, distance, distance, 1.0F);
        float angle = rm_vector3_angleDegree((vector3){normal.x, normal.y, normal.z}, cam.mainDirection);
        float lightIntensity = cos(angle * -1.0F * DEG_TO_RAD);

        color diffuse = rm_color_multiply(rm_color_createFromKnown(white), lightIntensity);
        col = diffuse;*/
        
    }

    output[i * 4 + 0] = col.b * 255;    //Blue
    output[i * 4 + 1] = col.g * 255;    //Green
    output[i * 4 + 2] = col.r * 255;    //Red
    output[i * 4 + 3] = col.a * 255;    //Alpha
}

float rm_render_closestDistance(__global volume* volumes, size_t n, vector3 p, int * index)
{
    float closest = INFINITY;

    for (size_t i = 0; i < n; i++)
    {
        float d = rm_volume_squaredDistance(volumes[i], p);
        if(d < closest)
        {
            * index = i;
            closest = d;
        }
    }

    return closest;
}

bool rm_render_raymarch(__global volume* volumes, size_t n, ray r, camera cam, hit* h)
{
    

    vector3 probe = r.origin;
    float rayDst = 0.0F;
    float maxDst = 100.0F;

    bool hasHit = false;
    
    
    float precision = cam.precision;

    while(!hasHit && rayDst < maxDst)
    {
        int closestIndex = 1;
        /*rm_volume_squaredDistance(volumes[0], probe);*/
        float dst = rm_render_closestDistance(volumes, n, probe, &closestIndex);
        if(dst < precision)
        {
            hasHit = true;
            *h = rm_hit_create(probe, rm_volume_normal(volumes[(int)closestIndex], probe));
        }

        probe = rm_vector3_add(probe, rm_vector3_multiply(r.direction, dst));
        rayDst += dst;
    }

    return hasHit;
}

ray rm_render_rayDirection(camera cam, int2 pixel)
{
    // https://www.scratchapixel.com/lessons/3d-basic-rendering/ray-tracing-generating-camera-rays/generating-camera-rays?http://www.scratchapixel.com/lessons/3d-basic-rendering/3d-viewing-pinhole-camera/how-pinhole-camera-works-part-1
    int x = pixel.x;
    int y = pixel.y;

    float imageAspectRatio = cam.resolution.x / (float)cam.resolution.y; // assuming width > height 
    float Px = (2.0F * ((x + 0.5F) / cam.resolution.x) - 1.0F) * tan(cam.fov / 2.0F * F_PI / 180.0F) * imageAspectRatio;
    float Py = (1.0F - 2.0F * ((y + 0.5F) / cam.resolution.y)) * tan(cam.fov / 2.0F * F_PI / 180.0F); 

    vector3 rayOrigin = cam.position;
    vector3 rayDirection = rm_vector3_create(Px, Py, 1.0F);
    
    rayDirection = rm_vector3_normalize(rayDirection);
    rayDirection = rm_quaternion_multiply(cam.rotation, rayDirection);

    return rm_ray_create(rayOrigin, rayDirection);
}

color rm_render_skyColour(vector3 dir, vector3 sunDir)
{
    sunDir = rm_vector3_normalize(sunDir);

    const color HorizonColourDay = rm_color_createFromRGBA(0.82F, 0.92F, 0.98F, 1.0F);
    const color HorizonColourSunset = rm_color_createFromRGBA(1.0F, 0.48F, 0.0F, 1.0F);
    const color NightColour = rm_color_createFromRGBA(0.0F, 0.0F, 0.0F, 1.0F);
    const color HighAtmosphereColour = rm_color_createFromRGBA(0.23F, 0.41F, 0.70F, 1.0F);
    const color GroundAtmosphereColour = rm_color_createFromRGBA(0.58F, 0.53F, 0.45F, 1.0F);

    color pix = rm_color_createFromRGBA(0.0F,0.0F,0.0F,1.0F);

    if(sunDir.x == 0.0F && sunDir.y == 0.0F && sunDir.z == 0.0F)
    {
        return pix;
    }

    float sunAltitude = 1.0F;

    bool negative = sunDir.y < 0.0F;
    sunAltitude = fabs(sunDir.y);

    color DenseAtmosphereColour = rm_color_lerp(HorizonColourSunset, HorizonColourDay, sqrt(sunAltitude));
    color highAtmosphereColour = rm_color_lerp(NightColour, HighAtmosphereColour, pow(sunAltitude, 0.6F));
    color groundAtmosphereColour = rm_color_lerp(NightColour, GroundAtmosphereColour, pow(sunAltitude, 0.6F));

    if(negative)
    {
        DenseAtmosphereColour = rm_color_lerp(HorizonColourSunset, NightColour, sqrt(sunAltitude));
        highAtmosphereColour = NightColour;
        groundAtmosphereColour = NightColour;
    }

    if (dir.y > 0.0F)
    {
        pix = rm_color_lerp(DenseAtmosphereColour, highAtmosphereColour, pow(dir.y, 0.8F));
    }
    else
    {
        pix = rm_color_lerp(DenseAtmosphereColour, groundAtmosphereColour, pow(fabs(dir.y), 0.25F));
    }

    return pix;
}

