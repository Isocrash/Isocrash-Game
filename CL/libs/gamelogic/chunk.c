#include <chunk.h>

chunk ic_create_chunk(int2 coordinates, unsigned short * blocks)
{
    return (chunk){ coordinates, blocks };
}

