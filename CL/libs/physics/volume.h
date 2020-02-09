#ifndef _RAYMARCHER_VOLUME_H_
#define _RAYMARCHER_VOLUME_H_

#include <vector3.h>
#include <quaternion.h>
#include <redef.h>

typedef enum volume_type
{
    ball,
    box
} volumeType;

typedef struct vol
{
    vector3 position;
    quaternion rotation;
    vector3 scale;
    volumeType type;
} volume;

bool rm_volume_equals(volume v1, volume v2);
float rm_volume_distance(volume v, vector3 p);
float rm_volume_squaredDistance(volume v, vector3 p);
vector3 rm_volume_normal(volume v, vector3 p);


#endif

