float map(float value, float oldLow, float oldHigh, float newLow, float newHigh)
{
    return newHigh + (value - oldLow) * (newHigh - newLow) / (oldHigh - oldLow);
}

float copysignf(float x, float y)
{
    return y == 0 ? x : fabs(x) * (y/fabs(y));
}

double copysignd(double x, double y)
{
    return y == 0 ? x : dabs(x) * (y/dabs(y));
}

double dabs(double d)
{
    return d < 0.0 ? d * -1.0 : d;
}

float vlength(vector3 v)
{
    return sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
}

vector3 vdirection(vector3 v)
{
    return vdividef(v, vlength(v));
}

vector3 vdividef(vector3 v, float diviser)
{
    v.x = v.x / diviser;
    v.y = v.y / diviser;
    v.z = v.z / diviser;

    return v;
}

vector3 vrotateEuler(vector3 v, float x, float y, float z)
{
    x *= DEG_TO_RAD;
    y *= DEG_TO_RAD;
    z *= DEG_TO_RAD;

    //X Rotation
    v.x = v.x;
    v.y = v.y * cosf(x) - v.z * sinf(x);
    v.z = v.y * sinf(x) + v.z * cosf(x);

}

float3 f3dividef(float3 v, float diviser)
{
    v.x = v.x / diviser;
    v.y = v.y / diviser;
    v.z = v.z / diviser;

    return v;
}

float f3length(float3 v)
{
    return sqrt(v.x * v.x + v.y * v.y + v.z * v.z);
}

float3 f3direction(float3 v)
{
    return f3dividef(v, f3length(v));
}
