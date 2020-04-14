#ifndef _IC_CHUNK_H_
#define _IC_CHUNK_H_

#define CHUNK_WIDTH 16;
#define CHUNK_DEPTH 16;
#define CHUNK_HEIGHT 256;
#define CHUNK_MAX_BLOCKS 65536‬;

typedef struct ic_chunk
{
    int2 coordinates;
    unsigned short *blocks;
} chunk;


chunk ic_create_chunk(int2 coordinates, unsigned short * blocks);

#endif

