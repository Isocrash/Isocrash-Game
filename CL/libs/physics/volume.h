#ifndef _RAYMARCHER_VOLUME_H_
#define _RAYMARCHER_VOLUME_H_

#include <vector3.h>
#include <quaternion.h>
#include <redef.h>

typedef enum volume_type
{
    none = 0,
    ball = 1,
    box = 2
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
float rm_volume_squaredDistanceToBall(volume v, vector3 p);
float rm_vector3_scalar(vector3 u, vector3 v);

#endif

