#ifndef _RAYMARCHER_BUILTINSHADER_H_
#define _RAYMARCHER_BUILTINSHADER_H_

#include <shader.h>

typedef struct unlit_shader
{
    shader base;
    color mainColor;
} unlit;

#endif