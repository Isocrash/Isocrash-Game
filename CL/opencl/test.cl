#include <raymath.h>
#include <colorimetry.h>

typedef struct
{
    int2 resolution;
    float fov;
    vector3 position;
    quaternion rotation;
    vector3 mainDirection;
    float farClipPlane;
    float precision;
} camera;

typedef struct
{
    vector3 point;
    colour128 colour;
    bool hasHit;
    float closest;
    vector3 normal;
} hitInfos;

ray pixelRay(camera cam, int2 pixel);
colour128 SkyColour(vector3 dir, vector3 sunDir);
hitInfos Hit(ray cray, camera cam); 
float closest(vector3 point);
vector3 SphereNormal(vector3 p, vector3 sp);


__kernel void CL_TEST(__global camera* input, __global byte* output)
{
    size_t i = get_global_id(0); // Indice du "tableau" (0 => 1 dimension, 1 => 2 dimensions, 2 => 2 dimensions)
    
    camera cam = *input;
    size_t x = i % 1024;//cam.resolution.x; //position pixel X
    size_t y = i / 1024;//cam.resolution.x; //position pixel Y
    
    int2 pixel;
    pixel.x = x;
    pixel.y = y; 

    ray r;
    r = pixelRay(cam, pixel);

    colour128 col = SkyColour(r.direction, cam.mainDirection);
    hitInfos hit = Hit(r, cam);

    hit.normal = vdirection(hit.normal);

    if(hit.hasHit)
    {
        col.r = map(hit.normal.x, -1.0F, 1.0F, 0.0F, 1.0F);//hit.colour;//c_colour128(1.0F, 1.0F, 1.0F, 1.0F);
        col.g = map(hit.normal.y, -1.0F, 1.0F, 0.0F, 1.0F);
        col.b = map(hit.normal.z, -1.0F, 1.0F, 0.0F, 1.0F);
    }

    else
    {
        float sunAltitude = 1.0F;
        bool negative = cam.mainDirection.y < 0.0F;
        sunAltitude = fabs(cam.mainDirection.y);

        const colour128 SunDayColour = c_colour128(1.0F, 1.0F, 1.0F, 1.0F);
        const colour128 SunSetColour = c_colour128(1.0F, 0.33F, 0.0F, 1.0F);

        if(vdistance(r.direction, cam.mainDirection) < 0.0025F && r.direction.y > 0.0F)
        {
            if(negative)
            {
                col = SunSetColour;
            }

            else
            {
                col = clerp(SunSetColour, SunDayColour, sqrt(sunAltitude * 2.0F));
            }
        }
    }
    
    output[i * 4 + 0] = col.b * 255;    //Blue
    output[i * 4 + 1] = col.g * 255;    //Green
    output[i * 4 + 2] = col.r * 255;    //Red
    output[i * 4 + 3] = col.a * 255;    //Alpha
};

colour128 SkyColour(vector3 dir, vector3 sunDir)
{
    sunDir = vdirection(sunDir);

    const colour128 HorizonColourDay = c_colour128(0.82F, 0.92F, 0.98F, 1.0F);
    const colour128 HorizonColourSunset = c_colour128(1.0F, 0.48F, 0.0F, 1.0F);
    const colour128 NightColour = c_colour128(0.0F, 0.0F, 0.0F, 1.0F);
    const colour128 HighAtmosphereColour = c_colour128(0.23F, 0.41F, 0.70F, 1.0F);
    const colour128 GroundAtmosphereColour = c_colour128(0.58F, 0.53F, 0.45F, 1.0F);

    colour128 pix = c_colour128(0.0F,0.0F,0.0F,1.0F);

    if(sunDir.x == 0.0F && sunDir.y == 0.0F && sunDir.z == 0.0F)
    {
        return pix;
    }

    float sunAltitude = 1.0F;

    bool negative = sunDir.y < 0.0F;
    sunAltitude = fabs(sunDir.y);

    colour128 DenseAtmosphereColour = clerp(HorizonColourSunset, HorizonColourDay, sqrt(sunAltitude));
    colour128 highAtmosphereColour = clerp(NightColour, HighAtmosphereColour, pow(sunAltitude, 0.6F));
    colour128 groundAtmosphereColour = clerp(NightColour, GroundAtmosphereColour, pow(sunAltitude, 0.6F));

    if(negative)
    {
        DenseAtmosphereColour = clerp(HorizonColourSunset, NightColour, sqrt(sunAltitude));
        highAtmosphereColour = NightColour;
        groundAtmosphereColour = NightColour;
    }

    if (dir.y > 0.0F)
    {
        pix = clerp(DenseAtmosphereColour, highAtmosphereColour, pow(dir.y, 0.8F));
    }
    else
    {
        pix = clerp(DenseAtmosphereColour, groundAtmosphereColour, pow(fabs(dir.y), 0.25F));
    }

    return pix;
}


ray pixelRay(camera cam, int2 pixel)
{
    // https://www.scratchapixel.com/lessons/3d-basic-rendering/ray-tracing-generating-camera-rays/generating-camera-rays?http://www.scratchapixel.com/lessons/3d-basic-rendering/3d-viewing-pinhole-camera/how-pinhole-camera-works-part-1
    int x = pixel.x;
    int y = pixel.y;

    float imageAspectRatio = cam.resolution.x / (float)cam.resolution.y; // assuming width > height 
    float Px = (2.0F * ((x + 0.5F) / cam.resolution.x) - 1.0F) * tan(cam.fov / 2.0F * F_PI / 180.0F) * imageAspectRatio;
    float Py = (1.0F - 2.0F * ((y + 0.5F) / cam.resolution.y)) * tan(cam.fov / 2.0F * F_PI / 180.0F); 

    vector3 rayOrigin = cam.position;
    vector3 rayDirection;
    rayDirection.x = Px;
    rayDirection.y = Py;
    rayDirection.z = 1.0;
    
    rayDirection = vdirection(rayDirection);
    rayDirection = qmultiplyv(rayDirection, cam.rotation);
    
    ray r;
    r.origin = rayOrigin;
    r.direction = rayDirection;

    return r;
}

hitInfos Hit(ray cray, camera cam)
{
    vector3 probe = cray.origin;
    vector3 dir = cray.direction;

    float rayDst = 0.0F;
    float maxDst = 10.0F;
    float precision = cam.precision;

    hitInfos hit;
    hit.closest = INFINITY;

    while(rayDst < maxDst)
    {
        float closestDst = closest(probe);

        if(closestDst < hit.closest)
        {
            hit.closest = closestDst;
        }

        if(closestDst < precision)
        {
            hit.point = probe;
            hit.colour = c_colour128(1.0F, 1.0F, 1.0F, 1.0F);
            hit.normal = SphereNormal(probe, (vector3){ 0.0F, 1.0F, 0.0F });
            hit.hasHit = true;

            return hit;
        }

        else
        {
            probe.x = probe.x + dir.x * closestDst;
            probe.y = probe.y + dir.y * closestDst;
            probe.z = probe.z + dir.z * closestDst;

            rayDst += closestDst;

            if(rayDst > maxDst)
            {
                hit.hasHit = false;
                return hit;
            }
        }
    }
}

vector3 SphereNormal(vector3 p, vector3 sp)
{
    vector3 relPos = (vector3){p.x - sp.x, p.y - sp.y, p.z - sp.z};
    return vdirection(relPos);
}

float DistanceToSphereSQRT(vector3 p, vector3 c, float radius)
{
    return sqrt((c.x - p.x) * (c.x - p.x) + (c.y - p.y) * (c.y - p.y) + (c.z - p.z) * (c.z - p.z)) - radius;
}  

float closest(vector3 point)
{
    return DistanceToSphereSQRT(point, (vector3){0.0F,0.0F,0.0F}, 0.5F);
}

