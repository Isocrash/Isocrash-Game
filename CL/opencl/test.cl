

#include <raymath.h>

typedef struct
{
    int2 resolution;
    float fov;
    vector3 position;
    quaternion rotation;
} camera;

ray pixelRay(camera cam, int2 pixel);

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

    vector3 dir = r.direction;
    dir.x = map(dir.x, -1.0F, 1.0F, 0.0F, 255.0F);
    dir.y = map(dir.y, -1.0F, 1.0F, 0.0F, 255.0F);
    dir.z = map(dir.z, -1.0F, 1.0F, 0.0F, 255.0F);



    output[i * 4] = (int)(dir.x);//(int)(dir.z); // Blue
    output[i * 4 + 1] = (int)(dir.y); // Green
    output[i * 4 + 2] = (int)(dir.z);//(int)(dir.x); // Red
    
    output[i * 4 + 3] = 255; // Alpha
};


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
    rayDirection.z = -1.0;
    
    rayDirection = vdirection(rayDirection);

    rayDirection = qmultiplyv(rayDirection, cam.rotation);
    
    ray r;
    r.origin = rayOrigin;
    r.direction = rayDirection;

    return r;
}


